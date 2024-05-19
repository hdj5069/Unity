using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]private enum Type{melee,ranger,elite,Boss};
    [SerializeField]private Type enemyType;
    [SerializeField]private int maxHealth = 100;
    [SerializeField]private GameObject ArrowObj;
    public int ArrowDMG = 10;
    
    [SerializeField]private int curHelath;
    [SerializeField]private int maxShiled = 100;
    [SerializeField] private int curShiled;
    [SerializeField] HealthBarUI healthbar;
    [SerializeField]private SpriteRenderer[] sprite;
    // [SerializeField]private Transform player;
    [SerializeField]private float speed;
    public bool isDead;
    public bool isEnter;
    public Rigidbody2D rigid;
    bool chasingPlayer;
    Player player;
    Vector2 movement;
    [SerializeField]private GameObject particle;
    SpriteRenderer spriteRenderer;
    // Animator anim;
    BoxCollider2D boxCollider;
    CapsuleCollider2D capsulechase;
    public int nextMove;
    [SerializeField]private Detector playerchase;
    [SerializeField]private Detector attackpoint;
    [SerializeField]private Detector rangeATK = null;
    private void OnEnable() {
        playerchase.OnTriggerEnter2DEvent.AddListener(enterplayer);
        attackpoint.OnTriggerEnter2DEvent.AddListener(attacktar);
        if(enemyType == Type.ranger)
            rangeATK.OnTriggerEnter2DEvent.AddListener(rangeMob);
    }
    void Start() {
        capsulechase = playerchase.GetComponent<CapsuleCollider2D>();

        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if(enemyType != Type.elite){
            particle.SetActive(false);
            curShiled = 0;
            healthbar.UpdateShieldBar(curShiled,maxShiled);
        }
        
        healthbar = GetComponentInChildren<HealthBarUI>();
        
    }
    void Awake(){
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponentsInChildren<SpriteRenderer>();//MeshRenderer에서 material을 뽑아올 때는 소문자로 작성
        // anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Think();

        Invoke("Think",5);
    }
    void Update(){
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        movement = direction;
        
    }
    void FixedUpdate(){
        if(nextMove > 0){
            transform.localScale = new Vector3(1,1,1);
        }else if(nextMove < 0){
            transform.localScale = new Vector3(-1,1,1);
        }
        else{
        }
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f,rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0,1,0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec,Vector3.down,2,LayerMask.GetMask("Floor")|LayerMask.GetMask("Wall"));

        if(rayHit.collider == null){

            chasingPlayer = false;
            
            nextMove = nextMove * -1;
            Invoke("Think",5f);
            
        }
        if(rayHit.collider!=null&&rayHit.collider.tag == "Wall"){
            nextMove = nextMove * -1;
        }
        if(!chasingPlayer){
            rigid.velocity = new Vector2(nextMove, rigid.velocity.y);
        }else if(chasingPlayer){
            if(movement.x > 0){
                transform.localScale = new Vector3(1,1,1);
            }
            else if(movement.x < 0){
                transform.localScale = new Vector3(-1,1,1);
            }
            MoveCharacter(movement);
        }

    }
    void enterplayer(Collider2D collider){
        if (collider.CompareTag("Player")) {
            chasingPlayer = true;
            CancelInvoke();
        }
    }
    void Think(){
        nextMove = Random.Range(-1,2);
        float nextThinkTime = Random.Range(3f,6f);
        Invoke("Think",nextThinkTime);
        // Think();//재귀함수 자기자신을 불러서 다시 작동하게 함 하지만 딜레이가 없이 작동하게하면 위험함
    }
    void rangeMob(Collider2D other){
        if(other.tag == "Player"){
            if(enemyType == Type.ranger&&chasingPlayer){
                Vector3 bulletPosition = transform.position + transform.up * 0.3f;
                GameObject bullet = Instantiate(ArrowObj,bulletPosition,Quaternion.identity);
                Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
                
                if(bulletRigidbody != null){
                    if(transform.localScale.x < 0){
                        bulletRigidbody.AddForce(transform.right * -10, ForceMode2D.Impulse);
                        bullet.transform.localScale = new Vector3(-1, 1, 1);
                    }
                    else{
                        bulletRigidbody.AddForce(transform.right * 10, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }
    // void frontback(){
    //     if(transform.localScale.x > 0){
    //         capsulechase
    //     }
    // }
    void attacktar(Collider2D other){
        if(other.tag =="Hammer"){
            if(player != null){
                switch(enemyType){
                    case Type.melee:
                    case Type.ranger:
                        curHelath -= player.Hammerdamgae;
                    break;
                    case Type.elite:
                        if(curShiled >= 0){
                            curShiled -= 40;
                        }
                        else{
                            curHelath -= player.Hammerdamgae;
                        }
                    break;
                }
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
                    case Type.melee:
                    case Type.ranger:
                        curHelath -= bulletd.bulletdamgae;
                    break;
                    case Type.elite:
                        if(curShiled >= 0){
                            curShiled -= 1;
                        }
                        else{
                        curHelath -= bulletd.bulletdamgae;
                        }
                    break;
                    }
                    Vector3 reactVec = transform.position - other.transform.position;
                    StartCoroutine(OnDamage(reactVec));
                }
            }
        }
        else if(other.tag =="Skill"){
            Player weapon = other.GetComponentInParent<Player>();
            if(weapon != null){
                switch(enemyType){
                    case Type.melee:
                    case Type.ranger:
                        curHelath -= weapon.Sworddamage;
                    break;
                    case Type.elite:
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
                    }else{
                        Debug.Log("보스거름");
                    }
                }
                Vector3 reactVec = transform.position - other.transform.position;
                if(weapon.arrowskill){
                    StartCoroutine(ArrowSkillDmg(reactVec));
                }
                StartCoroutine(OnDamage(reactVec));
            }
        } 
    }

    void MoveCharacter(Vector2 direction){
        float newY = transform.position.y;
        float newX = transform.position.x + direction.x *speed * Time.deltaTime;
        Vector2 newPos = new Vector2(newX,newY);
        rigid.MovePosition(newPos);
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
        if(ani == "Hammer"){
            switch(enemyType){
                    case Type.melee:
                    case Type.ranger:
                    curHelath -= player.Hammerdamgae;
                break;
                case Type.elite:
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
                    case Type.melee:
                    case Type.ranger:
                    curHelath -= player.Sworddamage + 3;
                break;
                case Type.elite:
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