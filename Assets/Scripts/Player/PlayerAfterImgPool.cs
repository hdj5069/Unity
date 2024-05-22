using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerAfterImgPool : MonoBehaviour
{
    [SerializeField]
    private GameObject afterImagePrefab;//PlayerAfterImg 스크립트를 가지고있는 프리팹

    private Queue<GameObject> availableObjects = new Queue<GameObject>();//내가 만든 객체 저장
    //Queue는 C# 에서 사용하는 자료구조로 GameObject를 저장함. 유니티에서 엔진에서 흔히 사용되는 패턴 : 오브젝트 폴링이라는 기법과 관련됨.
    //Queue First in, First Out 방식 > 선입선출 형태 윈도우 플밍에서 배우는 큐라는 개념과 동일한거 같음. 
    //오브젝트 폴링 개념 : 생성, 제거를 반복해야하는 일에는 유니티 내부에서 용량을 많이 사용해서 오브젝트에 SetActive를 false true로 변경시키면서 식별만 on, off하는 방식
    public static PlayerAfterImgPool Instance{get;private set;}//싱글톤 생성 PlayerAfterImgPool에 내용을 Instance로 변경 가능
    Player player;
    float direction;

    private void Awake() {
        Instance = this;
        GrowPool();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    private void Update() {
        UpdateDirection();
    }
    private void GrowPool(){//최초로 플레이어 이미지를 이 스크립트가 있는 오브젝트에 저장하는 부분
        for(int i =0; i<10; i++){
            var instanceToAdd = Instantiate(afterImagePrefab);//PlayerAfterImg에서 저장하는 플레이어의 이미지를 소환함.
            instanceToAdd.transform.localScale = new Vector3(1f * direction,1f,1f);
            instanceToAdd.transform.SetParent(transform);//이 스크립트가 있는 오브젝트의 하위오브젝트로 플레이어의 이미지들을 저장
            AddToPool(instanceToAdd);
        }
    }
    public void AddToPool(GameObject instance){//폴에 저장함 이미지는 안보이게함
        instance.SetActive(false);//이미지를 안보이게 처리함
        availableObjects.Enqueue(instance);//큐에있는 instance를 추가
    }
    public GameObject GetFromPool(){
        if(availableObjects.Count == 0){
            GrowPool();//큐에 저장된 값이 없으면 grwoPool을 작동하게함
        }
        var instance = availableObjects.Dequeue();// 큐에 저장된 첫번 째 부분을 꺼내 사용
        instance.transform.localScale = new Vector3(1f * direction,1f,1f);
        instance.SetActive(true);//객체 활성화
        return instance;//gameobject를 return해서 GetFromPool호출한곳에서 바로 instance를 가져감
    }
     private void UpdateDirection()
    {
        direction = player.transform.localScale.x > 0 ? 1 : -1; // 현재 플레이어가 바라보는 방향을 확인 update에서 매번 확인
    }
}
