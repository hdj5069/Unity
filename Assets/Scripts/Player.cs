using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour{
    
    [Header("쿨타임")]
    [SerializeField]private float SkillasCool,SkilladCool,SkillsdCool,SkillasdCool;
    
    [Header("최고속도,두번쨰 점프, 점프높이")]
    [SerializeField]private float maxSpeed,againjump,jumpPower;
    [Header("데미지")]
    public int Hammerdamgae;
    public int Sworddamage;
    public int bulletdamgae;

    public int skillasdmg;
    [Header("건들지마")]
    public int PlayerMaxHP = 100;
    public int PlayerCurHP;

    [SerializeField]private GameObject Sword;
    [SerializeField]private GameObject Hammer;
    [SerializeField]private GameObject playerPrefab;
    [SerializeField]private GameObject gameManager;
    [SerializeField]private GameObject Bow;
    [SerializeField]private GameObject ASskill;
    [SerializeField]private GameObject EnemyArrow;
    [SerializeField]private GameObject ArrowObj;
    [SerializeField]private GameObject perenArrow;
    [SerializeField]private GameObject SkillObj;
    [SerializeField]private BoxCollider2D jumphammer;
    [SerializeField]private BoxCollider2D jumpSword;
    [SerializeField]private BoxCollider2D HammerCollider;
    [SerializeField]private BoxCollider2D SwordCollider;
    [SerializeField]private ParticleSystem Hammerparticle; 
    public List<Enemy> enemies = new List<Enemy>();
    private Vector3 teleportOffset = new Vector3(5f, 0f, 0f); 
    private float detectionDistance = 5f; 
    [SerializeField]private LayerMask floorLayer;
    [SerializeField]private GameObject hamtag;
    public float distanceBetweenImages;
    bool isSkill,isSkillasd;
    bool skillasCol,skillsdCol,skilladCol,skillasdCol;
    float holdingkey = 0f;
    float curveDuration = 0.1f;
    SpriteRenderer spriteRenderer;
    bool HammerCool,SwordCool,ArrowCool;
    bool isMove,isJump,isDash,isDashCool,isagainJump;
    bool isArrow,isHammer,isSword;
    public bool HammerSkill,SwordSkill,arrowskill;
    private float dashTimeLeft;
    private float lastImageXpos;
    private float lastDash = -100f;
    public float dashTime;
    public float dashSpeed;
    public float dashCooldown;

    public int a;
    float targetTime = -Mathf.Infinity;
    Rigidbody2D rigid;
    Animator anim;
    bool enemyCheck;
    bool isKeyPressed = false;
    Enemy enemydmg;
    EnemyBullet enemybullet;
    void Awake() {
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in allEnemies) {
            enemies.Add(enemy);
        }
        // mob = GameObject.FindWithTag("Enemy").GetComponent<Enemy>();
        rigid = GetComponent<Rigidbody2D>(); 
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();  
        anim = GetComponent<Animator>();
        // enemydmg = GameObject.FindWithTag("Enemy").GetComponent<Enemy>();
        // enemybullet= GameObject.FindWithTag("Enemy").GetComponent<EnemyBullet>();
    }
    private void Start() {
        PlayerCurHP = PlayerMaxHP;
    }
    void Update() {
        if(rigid.velocity.y < 0){
            if(anim.GetBool("isJump")){
                anim.SetBool("isJump",false);

            }
            // anim.SetBool("fall",true);
        }
        // isMove = true;
        if(!isMove){
            if(Input.GetButtonDown("Vertical") && !isJump){
                isJump =true;
                anim.SetBool("isJump",true);
            // anim.SetTrigger("fall");
                rigid.velocity = new Vector2(rigid.velocity.x,0);
                rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            }else if(Input.GetButtonDown("Vertical") && !isagainJump){
                float jumpagagin = againjump;
                isagainJump = true;
                if(rigid.velocity.y < 0){
                    rigid.velocity = new Vector2(rigid.velocity.x,0);
                }else if(rigid.velocity.y > 0){
                    rigid.velocity = new Vector2(rigid.velocity.x,0);
                    jumpagagin = againjump + 0.5f;
                }
                rigid.AddForce(Vector2.up * jumpagagin, ForceMode2D.Impulse);
            }
            if(Input.GetButtonUp("Horizontal")){
                rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f,rigid.velocity.y);
            }
            float moveInput = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(moveInput) > 0) { 
                transform.localScale = new Vector3(moveInput * -1f, 1f, 1f); // 입력 방향에 따라 scale 값을 조정하여 크기 변경
            }
        }
        if(Input.GetButton("Horizontal")){
            if(!isMove){
                float h = Input.GetAxisRaw("Horizontal");
                rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
                anim.SetBool("isRun",true);
            }else{
                rigid.AddForce(Vector2.zero);
            }
            
        }else{
            anim.SetBool("isRun",false);
        }
        SkillPriority();
        if(PlayerCurHP > PlayerMaxHP){
            PlayerCurHP = PlayerMaxHP;
        }
        if(PlayerCurHP < 0){
            Debug.Log("죽음");
        }
    }

    void FixedUpdate() {
        
            if(rigid.velocity.x > maxSpeed&&!isDash){
                rigid.velocity = new Vector2(maxSpeed,rigid.velocity.y);
            }else if(rigid.velocity.x < maxSpeed * (-1)&&!isDash){ //left maxspeed계산을 위해 maxSpeed가 음수로 받게함
                rigid.velocity = new Vector2(maxSpeed*(-1),rigid.velocity.y);
            }
        
        CheckDash();
        if(checkGrounded()){
            isJump = false;
            isagainJump = false;
            anim.SetBool("isFall",false);
        }
    }
    bool prac;
    void SkillPriority(){
        if (Input.GetKey(KeyCode.A) &&Input.GetKey(KeyCode.S) &&Input.GetKey(KeyCode.D) && !skillasdCol && !ArrowCool && !SwordCool && !HammerCool && !isSkill) {
            skillasdCol = true;
            isSkill = true;
            SkillASD();
        }
        else if (Input.GetKey(KeyCode.A)&&Input.GetKey(KeyCode.S)&&!Input.GetKey(KeyCode.D) && !skillasCol && !ArrowCool && !SwordCool && !HammerCool && !isSkill) {
            skillasCol = true;
            isSkill = true;
            arrowskill = true;
            SkillAS();
        }else if (!Input.GetKey(KeyCode.A)&&Input.GetKey(KeyCode.S)&&Input.GetKey(KeyCode.D) && !skillsdCol && !ArrowCool && !SwordCool && !HammerCool && !isSkill &&!isJump ) {
            skillsdCol = true;
            SkillSD();
            isSkill = true;
        }
        else if(Input.GetKey(KeyCode.A)&&!Input.GetKey(KeyCode.S)&&Input.GetKey(KeyCode.D) &&!skilladCol && !ArrowCool && !SwordCool && !HammerCool&& !isSkill &&!isJump){
            skilladCol = true;
            SkillAD();
            isSkill = true;
        }else if(Input.GetButtonDown("AtkSword")&& !isSkill && !HammerCool && !SwordCool && !ArrowCool&&!isArrow&&!isHammer&&!isSword){
            OnSword();
            isSword = true;
            isKeyPressed = true;
            holdingkey = Time.time;
            anim.SetBool("isCharge",true);
            anim.SetBool("doSword",true);
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
            OnHammmer();
            anim.SetBool("isCharge",true);
            anim.SetBool("doHammer",true);
        }else if(Input.GetButtonUp("AtkHammer")&& !isSkill && !HammerCool && !SwordCool && !ArrowCool&&isHammer){
            if(isKeyPressed){
                float Pressduration = Time.time - holdingkey;
                if(Pressduration < 1f){

                    anim.SetBool("isCharge",false);
                    anim.SetBool("doHammer",false);
                    
                    AtkHammer();
                    AtkCool("Hammer");
                }
                else if(Pressduration >= 1f){
                    ChargeAttack("Hammer");
                }
            }
            
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
                AtkArrow();
            }
            
            AtkCool("Arrow");
            isArrow = false;
        }
        else if(Input.GetButtonDown("Dash")&& !isDash&&!isDashCool){
            AttemptToDash();
        }
    }
    void AttemptToDash(){
        isDashCool = true;
        isDash = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImgPool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }
    void CheckDash(){
        if(isDash){
            if(dashTimeLeft > 0){
                isMove = true;
                
                float direction = transform.localScale.x > 0 ? -1 : 1;
                rigid.velocity = new Vector2(dashSpeed * direction ,0f);
                dashTimeLeft -= Time.deltaTime;

                if(Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages){
                    PlayerAfterImgPool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }
            if(dashTimeLeft <= 0 || checkWallAhead()){
                isMove = false;
                isDash = false;
                rigid.velocity = Vector2.zero;

                Invoke("DashOut",0.7f);
            }
        }
    }
    void DashOut(){
        isDashCool = false;
    }


    void SkillAS(){
        Debug.Log("A와 S키가 동시에 눌렀습니다.");
        if(anim.GetBool("doSword")){
            anim.SetBool("isCharge",false);
            anim.SetBool("doSword",false);
        }
        anim.ResetTrigger("doShot");
        anim.ResetTrigger("Atk");
        OnBow();
        StartCoroutine("Charge");
    }
    void SkillAD(){
        Debug.Log("A와 D키가 동시에 눌렀습니다.");
        anim.ResetTrigger("doShot");
        if(anim.GetBool("doHammer")){
            anim.SetBool("doHammer",false);
            anim.SetBool("isCharge",false);
        }
        
        OnHammmer();
        
        detailAD();
        }
    void SkillASD(){
        if(skillasdCol){
            anim.ResetTrigger("doSkill2");
            if(anim.GetBool("doHammer")){
                anim.SetBool("doHammer",false);
                anim.SetBool("isCharge",false);
            }
            if(anim.GetBool("doSword")){
                anim.SetBool("isCharge",false);
                anim.SetBool("doSword",false);
            }
            
            anim.ResetTrigger("doShot");
            Debug.Log("ASD");
            isSkillasd = true;
            CreateBullet();
        }
        
    }
    
    void SkillSD(){
        if(anim.GetBool("doHammer")){
            anim.SetBool("doHammer",false);
            anim.SetBool("isCharge",false);
        }
        if(anim.GetBool("doSword")){
            anim.SetBool("isCharge",false);
            anim.SetBool("doSword",false);
        }
        OnSword();
        StartCoroutine("detailSD");
    }

    void AtkSword(){
        if(isJump){
            anim.SetBool("Sword",true);
            jumpSword.enabled = true;
        }
        if(!isJump){
            isMove = true;
            SwordCollider.enabled =true;
            anim.SetTrigger("doSWD");
            StartCoroutine("Delay");
        }
    }
    
    void AtkHammer(){
        if(isJump){
            // anim.SetBool("isJump",true);
            anim.SetBool("Hammer",true);
        }
        if(!isJump){
            isMove = true;
            HammerCollider.enabled = true;
            anim.SetTrigger("doHAM");
            StartCoroutine("Delay");
        }
    }
    IEnumerator Delay(){
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f);
            rigid.velocity = Vector2.zero;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f);
        
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("SwingHam")){
            HammerCollider.enabled = false;
        } 
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("noSWD")){
            SwordCollider.enabled = false;
        } else{

        }
        isMove = false;
    }
    void AtkArrow(){
        isMove = true;
        StartCoroutine("Delay");
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
            scale.y = 1.5f;
            isMove = true;
            rigid.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.5f);
        }
        rigid.AddForce(transform.position.normalized);
        ASskill.transform.localScale = scale;
        ASskill.SetActive(true);
        enemyCheck = true;
        yield return new WaitForSeconds(0.5f);
        isMove = false;
        ASskill.SetActive(false);
        StartCoroutine("ResetSkillCool");
        StartCoroutine("skillASTimes");
        isSkill = false;

        enemyCheck = false;
        yield return new WaitForSeconds(0.1f);
    }
    
    IEnumerator chargeD(){
        // isSkill = true;
        HammerSkill = true;
        Hammer.tag = "Skill";
        hamtag.tag = "Skill";
        HammerCollider.enabled = true;
        // bool hasTime = false;;
        yield return new WaitForSeconds(0.1f);
        targetTime = Time.time;
        anim.SetTrigger("Atk");
        yield return new WaitUntil(() => CheckEnter()||chargeHamTime());
        yield return new WaitForSeconds(0.1f);
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.isEnter){
                StartCoroutine(MoveEnemy(enemy.transform));
                enemy.isEnter = false;
            }else{
            }
        }
        yield return new WaitForSeconds(0.1f);
        
        Hammer.tag = "Hammer";
        hamtag.tag = "Hammer";
        HammerCollider.enabled = false;
        HammerSkill = false;
        
        AtkCool("Hammer");
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


    IEnumerator detailSD(){
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
            yield return new WaitUntil(() => CheckEnter()||checkWallAhead()||skillsdTime()||checkbossAhead());

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
        StartCoroutine("skillSDTimes");
        
        StartCoroutine("ResetSkillCool");
    }

    void detailAD(){
        for(int i = 0; i< 8;i++){
            if(transform.localScale.x < 0){
                Vector3 bulletPosition = transform.position + transform.forward * -100f + new Vector3(0,0,100f);
                GameObject bullet = Instantiate(ArrowObj,bulletPosition,Quaternion.identity);
                Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
                float randomAngle = Random.Range(-10f,45f);
                Quaternion randomRotation = Quaternion.Euler(0f,0f,randomAngle);
                bullet.transform.rotation = randomRotation;
                Vector2 forceDirection = randomRotation * Vector2.right;
                bulletRigidbody.AddForce(forceDirection * 20, ForceMode2D.Impulse);
            }
            else{
                Vector3 bulletPosition = transform.position + transform.forward * 100f + new Vector3(0,0,100f);
                GameObject bullet = Instantiate(ArrowObj,bulletPosition,Quaternion.identity);
                Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
                float randomAngle = Random.Range(-45f,10f);
                Quaternion randomRotation = Quaternion.Euler(0f,0f,randomAngle);
                bullet.transform.rotation = randomRotation;
                Vector2 forceDirection = randomRotation * Vector2.left;
                bulletRigidbody.AddForce(forceDirection * 20, ForceMode2D.Impulse);
                bullet.transform.localScale = new Vector3(-1, 1, 1);
            }
            StartCoroutine("skillADTimes");
            StartCoroutine("ResetSkillCool");
        }
        
    }

    IEnumerator Skillasd(){
        if(!prac){
            Debug.Log("왜 작동해");
        isSkillasd = false; 
        bool hasTime = false;
        targetTime = Time.time;
        yield return new WaitForSeconds(0.3f);
        // foreach (Enemy enemy in enemies){
        //     StartCoroutine(SkillCheck(enemy));
        // }
        yield return new WaitUntil(() => CheckEnter()||skilalsdTime());

        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.isEnter){
                hasTime = true;
                Debug.Log(enemy);
                Transform enemyTransform = enemy.transform;
                Vector3 targetPosition = enemyTransform.position - enemyTransform.forward * 2; // 적의 위치에서 앞으로 2만큼 떨어진 곳으로 설정
                GameObject playerClone = Instantiate(playerPrefab, targetPosition, Quaternion.identity);
                Destroy(gameObject);
                enemy.TakeDamage(100);
                enemy.isEnter = false;
            }
        }
        if(!hasTime){
        StartCoroutine("skillASDTimes");
        StartCoroutine("ResetSkillCool");
        yield break;
        }
        
        StartCoroutine("skillASDTimes");
        StartCoroutine("ResetSkillCool");
        }
    }
        
        IEnumerator ResetSkillCool(){
        isArrow = false;
        isHammer = false;
        isSword = false;
        yield return new WaitForSeconds(1f);
        isSkill = false;
        HammerSkill = false;
        isKeyPressed = false;
    }
    bool skillsdTime() {
        Debug.Log(Time.time - targetTime);
        if (Time.time - targetTime >= 1.5f) { 
            return true;
        } else {
            return false;
        }
    }
    bool skilalsdTime() {
        Debug.Log(Time.time - targetTime);
        if (Time.time - targetTime >= 2f) { 
            return true;
        } else {
            return false;
        }
    }
    bool chargeHamTime() {
        Debug.Log(Time.time - targetTime);
        if (Time.time - targetTime >= 0.6f) { 
            return true;
        } else {
            return false;
        }
    }
    IEnumerator ChargeHammer(){
        
        StartCoroutine("chargeD");
        anim.SetBool("doHammer",false);
        anim.SetBool("isCharge",false);
        yield return new WaitForSeconds(0.001f);
        
    }

    IEnumerator ChargeSword(){
        yield return new WaitForSeconds(0.1f);
        Debug.Log("텔포");
        
        LayerMask wallLayerMask = LayerMask.GetMask("Wall");
        LayerMask FloorLayer = LayerMask.GetMask("Floor");
        LayerMask BossLayerMask = LayerMask.GetMask("Boss");
        Vector3 teleportDirection = Vector3.zero;
        if(Input.GetKey(KeyCode.LeftArrow)){
            teleportDirection += Vector3.left;
        }
        else if(Input.GetKey(KeyCode.RightArrow)){
            teleportDirection += Vector3.right;
        }
        else if(Input.GetKey(KeyCode.UpArrow)){
            teleportDirection += Vector3.up;
        }
        else if(Input.GetKey(KeyCode.DownArrow)){
            teleportDirection += Vector3.down;
        }
        else if (transform.localScale.x > 0) {
            teleportDirection += Vector3.left;
        }
        else if (transform.localScale.x < 0) {
            teleportDirection += Vector3.right;
        }
        Vector3 playerPos = transform.position;
        Vector3 teleportPosition = playerPos + (teleportDirection.normalized * teleportOffset.magnitude);
        Debug.DrawRay(playerPos, teleportPosition - playerPos, Color.blue,3f);
        RaycastHit2D hit = Physics2D.Raycast(playerPos, teleportPosition - playerPos, Vector3.Distance(teleportPosition, playerPos), wallLayerMask|BossLayerMask|FloorLayer);
        if (hit.collider != null){
            // 벽이 있으면 텔포 위치를 벽의 바로 앞으로 이동
            if(!hit.collider.CompareTag("Floor")){
            teleportPosition = hit.point - (hit.normal * 0.1f);
            //hit.point로 ray로 벽이 있는지 확인하여 확인된 곳에 위치를 point로 저장 
            //hit.normal * 0.1f로 충돌지점에서 플레이어를 밀어냄
            }
            else{
                teleportPosition = hit.point + (hit.normal * 0.5f);
            }
            if(hit.collider.CompareTag("Boss")){
                    
                hit.collider.GetComponent<Enemy>().TakeDamage(100);
            }
        }
        LayerMask enemyLayerMask = LayerMask.GetMask("Enemy");
        RaycastHit2D[] hits = Physics2D.RaycastAll(playerPos, teleportPosition - playerPos, Vector3.Distance(teleportPosition, playerPos), enemyLayerMask);

        foreach (RaycastHit2D hited in hits)
        {
            if (hited.collider.CompareTag("Enemy"))
            {
                hited.collider.GetComponent<Enemy>().TakeDamage(100);

                Debug.Log("플레이어 뒤에 적이 감지되었습니다.");
                Debug.Log(hited.collider.name);
            }
        }
        
        transform.position = teleportPosition;
        anim.SetTrigger("Atk");
        yield return new WaitForSeconds(0.001f);
        anim.SetBool("Sword",false);
        anim.SetBool("isCharge",false);
        isKeyPressed = false;
        SwordCool = false;
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
        curveDuration = 0.8f;
        while ((Time.time - sTime) < curveDuration && !stopMove) {
        float fractionOfJourney = (Time.time - sTime) / curveDuration;

        Vector3 newPosition = BezierCurve(startPos, controlPoint1, controlPoint2, targetPos, fractionOfJourney);
        
        Vector3 rayStartPoint = transform.position;
        Vector3 rayEndPoint = newPosition;

        Debug.DrawRay(rayStartPoint, rayEndPoint - rayStartPoint, Color.red);
        
        RaycastHit2D hit = Physics2D.Raycast(enemy.position, Vector2.up, 1f ,floorLayer);
            if (hit.collider != null) {
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
        
        enemyRigid.gravityScale = 1f;
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
    bool checkWallAhead() {
        float direction = Mathf.Sign(transform.localScale.x); // 플레이어의 방향을 구함
        Vector2 raycastStart = rigid.position + Vector2.right * 0.5f * -direction;

        RaycastHit2D rayHit = Physics2D.Raycast(raycastStart, Vector2.right * direction, 1, LayerMask.GetMask("Wall")|LayerMask.GetMask("Boss"));

        return (rayHit.collider != null);

    }
    bool checkbossAhead() {
        float direction = Mathf.Sign(transform.localScale.x); // 플레이어의 방향을 구함
        Vector2 raycastStart = rigid.position + Vector2.right * 0.5f * -direction;

        RaycastHit2D rayHit = Physics2D.Raycast(raycastStart, Vector2.right * direction, 1, LayerMask.GetMask("Wall")|LayerMask.GetMask("Boss"));

        return (rayHit.collider != null);

    }
    bool checkGrounded() {
        Debug.DrawRay(rigid.position, Vector2.down, new Color(0, 1, 0));
        Vector2 raycastStart = rigid.position - Vector2.up * 1f;
        RaycastHit2D rayHit = Physics2D.Raycast(raycastStart, Vector2.down, 1, LayerMask.GetMask("Floor")|LayerMask.GetMask("Enemy"));
        if(rigid.velocity.y < 0){
            if (rayHit.collider != null && rayHit.distance < 0.3f) {
                anim.SetBool("isJump",false);
                // anim.SetBool("isfall",false);
                // anim.StopPlayback();
                // anim.ResetTrigger("fall");
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
            Hammerparticle.enableEmission = true;
        }
        anim.SetBool("Sword",false);
        anim.SetBool("Hammer",false);
        jumphammer.enabled = false;
        
        SwordCollider.enabled =false;
        yield return new WaitForSeconds(0.3f);
        Hammerparticle.enableEmission = false;
    }
    IEnumerator skillASTimes(){
        yield return new WaitForSeconds(SkillasCool);
        skillasCol = false;
    }IEnumerator skillADTimes(){
        yield return new WaitForSeconds(SkilladCool);
        skilladCol = false;
    }IEnumerator skillSDTimes(){
        yield return new WaitForSeconds(SkillsdCool);
        skillsdCol = false;
    }IEnumerator skillASDTimes(){
        Debug.Log(SkillasdCool);
        prac = true;
        yield return new WaitForSeconds(10f);
        // yield return new WaitForSeconds(SkillasdCool);
        Debug.Log(skillasdCol);
        skillasdCol = false;
    }
    bool CheckEnter(){
        foreach (Enemy enemy in enemies){
            if (enemy != null && enemy.isEnter){
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
    void OnTriggerEnter2D(Collider2D other) {
        var script = other.GetComponent<EnemyBullet>();
        var BossRock = other.GetComponent<rock>();
        var Boss = other.GetComponentInParent<Boss>();
        Debug.Log("충돌"+other.name);
        if(other.tag == "Enemy"){
            if(script != null){
                PlayerCurHP = PlayerCurHP - script.ArrowDMG;
                Debug.Log(PlayerCurHP);
            }
        }    
        if(other.tag == "Boss"){
            if(BossRock != null){
                PlayerCurHP -= BossRock.BossRockDmg;
            }
        }
        if(other.tag == "Bosslaser"){
            if(Boss!=null){
                PlayerCurHP -= Boss.laserDmg;
                Debug.Log(Boss.laserDmg);
                Debug.Log("레이저맞음");
            }
        }
        if(other.tag == "BossDMG"){
            if(Boss!=null){
                PlayerCurHP -= Boss.flooringdmg;
                Debug.Log("장판데미지");
                Debug.Log(PlayerCurHP);
            }
        }
    }
}
