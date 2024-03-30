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
    public GameObject ASskill;
    public int Hammerdamgae;
    public int Sworddamage;
    public int bulletdamgae;
    public GameObject ArrowObj;
    public GameObject perenArrow;
    public GameObject SkillObj;
    public BoxCollider2D jumphammer;
    public BoxCollider2D jumpSword;
    public BoxCollider2D HammerCollider;
    public BoxCollider2D SwordCollider;
    public ParticleSystem Hammerparticle; 
    public List<Enemy> enemies = new List<Enemy>();
    bool isSkill;
    bool skillCol1;
    bool isSkill2;
    bool skillCol2;
    public LayerMask obstacleLayer;
    public float curveDuration = 5f;
    public LayerMask floorLayer;
    bool HammerCool;
    bool SwordCool;
    bool isMove;
    bool isJump;
    bool isArrow;
    public int a;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    Animator anim;
    void Awake() {
        
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in allEnemies) {
            enemies.Add(enemy);
        }
        rigid = GetComponent<Rigidbody2D>(); 
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();  
        anim = GetComponent<Animator>();
    }
    void Update() {
        if(!isMove){
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

        }
        Skill1();
        Skill2();
        AtkArrow();
        AtkHammer();
        AtkSword();

    }

    void FixedUpdate() {
        if(!isMove){
            float h = Input.GetAxisRaw("Horizontal");

            rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        }
        if(rigid.velocity.x > maxSpeed){
            rigid.velocity = new Vector2(maxSpeed,rigid.velocity.y);
        }else if(rigid.velocity.x < maxSpeed * (-1)){ //left maxspeed계산을 위해 maxSpeed가 음수로 받게함
            rigid.velocity = new Vector2(maxSpeed*(-1),rigid.velocity.y);
        }
        if(checkGrounded()){
            isJump = false;
        }

    }

    void Skill1(){
        if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)){
            if(!skillCol1 && !isArrow && !SwordCool && !HammerCool){
                Debug.Log("A와 S키가 동시에 눌렀습니다.");
                isSkill = true;
                skillCol1 = true;
                StartCoroutine("Charge");
            }
        }
    }
    void Skill2(){
        if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)){
            if(!skillCol2 && !isArrow && !SwordCool && !HammerCool){
                Debug.Log("A와 D키가 동시에 눌렀습니다.");
                isSkill = true;
                skillCol2 = true;
                StartCoroutine("ActionSkill2");
            }
        }
    }
    
    void AtkSword(){
        if(Input.GetButtonUp("AtkSword")&& !isSkill && !SwordCool && !HammerCool && !isArrow){
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
        if(Input.GetButtonUp("AtkHammer")&&!isSkill&&!HammerCool&&!SwordCool && !isArrow){
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
    void AtkArrow(){
        if(Input.GetButtonDown("AtkArrow")&& !SwordCool && !HammerCool && !isArrow&& !isSkill){
            Sword.SetActive(false);
            Hammer.SetActive(false);
            CreateBullet();
            AtkCool("Arrow");
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
            isArrow = true;
            StartCoroutine("ArrowCooldown");
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
    IEnumerator ArrowCooldown(){
        yield return new WaitForSeconds(0.5f);
        isArrow = false;
    }
    IEnumerator Charge(){
        Sword.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        ASskill.SetActive(true);
        StartCoroutine("ResetSkillCool");
    }
    IEnumerator ActionSkill2(){
        Sword.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("doSkill2");
        Hammer.SetActive(true);
        HammerCollider.enabled = true;
        
        Vector2 newTarget = ModifyPath();
        StartCoroutine("ResetFiring");
    }

    void CreateBullet()
    {
        if(isSkill){

            Vector3 bulletPosition = transform.position + transform.up * 1f;
            Quaternion bulletRotation = Quaternion.Euler(0f,0f,-90f);
            
            GameObject bullet = Instantiate(SkillObj, bulletPosition, bulletRotation);

            Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            if(bulletRigidbody != null){
                bulletRigidbody.AddForce(transform.right * 20, ForceMode2D.Impulse);
            }
            StartCoroutine("ResetFiring");
            Destroy(bullet,5);
        }

    
        else if(!isSkill){
            Vector3 bulletPosition = transform.position + transform.up * 0.3f;
            GameObject bullet = Instantiate(ArrowObj,bulletPosition,Quaternion.identity);
            Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            if(bulletRigidbody != null){
                bulletRigidbody.AddForce(transform.right * 20, ForceMode2D.Impulse);
            }
        }
        
    }
    IEnumerator ResetSkillCool(){
        isSkill = false;
        isSkill2 = false;
        yield return new WaitForSeconds(2.5f);
        skillCol1 = false;
        skillCol2 = false;
        ASskill.SetActive(false);
        
    }
    IEnumerator ResetFiring(){
        yield return new WaitForSeconds(0.3f);
        isSkill2 = false;
        foreach (Enemy enemy in enemies)
        {
            StartCoroutine(SkillCheck(enemy));
        }
        yield return new WaitUntil(() => CheckEnter());
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.isEnter){
                Vector2 newTarget = ModifyPath();
                Debug.Log(enemy);
                StartCoroutine(MoveEnemy(enemy.transform));
            }
        }
        
        StartCoroutine("ResetSkillCool");
    }
    Vector2 ModifyPath()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, Mathf.Infinity, obstacleLayer);

        // Debug.DrawRay(transform.position, rayEndPoint - rayStartPoint, Color.red);
        
        if (hit.collider != null)
        {
            // 충돌한 물체가 있다면 해당 물체 아래로 이동할 베지어 곡선의 목표 지점 계산
            Vector2 hitPoint = hit.point;
            Vector2 newTarget = new Vector2(hitPoint.x, hitPoint.y - 1f);
            return newTarget;
        }
        else
        {
            // 충돌한 물체가 없다면 현재 위치를 반환하여 이동하지 않도록 함
            return transform.position;
        }
    }
    IEnumerator MoveEnemy(Transform enemy){
        Vector3 startPos = enemy.transform.position;
        // Vector3 originalTargetPos = target.position;
        Vector3 targetPos = startPos + new Vector3(6f,5f,0f);
        Vector3 controlPoint1 = startPos + new Vector3(0f, 0f, 0f);
        Vector3 controlPoint2 = targetPos + new Vector3(-5f, 0f, 0f);
        bool stopMove = false;
        float sTime = Time.time;
        Rigidbody2D enemyRigid = enemy.GetComponent<Rigidbody2D>();
        enemyRigid.gravityScale = 0f;
        Debug.Log("stime"+sTime);
        while ((Time.time - sTime) < curveDuration && !stopMove) {
        float fractionOfJourney = (Time.time - sTime) / curveDuration;
        Vector3 newPosition = BezierCurve(startPos, controlPoint1, controlPoint2, targetPos, fractionOfJourney);
        
        Vector3 rayStartPoint = transform.position;
        Vector3 rayEndPoint = newPosition;

        // 레이를 그려줍니다.
        Debug.DrawRay(rayStartPoint, rayEndPoint - rayStartPoint, Color.red);
        
        // 레이캐스트를 통해 이동 경로에 장애물이 있는지 확인
        RaycastHit2D hit = Physics2D.Raycast(enemy.position, Vector2.up, 1f ,floorLayer);
        if (hit.collider != null) {
            
            Debug.Log("Raycast hit: " + hit.collider);
            Vector3 hitPoint = hit.point;
            Vector3 newTargetPos = new Vector3(hitPoint.x, hitPoint.y - 1.5f, 0f); // 장애물 아래로 위치 조정
            
            targetPos = newTargetPos;
            stopMove = true;
            enemyRigid.velocity = Vector2.zero;
            
            //애니매이션 나오게 하고 아래에서 시간초 계산하면 되겠다
            yield return new WaitForSeconds(1f);
            
           
        }else{
            Debug.Log("장애물없음!");
        }
        if(!stopMove)
            enemy.position = newPosition; // 현재 위치를 새로운 위치로 설정
        
        yield return null;
    }    
        enemyRigid.gravityScale = 1f;
        ShootBullet(enemy);   
        yield return new WaitForSeconds(0.2f);
    }
    
    void ShootBullet(Transform enemy){
        Vector3 bulletPosition = transform.position;
        GameObject bullet = Instantiate(perenArrow,bulletPosition,Quaternion.identity);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        if(bulletRigidbody != null){
            Vector2 directTarget = (enemy.position - transform.position).normalized;
            bulletRigidbody.AddForce(directTarget * 50, ForceMode2D.Impulse);
        }
    }
    Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;
        return p;
    }
    // IEnumerator MoveEnemy(Transform enemy){
    //     Vector3 startPos = enemy.transform.position;
    //     Vector3 targetPos = startPos + new Vector3(6f,5f,0f);

    //     Vector3 controlPoint1 = startPos + new Vector3(0f, 0f, 0f);
    //     Vector3 controlPoint2 = targetPos + new Vector3(-5f, 0f, 0f);
    //     float eLength = Vector3.Distance(startPos, targetPos);
    //     Rigidbody2D enemyRigid = enemy.GetComponent<Rigidbody2D>();
    //     enemyRigid.gravityScale = 0f;
    //     float sTime = Time.time;
    //     Debug.Log("stime"+sTime);
    //     while(Vector3.Distance(enemy.position, targetPos) > 0.01f){
    //         float distance = (Time.time - sTime) * 1f;
    //         Debug.Log(distance+"디스탄스");
    //         float fraction = distance / eLength;
    //         enemy.position = BezierCurve(startPos,controlPoint1, controlPoint2, targetPos,distance);

    //         yield return null;
    //     }
    //     ShootBullet(enemy);
    //     yield return new WaitForSeconds(0.2f);
    //     enemyRigid.gravityScale = 1f;
    //     sTime = Time.time;

    // }
    // void ShootBullet(Transform enemy){
    //     Vector3 bulletPosition = transform.position;
    //     GameObject bullet = Instantiate(perenArrow,bulletPosition,Quaternion.identity);
    //     Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
    //     if(bulletRigidbody != null){
    //         Vector2 directTarget = (enemy.position - transform.position).normalized;
    //         bulletRigidbody.AddForce(directTarget * 50, ForceMode2D.Impulse);
    //     }
    // }
    // Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    // {
    //     float u = 1 - t;
    //     float tt = t * t;
    //     float uu = u * u;
    //     float uuu = uu * u;
    //     float ttt = tt * t;

    //     Vector3 p = uuu * p0;
    //     p += 3 * uu * t * p1;
    //     p += 3 * u * tt * p2;
    //     p += ttt * p3;

    //     return p;
    // }
    
    // IEnumerator MoveEnemy(Transform enemy){
    //     Vector3 startPos = enemy.transform.position;
    //     Vector3 targetPos = startPos + new Vector3(3f,3f,0f);
    //     Debug.Log("작동함");
    //     float eLength = Vector3.Distance(startPos, targetPos);
    //     Rigidbody2D enemyRigid = enemy.GetComponent<Rigidbody2D>();
    //     enemyRigid.gravityScale = 0f;
    //     float sTime = Time.time;
    //     Debug.Log("stime"+sTime);
    //     int i = 0;
    //     while(enemy.position != targetPos){
    //         float distance = (Time.time - sTime) * 5f;
    //         Debug.Log(distance+"디스탄스");
    //         float fraction = distance / eLength;
    //         enemy.position = Vector3.Lerp(startPos,targetPos,fraction);
    //         //     if(i < 1 && distance > 2.5f){
                
    //         //     i++;
    //         // }
    //         yield return null;
    //     }
    //     ShootBullet(enemy);
    //     yield return new WaitForSeconds(0.5f);
    //     enemyRigid.gravityScale = 1f;
    //     // yield return new WaitForSeconds(1.5f);
    //     sTime = Time.time;
    //     while (enemy.position != startPos){
    //         // float distance = (Time.time - sTime) * 5f;
    //         // float fraction = distance / eLength;
    //         // enemy.position = Vector3.Lerp(targetPos,startPos,fraction);
    //         yield return null;
    //     }
    // }
    // void ShootBullet(Transform enemy){
    //     Vector3 bulletPosition = transform.position;
    //     GameObject bullet = Instantiate(bulletObj,bulletPosition,Quaternion.identity);
    //     Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
    //     if(bulletRigidbody != null){
    //         Vector2 directTarget = (enemy.position - transform.position).normalized;
    //         bulletRigidbody.AddForce(directTarget * 50, ForceMode2D.Impulse);
    //     }
    // }
    // void CreateBullet()//적을 맞추고 날라가는스킬
    // {
    //     if(isSkill){

    //         Vector3 bulletPosition = transform.position + transform.up * 1f;
    //         Quaternion bulletRotation = Quaternion.Euler(0f,0f,-90f);
            
    //         GameObject bullet = Instantiate(SkillObj, bulletPosition, bulletRotation);

    //         Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
    //         if(bulletRigidbody != null){
    //             bulletRigidbody.AddForce(transform.right * 20, ForceMode2D.Impulse);
    //         }
    //         StartCoroutine("ResetFiring");
    //         Destroy(bullet,5);
    //     }

    
    //     else if(!isSkill){
    //         Vector3 bulletPosition = transform.position + transform.up * 0.3f;
    //         GameObject bullet = Instantiate(bulletObj,bulletPosition,Quaternion.identity);
    //         Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
    //         if(bulletRigidbody != null){
    //             bulletRigidbody.AddForce(transform.right * 20, ForceMode2D.Impulse);
    //         }
    //     }
        
    // }
    // IEnumerator ResetFiring(){
    //     yield return new WaitForSeconds(0.3f);
    //     isSkill = false;
    //     // yield return StartCoroutine(SkillCheck());
    //     foreach (Enemy enemy in enemies)
    //     {
    //         StartCoroutine(SkillCheck(enemy));
    //     }
    //     yield return new WaitUntil(() => CheckEnter());
    //     foreach (Enemy enemy in enemies)
    //     {
    //         if (enemy != null && enemy.isEnter){
    //             Debug.Log(enemy);
    //             isMove = true;
                
    //             maxSpeed = 100;
    //             Transform enemyTransform = enemy.transform;
    //             Vector2 directionToEnemy = (enemyTransform.position - transform.position).normalized;
    //             directionToEnemy.y = 0;
    //             rigid.velocity = directionToEnemy * 25;

    //             yield return new WaitForSeconds(0.5f);

    //             isMove = false;
                
    //             yield return new WaitForSeconds(0.1f);
    //             maxSpeed = 6f;
    //         }
    //     }
    // }
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
    bool CheckEnter(){
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.isEnter)
            {
                return true;
            }
        }
        return false;
    }
    IEnumerator SkillCheck(Enemy enemy){
        while(!enemy.isEnter){
            yield return null;
        }
    }
}
