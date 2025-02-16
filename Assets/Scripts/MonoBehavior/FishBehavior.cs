using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FishBehavior: MonoBehaviour
{ 
    public Vector3 moveDirection;
    public Vector3 targetDirection;
    public float moveSpeed = 1f;
    public float turningrate = 50f; // degree per second
    public float blindAngle = 90f; //360 - alpha
    public GameObject fishBody;
    public Material normalMaterial;
    public Material repulsionMaterial;
    public Material orientationMaterial;
    public Material attractionMaterial;
    public Material mixedMaterial;
    public float r1 = 1f; // zone of repulsion
    public float r2 = 15f; // zone of orientation
    public float r3 = 30f; // zone of attraction
    Rigidbody rb;
    public int fishLayerMask = 64;
    public float sigma = 5f;
    // Start is called before the first frame update
    void Start()
    {
        //fishLayerMask = LayerMask.GetMask("FishBehavior");
        //random direction
        do
        {
            moveDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        } 
        while (moveDirection == Vector3.zero);
        moveDirection = moveDirection.normalized;   
        rb = GetComponent<Rigidbody>();
    }
    void OnCollisionEnter(Collision collision){
        
        if(collision.gameObject.CompareTag("wall")){
            Vector3 normal = collision.contacts[0].normal;
            Debug.Log("Normal is: " + normal);
            moveDirection = Vector3.Reflect(moveDirection, normal);
            moveDirection = AddGaussianNoise(moveDirection, sigma);
            transform.position += moveDirection.normalized * 0.1f; // small offset to avoid collision
        } 
    }


    public void SwimBehavior()
    {
       // check for nearby fish within r1, r2, and r3
        Collider[] collidersR1 = Physics.OverlapSphere(transform.position, r1, fishLayerMask); 
        collidersR1 = collidersR1.Where(c => c.gameObject != gameObject).ToArray();
        Vector3 newMoveDirection = Vector3.zero;
        if(collidersR1.Length > 0)
        {
            collidersR1 = AddBlindArea(collidersR1);
            newMoveDirection = Equ_Repulsion(collidersR1); 
        }else{
            Collider[] collidersR2 = Physics.OverlapSphere(transform.position, r2, fishLayerMask);
            collidersR2 = collidersR2.Except(collidersR1).ToArray(); // remove r1 fish from r2 list
            collidersR2 = collidersR2.Where(c => c.gameObject != gameObject).ToArray();
            collidersR2 = AddBlindArea(collidersR2);

            Collider[] collidersR3 = Physics.OverlapSphere(transform.position, r3, fishLayerMask);
            collidersR3 = collidersR3.Except(collidersR1).ToArray(); // remove r1 fish from r3 list
            collidersR3 = collidersR3.Except(collidersR2).ToArray(); // remove r2 fish from r3 list
            collidersR3 = collidersR3.Where(c => c.gameObject != gameObject).ToArray();
            collidersR3 = AddBlindArea(collidersR3);

            if(collidersR2.Length == 0 && collidersR3.Length != 0){
                newMoveDirection =  Equ_Attraction(collidersR3);
                fishBody.GetComponent<Renderer>().material = attractionMaterial;
            } 
            else if(collidersR2.Length != 0 && collidersR3.Length == 0){
                newMoveDirection =  Equ_Orientation(collidersR2);
                fishBody.GetComponent<Renderer>().material = orientationMaterial;
            }
            else{
                newMoveDirection = (Equ_Orientation(collidersR2) + Equ_Attraction(collidersR3))/2;
                fishBody.GetComponent<Renderer>().material = mixedMaterial;
            }
        }
        if(newMoveDirection != Vector3.zero){
            targetDirection = AddGaussianNoise(newMoveDirection, sigma);
        }
        else
        {
            //change matierial to normal
            fishBody.GetComponent<Renderer>().material = normalMaterial;
        }
        
    }
    Vector3 AddGaussianNoise(Vector3 newMoveDirection, float sigma){
        Vector3 perturbedAxis = GetGaussianPerturbedAxis(newMoveDirection.normalized,sigma);
        float angle = GaussianRandom(0f, sigma);
        Quaternion rotation = Quaternion.AngleAxis(angle, perturbedAxis); 
        return rotation * newMoveDirection.normalized;
    }
    Collider[] AddBlindArea(Collider[] colliders)
    {
        List<Collider> blindColliders = new List<Collider>();
        foreach (Collider c in colliders)
        {
            Vector3 diff = c.transform.position - transform.position;
            float angle = Vector3.Angle(moveDirection, diff);
            if (angle < blindAngle/2)
            {
                blindColliders.Add(c);
            }
        }
        return blindColliders.ToArray();
    }
    Vector3 Equ_Repulsion(Collider[] colliders)
    {
        Vector3 repulsion = Vector3.zero;
        foreach (Collider c in colliders)
        {
            Vector3 diff = transform.position - c.transform.position;
            repulsion += diff.normalized / diff.magnitude;
        }
        return repulsion;
    }
    Vector3 Equ_Orientation(Collider[] colliders)
    {
        Vector3 orientation = Vector3.zero;
        foreach (Collider c in colliders)
        { 
            Vector3 moveDir = c.GetComponent<FishBehavior>().moveDirection;
            orientation += moveDir/moveDir.magnitude;
        }
        return orientation;
    }
    Vector3 Equ_Attraction(Collider[] colliders)
    {
        Vector3 attraction = Vector3.zero;
        foreach (Collider c in colliders)
        {
            Vector3 diff = c.transform.position - transform.position;
            attraction += diff.normalized / diff.magnitude;
        }
        return attraction;
    }
    //generate a normal random rotation angle
    public float GaussianRandom(float miu, float sigma)
    {
        float u1 = 1.0f - Random.value;
        float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return miu + sigma * randStdNormal;
    }
    Vector3 GetGaussianPerturbedAxis(Vector3 reference, float stdDev)
    {
        // generate two random perturbation
        float perturbX = GaussianRandom(0f, stdDev);
        float perturbY = GaussianRandom(0f, stdDev);

        // find two vectors orthogonal to the reference vector
        Vector3 arbitrary = (Mathf.Abs(reference.x) > 0.9f) ? Vector3.up : Vector3.right;
        Vector3 tangent1 = Vector3.Cross(reference, arbitrary).normalized;
        Vector3 tangent2 = Vector3.Cross(reference, tangent1).normalized;

        // calculate the perturbed axis
        Vector3 perturbation = perturbX * tangent1 + perturbY * tangent2;
        Vector3 perturbedAxis = (reference + perturbation).normalized;

        return perturbedAxis;
    }
    void FixedUpdate()
    {
        float angle = Vector3.Angle(moveDirection,targetDirection);
        // if(angle<turningrate){
        //     moveDirection = targetDirection;
        // }else{
            float maxRadiansDelta = Mathf.Deg2Rad * turningrate * Time.deltaTime;

            moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, maxRadiansDelta, 0.0f);
        //}
        
        //move the prefab
        rb.MovePosition(rb.position + moveDirection.normalized * moveSpeed * Time.fixedDeltaTime);

        //orient the prefab in the direction of movement
        transform.rotation = Quaternion.LookRotation(moveDirection.normalized);
    }
}
