using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImg : MonoBehaviour
{
    [SerializeField]
    private float activetime = 0.1f;
    private float timeActivated;
    private float alpha;
    
    [SerializeField]
    private float alphaSet = 0.8f;
    private float alphaMultiplier = 0.98f; // 스프라이트 페이드 되는 속도 작을수록 빠름
    private Transform player;
    private SpriteRenderer SR;
    private SpriteRenderer playerSR;
    private Color color;
    private void OnEnable() {//객체가 활성화 되었을 때 사용 정확히 이 부분에 스크립트가 작동하면 시작되는 부분 PlayerAfterImgPool에서는 gameObject를 사용해서 불러옴
        SR = GetComponent<SpriteRenderer>();//자기자신에게 있는 spriteRenderer를 가져옴
        player = GameObject.FindGameObjectWithTag("Player").transform;//플레이어에 위치정보를 가져옴
        playerSR = player.GetComponent<SpriteRenderer>();//player에게 있는 spriteRenderer정보를 가져옴. 여기서 현재 플레이어의 모습을 가져와서 위에 SR에 저장후 불러내는 방식인듯

        alpha = alphaSet;//작동할 때 마다 항상 alpha값을 초기화해주는 부분
        SR.sprite = playerSR.sprite;//플레이어 이미지 정보를 SR에 저장
        transform.position = player.position;//이 스크립트를 가진 오브젝트의 위치를 플레이어의 위치와 동일하게 함.
        transform.rotation = player.rotation;// ↑동일
        timeActivated = Time.time;//현재 시간 정보를 가져옴
    }
    private void Update() {
        alpha *= alphaMultiplier;//update라서 매 프레임당 연산 시작 > 0.8 * 0.98 = 0.7.... > 0.7 * 0.98 > 0.6.... 
                                //이렇게 줄어들면서 alpha값이 작아짐 단 이렇게 하려면 alphamulti값이 1보다 작아야함 
        color = new Color(0,0,0,alpha);// 그렇게 적용된게 매 번 color에 저장되면서 작동
        SR.color = color;//플레이어의 행동의 나온 이미지의 alpha값이 작아지면서 흐려짐
        if(Time.time >= (timeActivated + activetime)){//시간계산하는 부분 
            PlayerAfterImgPool.Instance.AddToPool(gameObject);//싱글톤 작동 plyaerAfterImgPool로
        }
    }
}
