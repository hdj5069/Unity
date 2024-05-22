using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRockPool : MonoBehaviour
{
    [SerializeField]
    private GameObject BossRockPrefab;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();
    public static BossRockPool Instance{get;private set;}

    private void Awake() {
        Instance = this;
        GrowPool();
    }
    private void Update() {
        // BossRockPrefab.
    }
    private void GrowPool(){
        for(int i =0; i<10; i++){
            var instanceToAdd = Instantiate(BossRockPrefab);
            instanceToAdd.transform.localScale = new Vector3(1f ,1f,1f);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }
    public void AddToPool(GameObject instance){
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }
    public GameObject GetFromPool(){
        if(availableObjects.Count == 0){
            GrowPool();
        }
        var instance = availableObjects.Dequeue();
        instance.transform.localScale = new Vector3(1f ,1f,1f);
        instance.SetActive(true);
        return instance;
    }
}
