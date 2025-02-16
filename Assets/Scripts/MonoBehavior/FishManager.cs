using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public int num_fish = 10;
    public int wall_size = 20;
    List<FishBehavior> allFish;
    int index = 0;
    public GameObject fishPrefab;
    public float updateInterval = 0.1f;
    void Start()
    {
        allFish = new List<FishBehavior>(num_fish);
        CreateFish();
        StartCoroutine(UpdateFishInBatches());
    }
    void CreateFish()
    {
        for (int i = 0; i < num_fish; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-wall_size/2, wall_size/2),
            Random.Range(-wall_size/2, wall_size/2),
            Random.Range(-wall_size/2, wall_size/2));
            GameObject fish = Instantiate(fishPrefab,
            pos, Quaternion.identity) as GameObject;
            allFish.Add(fish.GetComponent<FishBehavior>());
        }
    }
    IEnumerator UpdateFishInBatches()
    {
        while (true)
        {
            if (allFish.Count > 0)
            {
                allFish[index].SwimBehavior();
                index = (index + 1) % allFish.Count; // Cycle through fish
            }
            yield return new WaitForSeconds(updateInterval / allFish.Count); // Spread updates
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
