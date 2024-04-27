using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour{
    public GameObject playerPrefab;
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
    public GameObject EnemyArrow;
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
    bool isSkill,isSkillasd;
    bool skillCol1,skillCol2;
    float holdingkey = 0f;
    public float curveDuration = 0.1f;
    SpriteRenderer spriteRenderer;
    bool HammerCool,SwordCool,ArrowCool;
    bool isMove,isJump,isDash,isDashCool;
    bool isArrow,isHammer,isSword;
    public bool HammerSkill,SwordSkill;
    public int a;
    float targetTime = -Mathf.Infinity;
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
        if (Input.GetKey(KeyCode.A) &&Input.GetKey(KeyCode.S) &&Input.GetKey(KeyCode.D) && !skillCol1 && !ArrowCool && !SwordCool && !HammerCool && !isSkill) {
            isSkill = true;
            Skill3();
        }
        else if (Input.GetKey(KeyCode.A)&&Input.GetKey(KeyCode.S)&&!Input.GetKey(KeyCode.D) && !skillCol1 && !ArrowCool && !SwordCool && !HammerCool && !isSkill) {
            isSkill = true;
            Skill1();
        }else if (!Input.GetKey(KeyCode.A)&&Input.GetKey(KeyCode.S)&&Input.GetKey(KeyCode.D) && !skillCol1 && !ArrowCool && !SwordCool && !HammerCool && !isSkill  ) {
            Skill4();
            isSkill = true;
        }
        else if(Input.GetKey(KeyCode.A)&&!Input.GetKey(KeyCode.S)&&Input.GetKey(KeyCode.D) &&!skillCol2 && !ArrowCool && !SwordCool && !HammerCool&& !isSkill &&!isJump){
            Skill2();
            isSkill = true;
        }else if(Input.GetButtonDown("AtkSword")&& !isSkill && !HammerCool && !SwordCool && !ArrowCool&&!isArrow&&!isHammer){
            OnSword();
            isSword = true;
            isKeyPressed = true;
            holdingkey = Time.time;
            anim.SetBool("doSword",true);
            anim.SetBool("isCharge",true);
        }else if(Input.GetButtonUp("AtkSword")&& !isSkill && !HammerCool && !SwordCool && !ArrowCool && isSword){
            if(isKeyPressed){
                float Pressduration = Time.time - holdingkey;
                if(Pressduration < 1f){
                    anim.SetBool("isCharge",false);
                    anim.SetBool("doSword",false);
                    
                    AtkSword();
                }
                else if(Pressduration >= 1f){
                    ChargeAttack("Sword");
                }
                isSword = false;
                AtkCool("Sword");
            }
        }else if(Input.GetButtonDown("AtkHammer")&& !isSkill && !HammerCool && !SwordCool && !ArrowCool&&!isArrow&&!isSword){
            isHammer = true;
            isKeyPressed = true;
            holdingkey = Time.time;
            Debug.Log("눌림");
            OnHammmer();
            anim.SetBool("isCharge",true);
            anim.SetBool("doHammer",true);
        }else if(Input.GetButtonUp("AtkHammer")&& !isSkill && !HammerCool && !SwordCool && !ArrowCool&&isHammer){
            if(isKeyPressed){
                float Pressduration = Time.time - holdingkey;
                Debug.Log(Pressduration);
                if(Pressduration < 1f){
                    Debug.Log("일반헤머");

                    anim.SetBool("isCharge",false);
                    anim.SetBool("doHammer",false);
                    
                    AtkHammer();
                }
                else if(Pressduration >= 1f){
                Debug.Log("헤머");
                    ChargeAttack("Hammer");
                }
            }
            
         AtkCool("Hammer");
            isHammer = false;
            isKeyPressed = false;
        }
        else if(Input.GetButtonDown("AtkArrow")&& !SwordCool && !HammerCool && !ArrowCool&& !isSkill && !isHammer && !isSword){
            isArrow = true;
            OnBow();
            anim.SetTrigger("doShot");
            isKeyPressed = true;
            holdingkey = Time.time;
        }else if(Input.GetButtonUp("AtkArrow")&& !SwordCool && !HammerCool && !ArrowCool&& !isSkill &&isArrow){
            float Pressduration = Time.time - holdingkey;
            if(isKeyPressed && Pressduration  >= 1f){
            Debug.Log(Pressduration);
                ChargeAttack("Arrow");
            }
            else{
            Debug.Log("?!");
                AtkArrow();
            }
            
            AtkCool("Arrow");
            isArrow = false;
        }
        else if(Input.GetButtonDown("Dash")&& !isDash&&!isDashCool){
            isDash = true;
            isDashCool = true;
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
        rigid.velocity = Vector2.zero;
        Invoke("DashCool",2f);
    }
    void DashCool(){
        isDashCool = false;
    }

    void Skill1(){
        Debug.Log("A와 S키가 동시에 눌렀습니다.");
        OnBow();
        skillCol1 = true;
        StartCoroutine("Charge");
    }
    void Skill2(){
        Debug.Log("A와 D키가 동시에 눌렀습니다.");
        if(anim.GetBool("doHammer")){
            anim.SetBool("doHammer",false);
            anim.SetBool("isCharge",false);
        }
        OnHammmer();
        skillCol2 = true;
        StartCoroutine("ActionSkill2");
    }
    void Skill3(){
        anim.ResetTrigger("doSkill2");
        if(anim.GetBool("doHammer")){
            anim.SetBool("doHammer",false);
            anim.SetBool("isCharge",false);
        }
        if(anim.GetBool("doSword")){
            anim.SetBool("isCharge",false);
            anim.SetBool("doSword",false);
        }
        Debug.Log("ASD");
        isSkillasd = true;
        CreateBullet();
    }
    void Skill4(){
        if(anim.GetBool("doHammer")){
            anim.SetBool("doHammer",false);
            anim.SetBool("isCharge",false);
        }
        if(anim.GetBool("doSword")){
            anim.SetBool("isCharge",false);
            anim.SetBool("doSword",false);
        }
        Debug.Log("SD");
        OnSword();
        StartCoroutine("SkillSD");
    }
    void AtkSword(){
        if(isJump){
            anim.SetBool("isJump",true);
            anim.SetBool("Sword",true);
            jumpSword.enabled = true;
        }
        if(!isJump){
            anim.SetTrigger("doSWD");
            SwordCollider.enabled =true;
        }
    }
    
    void AtkHammer(){
        if(isJump){
            anim.SetBool("isJump",true);
            anim.SetBool("Hammer",true);
        }
        if(!isJump){
            HammerCollider.enabled = true;
            anim.SetTrigger("doHAM");
        }
    }
    void AtkArrow(){
        CreateBullet();
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
            ArrowCool = true;
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
            ArrowCool = true;
            StartCoroutine("Charge");
            StartCoroutine("ArrowCooldown");
        }
    }
    void OnSword(){
        Sword.SetActive(true);
        Hammer.SetActive(false);
        Bow.SetActive(false);
    }
    void OnHammmer(){
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
        ArrowCool = false;
    }
    IEnumerator Charge(){
        OnBow();

        Vector3 scale = ASskill.transform.localScale;
        if(ArrowCool){
            scale.y = 0.7f;
            StartCoroutine("ArrowCooldown");
        }else{
            scale.y = 3f;
            isMove = true;
            yield return new WaitForSeconds(0.7f);
        }
        ASskill.transform.localScale = scale;
        ASskill.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        isMove = false;
        ASskill.SetActive(false);
        StartCoroutine("ResetSkillCool");
        isSkill = false;
    }
    IEnumerator ActionSkill2(){
        HammerSkill = true;
        Hammer.tag = "Skill";
        hamtag.tag = "Skill";
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("doSkill2");
        HammerCollider.enabled = true;
        yield return new WaitForSeconds(0.3f);
        bool hasTime = false;
        yield return new WaitForSeconds(0.1f);
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.isEnter){
                hasTime = true;
                Debug.Log(enemy);
                StartCoroutine(MoveEnemy(enemy.transform));
                enemy.isEnter = false;
            }
        }
        if(!hasTime){
            Hammer.tag = "Hammer";
            hamtag.tag = "Hammer";
            HammerCollider.enabled = false;
            isSkill = false;
            StartCoroutine("ResetSkillCool");
            yield break;
        }
    }
    IEnumerator ChargeSword(){
        yield return new WaitForSeconds(0.1f);
        Debug.Log("텔포");
        Vector3 teleportPosition = transform.position + (transform.localScale.x > 0 ? -teleportOffset : teleportOffset);
        transform.position = teleportPosition;

        yield return StartCoroutine(DetectAndDamageEnemiesBehind());
    }
    IEnumerator ChargeHammer(){
        anim.SetTrigger("Atk");
        HammerCollider.enabled = true;
        yield return new WaitForSeconds(0.001f);
        
        anim.SetBool("doHammer",false);
        anim.SetBool("isCharge",false);
        yield return new WaitForSeconds(0.5f);
        HammerCollider.enabled = false;
    }
    IEnumerator DetectAndDamageEnemiesBehind(){
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
    void CreateBullet(){
        if(isSkillasd){
            Vector3 bulletPosition = transform.position + transform.up * 1f;
            float direction = transform.localScale.x > 0 ? -1 : 1;

            Quaternion rotation = Quaternion.Euler(0, 0, direction > 0 ? -90f : 90f);
            
            GameObject bullet = Instantiate(SkillObj, bulletPosition, rotation);

            Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            if(bulletRigidbody != null){
                if(transform.localScale.x > 0){
                    bulletRigidbody.AddForce(transform.right * -50, ForceMode2D.Impulse);
                }if(transform.localScale.x < 0){
                    bulletRigidbody.AddForce(transform.right * 50, ForceMode2D.Impulse);
                }
                
                bullet.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            }
            StartCoroutine("Skillasd");
            Destroy(bullet,5);
        }
        else if(!isSkill){
            Vector3 bulletPosition = transform.position + transform.up * 0.3f;
            GameObject bullet = Instantiate(ArrowObj,bulletPosition,Quaternion.identity);
            Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            bullet bulletScript = bullet.GetComponent<bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetPlayerX(transform.position.x);
            }
            if(bulletRigidbody != null){
                if(transform.localScale.x < 0){
                bulletRigidbody.AddForce(transform.right * 20, ForceMode2D.Impulse);
                }
                else{
                bulletRigidbody.AddForce(transform.right * -20, ForceMode2D.Impulse);
                bullet.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
        }
    }
    IEnumerator ResetSkillCool(){
        yield return new WaitForSeconds(1f);
        HammerSkill = false;
        isSkillasd = false;
        isKeyPressed = false;
        yield return new WaitForSeconds(0.2f);
        skillCol1 = false;
        skillCol2 = false;
        
    }

    IEnumerator SkillSD(){
        SwordSkill = true;
        Sword.tag = "Skill";
        yield return new WaitForSeconds(0.1f);
        SwordCollider.enabled = true;
        isMove = true;
        if (transform.localScale.x > 0) {
                rigid.AddForce(Vector2.right * -100f, ForceMode2D.Impulse);
            }else{
                rigid.AddForce(Vector2.right * 100f, ForceMode2D.Impulse);
            }
        maxSpeed *= 2;
        Debug.Log(maxSpeed);
        targetTime = Time.time;
        while(!checkWallAhead()){
            if(skillsdTime()){
                break;
            }
            foreach (Enemy enemy in enemies){
                StartCoroutine(SkillCheck(enemy));
            }
            yield return new WaitUntil(() => CheckEnter()||checkWallAhead()||skillsdTime());

            foreach (Enemy enemy in enemies){
                if (enemy != null && enemy.isEnter){
                    
                    Transform enemyTransform = enemy.transform;
                    enemyTransform.SetParent(playerPrefab.transform);
                    Collider2D collider = enemy.GetComponent<Collider2D>();
                    collider.isTrigger = true;
                    enemy.rigid.isKinematic = true;
                }
            }
        }
        
        foreach (Enemy enemy in enemies){
            if (enemy != null && enemy.isEnter){
                Transform enemyTransform = enemy.transform;
                enemyTransform.SetParent(null);
                Collider2D collider = enemy.GetComponent<Collider2D>();
                collider.isTrigger = false;
                enemy.rigid.isKinematic = false;
                enemy.isEnter = false;
            }
        }
        
        isMove = false;
        maxSpeed /= 2;
        // if(isSkill){
        //걍 애니매이션으로 처리해
        OnHammmer();
        // }
        
        isSkill = false;
        yield return new WaitForSeconds(3f);
        
        StartCoroutine("ResetSkillCool");
    }

    bool checkWallAhead() {
        float direction = Mathf.Sign(transform.localScale.x); // 플레이어의 방향을 구함
        Vector2 raycastStart = rigid.position + Vector2.right * 0.5f * -direction;

        RaycastHit2D rayHit = Physics2D.Raycast(raycastStart, Vector2.right * direction, 1, LayerMask.GetMask("Wall"));

        return (rayHit.collider != null);

    }
    bool skillsdTime() {
        Debug.Log(Time.time - targetTime);
        if (Time.time - targetTime >= 1.5f) { 
            return true;
        } else {
            return false;
        }
    }

    IEnumerator Skillasd(){
        yield return new WaitForSeconds(0.3f);
        foreach (Enemy enemy in enemies){
            StartCoroutine(SkillCheck(enemy));
        }
        yield return new WaitUntil(() => CheckEnter());
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.isEnter){
                Debug.Log("??");
                Debug.Log(enemy);
                Transform enemyTransform = enemy.transform;
                Vector3 targetPosition = enemyTransform.position - enemyTransform.forward * 2; // 적의 위치에서 앞으로 2만큼 떨어진 곳으로 설정
                GameObject playerClone = Instantiate(playerPrefab, targetPosition, Quaternion.identity);
                Destroy(gameObject);
                enemy.TakeDamage(100);
                enemy.isEnter = false;
                yield break;
            }

        }
        isSkill =false;
        StartCoroutine("ResetSkillCool");
    }
    IEnumerator MoveEnemy(Transform enemy){
        Vector3 startPos = enemy.transform.position;

        Vector3 controlPoint1, controlPoint2,targetPos;
        if (transform.localScale.x > 0) {
            targetPos = startPos + new Vector3(-3f,5f,0f);
            controlPoint1 = startPos + new Vector3(0f, 3f, 0f); 
            controlPoint2 = targetPos + new Vector3(0f, 0f, 0f); 
        } else {
            
            targetPos = startPos + new Vector3(3f,5f,0f);
            controlPoint1 = startPos + new Vector3(0f, 3f, 0f); 
            controlPoint2 = targetPos + new Vector3(0f, 0f, 0f); 
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

        Debug.DrawRay(rayStartPoint, rayEndPoint - rayStartPoint, Color.red);
        
        RaycastHit2D hit = Physics2D.Raycast(enemy.position, Vector2.up, 1f ,floorLayer);
            if (hit.collider != null) {
                Debug.Log("Raycast hit: " + hit.collider);
                Vector3 hitPoint = hit.point;
                Vector3 newTargetPos = new Vector3(hitPoint.x, hitPoint.y - 1.5f, 0f); 
                
                targetPos = newTargetPos;
                stopMove = true;
                enemyRigid.velocity = Vector2.zero;
                
                yield return new WaitForSeconds(1f);
            }
            if(!stopMove)
                enemy.position = newPosition; 
            
            yield return null;
        }    
        Hammer.tag = "Hammer";
        hamtag.tag = "Hammer";
        OnBow();
        enemyRigid.gravityScale = 1f;
        ShootBullet(enemy);   
        yield return new WaitForSeconds(0.2f);
        
        isSkill = false;
        StartCoroutine("ResetSkillCool");
    }
    
    void ShootBullet(Transform enemy){
        Vector3 bulletPosition = transform.position;    
        float direction = transform.localScale.x > 0 ? -1 : 1;

        Quaternion rotation = Quaternion.Euler(0, 0, direction > 0 ? 0 : 180);

        GameObject bullet = Instantiate(perenArrow,bulletPosition,rotation);
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
