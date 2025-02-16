 
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using System.Linq;

public struct FishBehaviorJob : IJobParallelFor
{
    public NativeArray<Fish> fishes;
    public float moveSpeed;
    public float turningrate; // degree per second
    public float blindAngle; //360 - alpha
    // public GameObject fishBody;
    // public Material normalMaterial;
    // public Material repulsionMaterial;
    // public Material orientationMaterial;
    // public Material attractionMaterial;
    // public Material mixedMaterial;
    public float r1; // zone of repulsion
    public float r2; // zone of orientation
    public float r3; // zone of attraction
    //Rigidbody rb;
    public int fishLayerMask;
    public float deltaTime;
    public FishBehaviorJob(NativeArray<Fish> fishes, float deltaTime){
        this.fishes = fishes;
        this.deltaTime = deltaTime;
        moveSpeed = 1f;
        turningrate = 10f;
        blindAngle = 90f;
        r1 = 1f;
        r2 = 15f;
        r3 = 30f;
        fishLayerMask = 64;
    }
    public void Execute(int index){
        Fish fish = fishes[index];
    }

    // public void SwimBehavior(GameObject fish)
    // {
    //    // check for nearby fish within r1, r2, and r3
    //     Collider[] collidersR1 = Physics.OverlapSphere(fish.transform.position, r1, fishLayerMask); 
    //     collidersR1 = collidersR1.Where(c => c.gameObject != fish.gameObject).ToArray();
    //     Vector3 newMoveDirection = Vector3.zero;
    //     if(collidersR1.Length > 0)
    //     {
    //         collidersR1 = AddBlindArea(collidersR1);
    //         newMoveDirection = Equ_Repulsion(collidersR1); 
    //     }else{
    //         Collider[] collidersR2 = Physics.OverlapSphere(transform.position, r2, fishLayerMask);
    //         collidersR2 = collidersR2.Except(collidersR1).ToArray(); // remove r1 fish from r2 list
    //         collidersR2 = collidersR2.Where(c => c.gameObject != gameObject).ToArray();
    //         collidersR2 = AddBlindArea(collidersR2);

    //         Collider[] collidersR3 = Physics.OverlapSphere(transform.position, r3, fishLayerMask);
    //         collidersR3 = collidersR3.Except(collidersR1).ToArray(); // remove r1 fish from r3 list
    //         collidersR3 = collidersR3.Except(collidersR2).ToArray(); // remove r2 fish from r3 list
    //         collidersR3 = collidersR3.Where(c => c.gameObject != gameObject).ToArray();
    //         collidersR3 = AddBlindArea(collidersR3);

    //         if(collidersR2.Length == 0 && collidersR3.Length != 0){
    //             newMoveDirection =  Equ_Attraction(collidersR3);
    //             fishBody.GetComponent<Renderer>().material = attractionMaterial;
    //         } 
    //         else if(collidersR2.Length != 0 && collidersR3.Length == 0){
    //             newMoveDirection =  Equ_Orientation(collidersR2);
    //             fishBody.GetComponent<Renderer>().material = orientationMaterial;
    //         }
    //         else{
    //             newMoveDirection = (Equ_Orientation(collidersR2) + Equ_Attraction(collidersR3))/2;
    //             fishBody.GetComponent<Renderer>().material = mixedMaterial;
    //         }
    //     }
    //     if(newMoveDirection != Vector3.zero){
    //         targetDirection = newMoveDirection.normalized;
    //     }
    //     else
    //     {
    //         //change matierial to normal
    //         fishBody.GetComponent<Renderer>().material = normalMaterial;
    //     }
        
    // }
    // Collider[] AddBlindArea(Collider[] colliders, Fish fish, GameObject fish_obj)
    // {
    //     List<Collider> blindColliders = new List<Collider>();
    //     foreach (Collider c in colliders)
    //     {
    //         Vector3 diff = c.transform.position - fish_obj.transform.position;
    //         float angle = Vector3.Angle(fish.moveDirection, diff);
    //         if (angle < blindAngle/2)
    //         {
    //             blindColliders.Add(c);
    //         }
    //     }
    //     return blindColliders.ToArray();
    // }
    // Vector3 Equ_Repulsion(Collider[] colliders, GameObject fish_obj)
    // {
    //     Vector3 repulsion = Vector3.zero;
    //     foreach (Collider c in colliders)
    //     {
    //         Vector3 diff = fish_obj.transform.position - c.transform.position;
    //         repulsion += diff.normalized / diff.magnitude;
    //     }
    //     return repulsion;
    // }
    // Vector3 Equ_Orientation(Collider[] colliders)
    // {
    //     Vector3 orientation = Vector3.zero;
    //     foreach (Collider c in colliders)
    //     { 
    //         Vector3 moveDir = c.GetComponent<FishBehavior>().moveDirection;
    //         orientation += moveDir/moveDir.magnitude;
    //     }
    //     return orientation;
    // }
    // Vector3 Equ_Attraction(Collider[] colliders)
    // {
    //     Vector3 attraction = Vector3.zero;
    //     foreach (Collider c in colliders)
    //     {
    //         Vector3 diff = c.transform.position - transform.position;
    //         attraction += diff.normalized / diff.magnitude;
    //     }
    //     return attraction;
    // }
}
