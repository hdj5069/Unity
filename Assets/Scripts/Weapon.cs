using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }//Melee부딫히다란 뜻
    public Type type;
    public int damgae;
    public BoxCollider2D meleeArea;//공격범위

    public void Use(){
        if(type == Type.Melee){
            StopCoroutine("Swing");//실행되는 코루틴 정지
            StartCoroutine("Swing");//코루틴 함수 클래스 이름 String상태로 작성
        }
    }
    IEnumerator Swing(){//무기 공격을 코루틴 사용하기 위해 열거형 함수 클래스 void대신 사용
        yield return new WaitForSeconds(0.4f);
        meleeArea.enabled = true;


        yield return new  WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);

        //boxCollider,TrailRenderer를 시간차로 활성화 컨트롤 가능 invoke로 여러번 사용할 꺼를 이렇게 사용해 줄임
    }
}