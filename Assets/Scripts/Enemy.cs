using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]private enum Type{Shiled, normal,Boss};
    [SerializeField]private Type enemyType;
    [SerializeField]private int maxHealth = 100;
    
    [SerializeField]private int maxShiled = 100;
    [SerializeField] HealthBarUI healthbar;
    [SerializeField] private int curShiled;
    [SerializeField]private int curHelath;
    [SerializeField]private SpriteRenderer[] sprite;
    [SerializeField]private GameObject particle;
    public bool isDead;
    public bool isEnter;
    public Rigidbody2D rigid;
    public GameObject MSword;
    Player player;
    void Start() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if(enemyType == Type.normal){
            particle.SetActive(false);
            curShiled = 0;
            healthbar.UpdateShieldBar(curShiled,maxShiled);
        }
        // if(enemyType == Type.Shiled){
        // }
        healthbar = GetComponentInChildren<HealthBarUI>();
        
    }
    void Awake(){
        sprite = GetComponentsInChildren<SpriteRenderer>();//MeshRenderer에서 material을 뽑아올 때는 소문자로 작성

    }
    void Update(){
        // ShiledMonster();
    }

    // void ShiledMonster(){
    //     if(enemyType == Type.Shiled){
    //         if(curShiled >= 0){
    //             if()
    //         }
    //     }
    // }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag =="Hammer"){
            if(player != null){
                
                Damged("Hammer");
                Vector3 reactVec = transform.position - other.transform.position;
                StartCoroutine(OnDamage(reactVec));
            }

        }
        else if(other.tag =="Sword"){
            if(player != null){
                Damged("Sword");
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
                    switch(enemyType){
                    case Type.normal:
                    curHelath -= bulletd.bulletdamgae;
                    break;
                    case Type.Shiled:
                    if(curShiled >= 0){
                        Debug.Log("!");
                        curShiled -= 1;
                    }
                    else{
                        Debug.Log("?");
                    curHelath -= bulletd.bulletdamgae;
                    }
                    break;
                }
                    // curHelath -= bulletd.bulletdamgae;
                    
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
                if(weapon.HammerSkill){
                    isEnter = true;
                }
                if(weapon.SwordSkill){
                    if(enemyType != Type.Boss){
                        Debug.Log("확인중");
                        isEnter = true;
                    }else
                        Debug.Log("보스거름");
                }
                Vector3 reactVec = transform.position - other.transform.position;
                if(weapon.arrowskill){
                    StartCoroutine(ArrowSkillDmg(reactVec));
                }
                StartCoroutine(OnDamage(reactVec));
            }
        } 
    }
    IEnumerator ArrowSkillDmg(Vector3 reactVec){
        StartCoroutine(OnDamage(reactVec));
        curHelath -= player.Sworddamage;
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(OnDamage(reactVec));
        curHelath -= player.Sworddamage;
        yield return new WaitForSeconds(0.1f);
        curHelath -= player.Sworddamage;
        yield return new WaitForSeconds(0.1f);
    }
    IEnumerator OnDamage(Vector3 reactVec){
        foreach(SpriteRenderer mesh in sprite)
            mesh.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        
        healthbar.UpdateHealthBar(curHelath,maxHealth);
        healthbar.UpdateShieldBar(curShiled,maxShiled);
        if(curHelath > 0){
            foreach(SpriteRenderer mesh in sprite){
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
        if(curShiled >= 0){
            
        curShiled -= damageAmount;
        }else{

        curHelath -= damageAmount;
        }
        
        StartCoroutine(OnDamage(Vector3.zero)); // OnDamage 함수를 호출하여 피해를 입힙니다.
    }

    public void Damged(string ani)
    {
        Debug.Log("0");
        if(ani == "Hammer"){
        Debug.Log("1");
            switch(enemyType){
                case Type.normal:
        Debug.Log("2");
                    curHelath -= player.Hammerdamgae;
                break;
                case Type.Shiled:
        Debug.Log("3");
                if(curShiled >= 0){
                    curShiled -= 40;
                }
                else{
                    curHelath -= player.Hammerdamgae;
                }
                break;
            }
        }
        else if(ani == "Sword"){
            switch(enemyType){
                case Type.normal:
                    curHelath -= player.Sworddamage + 3;
                break;
                case Type.Shiled:
                if(curShiled >= 0){
                    curShiled -=  10;
                }
                else{
                    curHelath -= player.Sworddamage;
                }
                break;
            }
        }
        
        StartCoroutine(OnDamage(Vector3.zero)); // OnDamage 함수를 호출하여 피해를 입힙니다.

    }
    public IEnumerator skillDmg()
    {
        yield return new WaitForSeconds(0.1f);

        TakeDamage(10);
    }
    void Die()
    {
        Destroy(gameObject);
    }
    
}