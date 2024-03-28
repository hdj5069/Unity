using System.Collections;
using System.Collections.Generic;
// using System.Diagnostics;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum Type{Shiled, normal};
    public Type enemyType;
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
        Player weapon = other.GetComponentInParent<Player>();
        if(weapon != null){
            switch(enemyType){
                case Type.normal:
                    curHelath -= weapon.Hammerdamgae;
                break;
                case Type.Shiled:
                    curHelath -= weapon.Hammerdamgae + 5;
                break;
        }
            Debug.Log("데미지받음");
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec));
            Debug.Log("데미지받음");
        }

    }
    else if(other.tag =="Sword"){
        Player weapon = other.GetComponentInParent<Player>();
        if(weapon != null){
            switch(enemyType){
                case Type.normal:
                    curHelath -= weapon.Sworddamage + 3;
                break;
                case Type.Shiled:
                    curHelath -= weapon.Sworddamage;
                break;
        }
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec));
        }

    } 
    else if(other.tag == "Bullet"){
            bullet bulletd = other.GetComponent<bullet>();
            if(bulletd != null){
                if(bulletd.type == bullet.Type.Arrow){
                    curHelath -= bulletd.bulletdamgae;
                    Debug.Log("총알닿음2?");
                }
                else if(bulletd.type == bullet.Type.Skill){
                    curHelath -= bulletd.bulletdamgae;
                    Debug.Log("총알닿음1?");
                    Vector3 reactVec = transform.position - other.transform.position;
                    MSword.SetActive(true);
                    isEnter = true;
                    Destroy(other.gameObject);

                    StartCoroutine(OnDamage(reactVec));
                    if(isEnter){
                        StartCoroutine("DestroySwd");
                        Debug.Log("dddd");
                    }
                }
            }
        }
    }

    IEnumerator OnDamage(Vector3 reactVec){
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
        yield return new WaitForSeconds(0.5f);
        isEnter = false;
        yield return new WaitForSeconds(5f);
        MSword.SetActive(false);
    }
}