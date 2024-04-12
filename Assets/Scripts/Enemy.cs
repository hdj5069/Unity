using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum Type{Shiled, normal};
    public Type enemyType;
    public int maxHealth = 100;
    public int curHelath;
    public SpriteRenderer[] sprite;
    public GameObject particle;
    public bool isDead;
    public bool isEnter;
    public Rigidbody2D rigid;
    public GameObject MSword;
    Player player;
    void Awake(){
        sprite = GetComponentsInChildren<SpriteRenderer>();//MeshRenderer에서 material을 뽑아올 때는 소문자로 작성

    }
    void Update(){
        if(enemyType == Type.normal){
            particle.SetActive(false);
        }
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
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec));
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
                if(bulletd.type == bullet.Type.Skill){
                    isEnter = true;
                    curHelath -= bulletd.bulletdamgae;
                    Vector3 reactVec = transform.position - other.transform.position;

                    StartCoroutine(OnDamage(reactVec));
                    Destroy(other.gameObject);
                    StartCoroutine("DestroySwd");
                }else if(bulletd.type == bullet.Type.Arrow){
                    curHelath -= bulletd.bulletdamgae;
                    
                    Vector3 reactVec = transform.position - other.transform.position;
                    StartCoroutine(OnDamage(reactVec));
                }
            }
        }
        else if(other.tag =="Skill"){
            Player weapon = other.GetComponentInParent<Player>();
            
            if(weapon != null){
                switch(enemyType){
                    case Type.normal:
                        curHelath -= weapon.Sworddamage;
                    break;
                    case Type.Shiled:
                        curHelath -= weapon.Sworddamage;
                    break;
                }
                if(weapon.isHammer){
                    Debug.Log("확인");
                    isEnter = true;
                    StartCoroutine("isEnterdel");
                }
                Vector3 reactVec = transform.position - other.transform.position;
                    
                StartCoroutine(OnDamage(reactVec));
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
            Die();
        }
    }
    
    public void TakeDamage(int damageAmount)
    {
        curHelath -= damageAmount;
        
        StartCoroutine(OnDamage(Vector3.zero)); // OnDamage 함수를 호출하여 피해를 입힙니다.
    }
    void Die()
    {
        Destroy(gameObject);
    }
    IEnumerator DestroySwd(){
        yield return new WaitForSeconds(2.5f);
        isEnter = false;
        MSword.SetActive(false);
    }
    IEnumerator isEnterdel(){
        yield return new WaitForSeconds(2.5f);
        isEnter = false;
    }
}