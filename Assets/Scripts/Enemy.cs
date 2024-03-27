using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHelath;
    public SpriteRenderer[] sprite;
    public bool isDead;
    public bool isEnter;
    public Rigidbody2D rigid;
    public GameObject MSword;
    void Awake(){
        sprite = GetComponentsInChildren<SpriteRenderer>();//MeshRenderer에서 material을 뽑아올 때는 소문자로 작성
        
    }
private void OnTriggerEnter2D(Collider2D other) {
    if(other.tag =="Hammer"){
        Debug.Log("해머인식");
        Player weapon = other.GetComponentInParent<Player>();
        if(weapon != null){
            curHelath -= weapon.Hammerdamgae;
            Debug.Log("데미지받음");
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec,false));
            Debug.Log("데미지받음");
        }

    }
    else if(other.tag =="Sword"){
        Debug.Log("해머인식");
        Player weapon = other.GetComponentInParent<Player>();
        if(weapon != null){
            curHelath -= weapon.Sworddamage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec,false));
        }
    } 
    else if(other.tag == "Bullet"){
            bullet bulletd = other.GetComponent<bullet>();
            if(bulletd != null){
                curHelath -= bulletd.bulletdamgae;
                Debug.Log("총알닿음?");
                Vector3 reactVec = transform.position - other.transform.position;
                MSword.SetActive(true);
                isEnter = true;
                Destroy(other.gameObject);

                StartCoroutine(OnDamage(reactVec,false));
                if(isEnter){
                    StartCoroutine("DestroySwd");
                }
            }
        }
        else{
            Debug.Log("아무도안맞음");
        }
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade){
        foreach(SpriteRenderer mesh in sprite)
            mesh.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHelath > 0){
            foreach(SpriteRenderer mesh in sprite)
                mesh.color = Color.white;
            reactVec = reactVec.normalized;
            rigid.AddForce(reactVec * 5, ForceMode2D.Impulse);
        }
        else{
            foreach(SpriteRenderer mesh in sprite)
                mesh.color = Color.gray;
            gameObject.layer = 12;
            isDead = true;
        }
    }
    IEnumerator DestroySwd(){
        yield return new WaitForSeconds(5f);
        MSword.SetActive(false);
    }
}