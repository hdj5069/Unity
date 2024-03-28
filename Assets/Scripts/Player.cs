using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour{
    public float maxSpeed;
    public float jumpPower;
    public GameObject Sword;
    public GameObject Hammer;
    public int Hammerdamgae;
    public int Sworddamage;
    public int bulletdamgae;
    public GameObject bulletObj;
    public GameObject SkillObj;
    public BoxCollider2D jumphammer;
    public BoxCollider2D jumpSword;
    public BoxCollider2D HammerCollider;
    public BoxCollider2D SwordCollider;
    public ParticleSystem Hammerparticle; 
    float originalspeed;
    float tempspeed = 100;
    bool isSkill;
    bool skillCol;
    bool HammerCool;
    bool SwordCool;
    bool isMove;
    bool isJump;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    Animator anim;
    Enemy enemy;
    void Awake() {
        rigid = GetComponent<Rigidbody2D>(); 
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();  
        anim = GetComponent<Animator>();
        enemy = GameObject.FindWithTag("Enemy").GetComponent<Enemy>();
    }
    void Update() {
        if(Input.GetButtonDown("Vertical") && !isJump){
            isJump =true;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

        }
        if(Input.GetButtonUp("Horizontal")){
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f,rigid.velocity.y);
        }
        if(Input.GetButton("Horizontal")){
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;//움직이는 값이 -일 때 flipx를 바꿔라
        }
        Skill();
        AtkArrow();
        AtkHammer();
        AtkSword();

    }

    void FixedUpdate() {
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if(rigid.velocity.x > maxSpeed){
            rigid.velocity = new Vector2(maxSpeed,rigid.velocity.y);
        }else if(rigid.velocity.x < maxSpeed * (-1)){ //left maxspeed계산을 위해 maxSpeed가 음수로 받게함
            rigid.velocity = new Vector2(maxSpeed*(-1),rigid.velocity.y);
        }
        // bool isGrounded = CheckGrounded();
        if(checkGrounded()){
            isJump = false;
        }
        
    }

    void Skill(){
        if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)){
            if(!skillCol){
            Debug.Log("A와 S키가 동시에 눌렀습니다.");
            isSkill = true;
            skillCol = true;
            CreateBullet();
            }
        }
    }
    void AtkArrow(){
        if(Input.GetButtonDown("AtkArrow")&&(!SwordCool)&&(!HammerCool)){
            if(isSkill){

                Sword.SetActive(false);
                Hammer.SetActive(false);
                CreateBullet();
                AtkCool("Arrow");
            }
        }
    }
    void AtkSword(){
        if(Input.GetButtonDown("AtkSword")&&(isSkill)&&(!SwordCool)&&(!HammerCool)){
            Sword.SetActive(true);
            Hammer.SetActive(false);
            if(isJump){
                anim.SetBool("isJump",true);
                anim.SetBool("Sword",true);
                jumpSword.enabled = true;
            }
            if(!isJump){
                anim.SetTrigger("doSWD");
                SwordCollider.enabled =true;
            }
            AtkCool("Sword");
        }
    }
    
    void AtkHammer(){
        if(Input.GetButtonDown("AtkHammer")&&(isSkill)&&(!HammerCool)&&(!SwordCool)){
            Sword.SetActive(false);
            Hammer.SetActive(true);
            if(isJump){
                anim.SetBool("isJump",true);
                anim.SetBool("Hammer",true);
            }
            if(!isJump){
                HammerCollider.enabled = true;
                anim.SetTrigger("doHAM");
            }
            AtkCool("Hammer");
        }
    }
    void AtkCool(string AtkEnum){
        if(AtkEnum == "Hammer"){
            HammerCool = true;
            StartCoroutine("HammerCooldown");   
        }
        else if(AtkEnum == "Sword"){
            SwordCool = true;
            StartCoroutine("SwordCooldown");
        }
        else if(AtkEnum == "Arrow"){
            SwordCool = true;
            StartCoroutine("SwordCooldown");
        }
    }
    IEnumerator HammerCooldown(){
        if(anim.GetBool("isJump")){
            yield return new WaitForSeconds(0.2f);
            jumphammer.enabled = true;
            yield return StartCoroutine(GroundCheck());
        }
        else{
            yield return new WaitForSeconds(0.2f);
            HammerCollider.enabled = false;
        }
        yield return new WaitForSeconds(0.5f);
        HammerCool = false;
    }
    IEnumerator SwordCooldown(){
        if(anim.GetBool("isJump")){
            yield return StartCoroutine(GroundCheck());
        }
        else{
            yield return new WaitForSeconds(0.4f);
            SwordCollider.enabled = false;
        }
        SwordCool = false;
    }
    void CreateBullet()
    {
        if(isSkill){
            Vector3 bulletPosition = transform.position + transform.up * 1f;
            Quaternion bulletRotation = Quaternion.Euler(0f,0f,-80f);
            
            GameObject bullet = Instantiate(SkillObj, bulletPosition, bulletRotation);

            Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            if(bulletRigidbody != null){
                bulletRigidbody.AddForce(transform.right * 20, ForceMode2D.Impulse);
            }
            isSkill = false;
            StartCoroutine("ResetFiring");
        
            Destroy(bullet,5);
        }
        else if(!isSkill){
            Vector3 bulletPosition = transform.position + transform.up * 0.3f;
            GameObject bullet = Instantiate(bulletObj,bulletPosition,Quaternion.identity);
            Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            if(bulletRigidbody != null){
                bulletRigidbody.AddForce(transform.right * 20, ForceMode2D.Impulse);
            }
        }
    }
    IEnumerator ResetFiring(){
        yield return new WaitForSeconds(0.5f);
        // yield return StartCoroutine(SkillCheck());
        Debug.Log("외부?");  
        if (enemy != null && enemy.isEnter){
            
            Debug.Log("내부?");  
            Debug.Log(enemy.isEnter);
            originalspeed = maxSpeed;
            maxSpeed = tempspeed;
            Transform enemyTransform = GameObject.FindWithTag("Enemy").transform;
            Debug.Log("이동");
            Vector2 directionToEnemy = (enemyTransform.position - transform.position).normalized;
            directionToEnemy.y = 0;
            rigid.velocity = directionToEnemy * 15; 
            Debug.Log("이동2");
            Debug.Log(directionToEnemy+":diretenemy");
            
            yield return new WaitForSeconds(1f);
            maxSpeed = originalspeed;
        }
        else
        {
            Debug.Log(enemy);
            Debug.Log(enemy.isEnter);
            Debug.Log("No enemy or enemy is not in range.");
        }
        yield return new WaitForSeconds(3f);
        skillCol = false;
    }
    bool checkGrounded() {
        Debug.DrawRay(rigid.position, Vector2.down, new Color(0, 1, 0));
        Vector2 raycastStart = rigid.position - Vector2.up * 1f;
        RaycastHit2D rayHit = Physics2D.Raycast(raycastStart, Vector2.down, 1, LayerMask.GetMask("Floor"));
        if(rigid.velocity.y < 0){
            if (rayHit.collider != null && rayHit.distance < 0.3f) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }
    IEnumerator GroundCheck(){
        while(!checkGrounded()){
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        Debug.Log("지면 도착");
        if(jumphammer.enabled == true){
            // var emission = Hammerparticle.enableEmission;
            // emission.enabled = true;
            Hammerparticle.enableEmission = true;
        }
        anim.SetBool("Sword",false);
        anim.SetBool("Hammer",false);
        jumphammer.enabled = false;
        
        SwordCollider.enabled =false;
        yield return new WaitForSeconds(0.3f);
        anim.SetBool("isJump",false);
        Hammerparticle.enableEmission = false;
    }
    // IEnumerator SkillCheck(){
    //     while(!enemy.isEnter){
    //         yield return null;
    //     }
    //     yield return new WaitForSeconds(0.1f);
        
    // }
}
