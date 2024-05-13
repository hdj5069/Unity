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
    public Collider2D enemycollider;
    void Start() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if(enemyType == Type.normal){
            particle.SetActive(false);
        }
    }
    void Awake(){
        sprite = GetComponentsInChildren<SpriteRenderer>();//MeshRenderer에서 material을 뽑아올 때는 소문자로 작성
        enemycollider = GetComponent<Collider2D>();
    }
    void Update(){
        
    }

    // private void OnTriggerStay2D(Collider2D other) {
    //     if(other.tag =="Skill"){
    //         Player weapon = other.GetComponentInParent<Player>();
    //         if(weapon != null){
    //                 Debug.Log("0.4");
    //             switch(enemyType){
    //                 case Type.normal:
    //                     curHelath -= weapon.Sworddamage;
    //                 break;
    //                 case Type.Shiled:
    //                     curHelath -= weapon.Sworddamage;
    //                 break;
    //             }
    //             if(weapon.HammerSkill){
    //                 isEnter = true;
    //             }
    //             if(weapon.SwordSkill){
    //                 isEnter = true;
    //             }
    //             Vector3 reactVec = transform.position - other.transform.position;
    //             if(weapon.arrowskill){
    //                 StartCoroutine(ArrowSkillDmg(reactVec));
    //                 Debug.Log(1);
    //             }
    //                 Debug.Log(reactVec);
    //             StartCoroutine(OnDamage(reactVec));
    //         }
    //     } 
    //     Debug.Log(other.name);
    // }

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
                    Debug.Log("0.4");
                switch(enemyType){
                    case Type.normal:
                        curHelath -= weapon.Sworddamage;
                    break;
                    case Type.Shiled:
                        curHelath -= weapon.Sworddamage;
                    break;
                }
                if(weapon.HammerSkill){
                    Debug.Log("0.5");
                    isEnter = true;
                }
                if(weapon.SwordSkill){
                    isEnter = true;
                }
                Vector3 reactVec = transform.position - other.transform.position;
                if(weapon.arrowskill){
                    StartCoroutine(ArrowSkillDmg(reactVec));
                    Debug.Log(1);
                }
                    Debug.Log(reactVec);
                StartCoroutine(OnDamage(reactVec));
            }
        } 
    }
    IEnumerator ArrowSkillDmg(Vector3 reactVec){
        StartCoroutine(OnDamage(reactVec));
        curHelath -= player.Sworddamage;
                    Debug.Log(2);
        yield return new WaitForSeconds(0.1f);
        curHelath -= player.Sworddamage;
        StartCoroutine(OnDamage(reactVec));
                    Debug.Log(3);
        yield return new WaitForSeconds(0.1f);
        curHelath -= player.Sworddamage;
        
        yield return new WaitForSeconds(0.1f);
                    Debug.Log(4);
    }
    IEnumerator OnDamage(Vector3 reactVec){
                    Debug.Log(5.1);
        foreach(SpriteRenderer mesh in sprite)
            mesh.color = Color.red;
                    Debug.Log(5.2);
        yield return new WaitForSeconds(0.1f);

        if(curHelath > 0){
                    Debug.Log(6.1);
            foreach(SpriteRenderer mesh in sprite){
                    Debug.Log(6.2);
                mesh.color = Color.white;
                reactVec = reactVec.normalized;
                rigid.AddForce(reactVec * 5, ForceMode2D.Impulse);
                Debug.Log(curHelath);
            }
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
    public IEnumerator skillDmg(int damageAmount)
    {
        yield return new WaitForSeconds(0.1f);

        TakeDamage(10);
        yield return new WaitForSeconds(0.1f);

        TakeDamage(10);
        yield return new WaitForSeconds(0.1f);

        TakeDamage(10);
    }
    void Die()
    {
        Destroy(gameObject);
    }
    
}