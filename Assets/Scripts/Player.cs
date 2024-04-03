using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour{
    public GameObject gameManager;
    public float maxSpeed;
    public float jumpPower;
    public GameObject Sword;
    public GameObject Hammer;
    public GameObject Bow;
    public GameObject ASskill;
    public int Hammerdamgae;
    public int Sworddamage;
    public int bulletdamgae;
    public GameObject Arrow;
    public GameObject ArrowObj;
    public GameObject perenArrow;
    public GameObject SkillObj;
    public BoxCollider2D jumphammer;
    public BoxCollider2D jumpSword;
    public BoxCollider2D HammerCollider;
    public BoxCollider2D SwordCollider;
    public ParticleSystem Hammerparticle; 
    public List<Enemy> enemies = new List<Enemy>();
    public Vector3 teleportOffset = new Vector3(5f, 0f, 0f); 
    public float detectionDistance = 5f; 
    public LayerMask floorLayer;
    public GameObject hamtag;
    bool isSkill;
    bool skillCol1;
    bool isSkill2;
    bool skillCol2;
    float holdingkey = 0f;
    bool isDash;
    float pressTime;
    public float curveDuration = 0.1f;
    SpriteRenderer spriteRenderer;
    bool HammerCool;
    bool SwordCool;
    bool isMove;
    bool isJump;
    bool isArrow;
    public bool isHammer;
    public int a;
    Rigidbody2D rigid;
    Animator anim;
    
    bool isKeyPressed = false;
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
             float moveInput = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(moveInput) > 0) { 
                transform.localScale = new Vector3(moveInput * -5f, 5f, 1f); // 입력 방향에 따라 scale 값을 조정하여 크기 변경
            }
        }
        SkillPriority();

    }

    void FixedUpdate() {
        if(!isMove){
            float h = Input.GetAxisRaw("Horizontal");

            rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        }
        if(rigid.velocity.x > maxSpeed&&!isDash){
            rigid.velocity = new Vector2(maxSpeed,rigid.velocity.y);
        }else if(rigid.velocity.x < maxSpeed * (-1)&&!isDash){ //left maxspeed계산을 위해 maxSpeed가 음수로 받게함
            rigid.velocity = new Vector2(maxSpeed*(-1),rigid.velocity.y);
        }
        if(checkGrounded()){
            isJump = false;
        }

    }
    void SkillPriority(){
        
        if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)&& !skillCol1 && !isArrow && !SwordCool && !HammerCool){
            Skill1();
        }else if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)&&!skillCol2 && !isArrow && !SwordCool && !HammerCool){
            Skill2();
        }else if(Input.GetButtonDown("AtkSword")&& !isSkill && !SwordCool && !HammerCool && !isArrow){
            OnSword();
            isKeyPressed = true;
            holdingkey = Time.time;
        }else if(Input.GetButtonUp("AtkSword")&&!isSkill && !SwordCool && !HammerCool && !isArrow){
            if(isKeyPressed){
                float Pressduration = Time.time - holdingkey;
                if(Pressduration < 1f){
                    AtkSword();
                }
                else if(Pressduration >= 1f){
                Debug.Log("소드");
                    ChargeAttack("Sword");
                }
                
            }
        }else if(Input.GetButtonDown("AtkHammer")&&!isSkill&&!HammerCool&&!SwordCool && !isArrow){
            isKeyPressed = true;
            // holdingkey = 0;
            holdingkey = Time.time;
            OnHanmmer();
            // if(Input.GetButton("AtkHammer")){
            anim.SetBool("isCharge",true);
            anim.SetBool("Hammer",true);
        }else if(Input.GetButtonUp("AtkHammer")&&!isSkill&&!HammerCool&&!SwordCool && !isArrow){
            if(isKeyPressed){
                float Pressduration = Time.time - holdingkey;
                Debug.Log(Pressduration);
                if(Pressduration < 1f){
                    Debug.Log("일반헤머");
                    AtkHammer();
                    
                    anim.SetBool("isCharge",false);
                    anim.SetBool("Hammer",false);
                }
                else if(Pressduration >= 1f){
                Debug.Log("헤머");
                
                    ChargeAttack("Hammer");
                }
            }
            isKeyPressed = false;
            isMove= false;
        }else if(Input.GetButtonDown("AtkArrow")&& !SwordCool && !HammerCool && !isArrow&& !isSkill){
            OnBow();
            anim.SetTrigger("doShot");
            isKeyPressed = true;
            holdingkey = Time.time;
        }else if(Input.GetButtonUp("AtkArrow")&& !SwordCool && !HammerCool && !isArrow&& !isSkill){
            float Pressduration = Time.time - holdingkey;
            if(isKeyPressed && Pressduration >= 1f){
            Debug.Log("화살");
            
            Debug.Log(Pressduration);
                ChargeAttack("Arrow");
            }
            else{
                AtkArrow();
            }
        }else if(Input.GetButtonDown("Dash")&& !isDash){
            isDash = true;
            isMove = true;
            Vector2 dashDirection = new Vector2(-transform.localScale.x, 0f).normalized;
            Vector2 startPos = rigid.position;
            float dashSpeed = 50f;
            float maxDash = 5f;
            rigid.velocity = dashDirection * dashSpeed;
            Invoke("DashOut", maxDash / dashSpeed); // 최대 대시 거리에 도달하면 일정 시간 후에 대시를 멈춥니다.
        }
    }
    void DashOut(){
        isDash = false;
        isMove = false;
        rigid.velocity = Vector2.zero;
    }
    void Skill1(){
        Debug.Log("A와 S키가 동시에 눌렀습니다.");
        
        OnBow();
        isSkill = true;
        skillCol1 = true;
        StartCoroutine("Charge");
    }
    void Skill2(){
        Debug.Log("A와 D키가 동시에 눌렀습니다.");
        OnHanmmer();
        isSkill = true;
        skillCol2 = true;
        StartCoroutine("ActionSkill2");
    }
    // void HoldSword(){
    //     Debug.Log("홀드검공격");
    //     Sword.SetActive(true);
    //     Hammer.SetActive(false);

    //     anim.SetTrigger("doSWD");
    //     SwordCollider.enabled =true;
        
    //     ChargeAttack("Sword");
    // }
    void AtkSword(){
        OnSword();
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
    
    void AtkHammer(){
        OnHanmmer();
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
    void AtkArrow(){
        OnBow();
        CreateBullet();
        AtkCool("Arrow");
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
    void ChargeAttack(string AtkEnum){
        if(AtkEnum == "Sword"){
            StartCoroutine("ChargeSword");
        }
        else if(AtkEnum == "Hammer"){
            StartCoroutine("ChargeHammer");   
        }
        else if(AtkEnum == "Arrow"){
            isArrow = true;
            StartCoroutine("Charge");
            // StartCoroutine("ArrowCooldown");
        }
    }
    void OnSword(){
        Sword.SetActive(true);
        Hammer.SetActive(false);
        Bow.SetActive(false);
    }
    void OnHanmmer(){
        Sword.SetActive(false);
        Hammer.SetActive(true);
        Bow.SetActive(false);
    }
    void OnBow(){
        Sword.SetActive(false);
        Hammer.SetActive(false);
        Bow.SetActive(true);
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
        yield return new WaitForSeconds(0.1f);
        isArrow = false;
        
    }
    IEnumerator Charge(){
        OnBow();

        Vector3 scale = ASskill.transform.localScale;
        if(isArrow){
            scale.y = 0.7f;
            isMove = true;
            StartCoroutine("ArrowCooldown");
        }
        else if(!isArrow){
            yield return new WaitForSeconds(0.7f);
            isMove = true;
            Debug.Log("확인");
            scale.y = 3f;
        }
        ASskill.transform.localScale = scale;
        ASskill.SetActive(true);
        StartCoroutine("ResetSkillCool");
    }
    IEnumerator ActionSkill2(){
        isHammer = true;
        Hammer.tag = "Skill";
        hamtag.tag = "Skill";
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("doSkill2");
        HammerCollider.enabled = true;
        
        StartCoroutine("ResetFiring");
    }
    IEnumerator ChargeSword(){
        yield return new WaitForSeconds(0.1f);
        Debug.Log("텔포");
        Vector3 teleportPosition = transform.position + (transform.localScale.x > 0 ? -teleportOffset : teleportOffset);
        transform.position = teleportPosition;

        yield return StartCoroutine(DetectAndDamageEnemiesBehind());
    }
    IEnumerator ChargeHammer(){
        Debug.Log("atk들감");
        anim.SetTrigger("Atk");
        HammerCollider.enabled = true;
        yield return new WaitForSeconds(0.001f);
        
        anim.SetBool("Hammer",false);
        anim.SetBool("isCharge",false);
        Debug.Log("wldnrl");
        yield return new WaitForSeconds(0.5f);
        HammerCollider.enabled = false;
    }
    IEnumerator DetectAndDamageEnemiesBehind()
    {
        Vector2 playerPosition = transform.position;
        Vector2 playerDirection = transform.right;
        teleportOffset.x = detectionDistance;
        Debug.DrawRay(playerPosition, playerDirection * detectionDistance, Color.red);

        LayerMask enemyLayerMask = LayerMask.GetMask("Enemy");

        RaycastHit2D[] hits = Physics2D.RaycastAll(playerPosition, playerDirection, detectionDistance, enemyLayerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<Enemy>().TakeDamage(100);

                Debug.Log("플레이어 뒤에 적이 감지되었습니다.");
                Debug.Log(hit.collider.name);
            }
        }
        
        anim.SetTrigger("Atk");
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("Sword",false);
        anim.SetBool("isCharge",false);
        isKeyPressed = false;
        SwordCool = false;

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
            Arrow.SetActive(false);
            if(bulletRigidbody != null){
                if(transform.localScale.x < 0){
                    
                bulletRigidbody.AddForce(transform.right * 20, ForceMode2D.Impulse);
                }else{

                bulletRigidbody.AddForce(transform.right * -20, ForceMode2D.Impulse);
                bullet.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
            
            Arrow.SetActive(true);
        }
        
        
    }
    IEnumerator ResetSkillCool(){
        isSkill = false;
        isSkill2 = false;
        isKeyPressed = false;
        yield return new WaitForSeconds(0.2f);
        isMove = false;
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
                Debug.Log(enemy);
                StartCoroutine(MoveEnemy(enemy.transform));
            }
        }
        
        StartCoroutine("ResetSkillCool");
    }
    
    IEnumerator MoveEnemy(Transform enemy){
        Vector3 startPos = enemy.transform.position;

        Vector3 controlPoint1, controlPoint2,targetPos;
        if (transform.localScale.x > 0) {
            targetPos = startPos + new Vector3(-3f,5f,0f);
            controlPoint1 = startPos + new Vector3(0f, 3f, 0f); // 시작점 위로
            controlPoint2 = targetPos + new Vector3(0f, 0f, 0f); // 끝점 위로
        } else {
            
            targetPos = startPos + new Vector3(3f,5f,0f);
            controlPoint1 = startPos + new Vector3(0f, 3f, 0f); // 시작점 오른쪽으로
            controlPoint2 = targetPos + new Vector3(0f, 0f, 0f); // 끝점 왼쪽으로
        }
        bool stopMove = false;
        float sTime = Time.time;
        Rigidbody2D enemyRigid = enemy.GetComponent<Rigidbody2D>();
        enemyRigid.gravityScale = 0f;
        Debug.Log("stime"+sTime);
        curveDuration = 0.8f;
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
        }
        if(!stopMove)
            enemy.position = newPosition; // 현재 위치를 새로운 위치로 설정
        
        yield return null;
    }    
        Hammer.tag = "Hammer";
        hamtag.tag = "Hammer";
        OnBow();
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
    bool checkGrounded() {
        Debug.DrawRay(rigid.position, Vector2.down, new Color(0, 1, 0));
        Vector2 raycastStart = rigid.position - Vector2.up * 1f;
        RaycastHit2D rayHit = Physics2D.Raycast(raycastStart, Vector2.down, 1, LayerMask.GetMask("Floor")|LayerMask.GetMask("Enemy"));
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
    