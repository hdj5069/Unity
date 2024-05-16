using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerAfterImgPool : MonoBehaviour
{
    [SerializeField]
    private GameObject afterImagePrefab;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();//내가 만든 객체 저장
    public static PlayerAfterImgPool Instance{get;private set;}//싱글톤 생성
    Player player;
    float direction;

    private void Awake() {
        Instance = this;
        // GrowPool();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

    }
    private void Update() {
        UpdateDirection();
        // GrowPool();
    }
    private void GrowPool(){
        for(int i =0; i<10; i++){
            var instanceToAdd = Instantiate(afterImagePrefab);
            // float direction = player.transform.localScale.x > 0 ? -1 : 1;
            
            // direction = player.transform.localScale.x > 0 ? -1 : 1;
            Debug.Log(direction);
            Debug.Log(player.transform.localScale.x);

            instanceToAdd.transform.localScale = new Vector3(5f * direction,5f,1f);
            instanceToAdd.transform.SetParent(transform);
            Debug.Log("?");
            AddToPool(instanceToAdd);
        }
    }
    public void AddToPool(GameObject instance){
        instance.SetActive(false);
        availableObjects.Enqueue(instance);//큐에있는 instance를 
    }
    public GameObject GetFromPool(){
        if(availableObjects.Count == 0){
            GrowPool();
        }
        var instance = availableObjects.Dequeue();
        instance.transform.localScale = new Vector3(5f * direction,5f,1f);
        instance.SetActive(true);
        return instance;
    }
     private void UpdateDirection()
    {
        direction = player.transform.localScale.x > 0 ? 1 : -1;
        Debug.Log(direction);
    }
}
