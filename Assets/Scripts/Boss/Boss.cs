using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UIElements;

public class Boss : MonoBehaviour
{
    [SerializeField]private GameObject rockStart;
    [SerializeField]private GameObject tlaser;
    [SerializeField]private GameObject laser;
    public int flooringdmg;
    BoxCollider2D sadFlooring;
    public float dropInterval = 3f;
    public float laserinterval = 6f;
    public int laserDmg;
    public bool angerdoor = true;
    public bool saddoor = false;
    public bool happydoor = false;
    GameObject sadflooreffect;
    bool islaser;
    int lasery;
    float timer;
    float pattern1timer;
    bool ispattern;
    public float transtimer = 0;
    public bool isRestrict;
    Rigidbody2D rigid;
    void Update() {
        timer += Time.deltaTime;
        transtimer += Time.deltaTime;
        if(angerdoor){
                if(!islaser){
                pattern1timer += Time.deltaTime;
                }
            if(pattern1timer >= laserinterval){
                    StartCoroutine("Laser");
                pattern1timer = 0;
            }
            if(timer >= dropInterval){   
                Vector3 dropPosition = GetRandomPointInBox();
                StartCoroutine(DropWithEffect(dropPosition));
                timer = 0;
            }
            // if(transtimer > 60){
            //     saddoor = true;
            //     angerdoor = false;
            //     happydoor = false;
            //     transtimer = 0;
            // }
        }
        if(saddoor){
            pattern1timer += Time.deltaTime;
            if(timer > 5f){
                Vector3 droppos = rockStart.transform.position;
                droppos.y = 0;
                StartCoroutine(sadfloor(droppos));
                timer = 0;
            }
            if(!ispattern){
                if(pattern1timer >= 3f){
                    StartCoroutine("SadPattern2");
                    pattern1timer = 0;
                    ispattern = true;
                }
            }
            if(transtimer >= 20f){
                saddoor = false;
                angerdoor = true;
                happydoor = false;
                BossEffectPool.Instance.ReturnEffectall();
                transtimer = 0;
            }
        }
    }
    private void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        
    }
    IEnumerator Laser(){
        int y = Random.Range(0,3);
        islaser = true;
        pattern1timer = 0;
        if(y == 0){
            lasery = -1;
            tlaser.transform.position = new Vector3(tlaser.transform.position.x,lasery,0); 
        }
        if(y == 1){
            lasery = 2;
            tlaser.transform.position = new Vector3(tlaser.transform.position.x,lasery,0); 
        }
        if(y == 2){
            lasery = 5;
            tlaser.transform.position = new Vector3(tlaser.transform.position.x,lasery,0); 
        }
        tlaser.SetActive(true);
        yield return new WaitForSeconds(2f);
        tlaser.SetActive(false);
        laser.transform.position = new Vector3(laser.transform.position.x,lasery,0);
        rigid.AddForce(transform.position.normalized);
        laser.SetActive(true);
        yield return new WaitForSeconds(1f);
        laser.SetActive(false);
        pattern1timer = 0;
        islaser = false;
    }
    IEnumerator DropWithEffect(Vector3 position) {
        GameObject effect = BossEffectPool.Instance.GetEffect();
        effect.transform.position = new Vector3(position.x,-2.5f,0);
        yield return new WaitForSeconds(2f); 
        BossEffectPool.Instance.ReturnEffect(effect);
        yield return new WaitForSeconds(0.3f); 
        GameObject drop = BossRockPool.Instance.GetFromPool();
        drop.transform.position = position;
        yield return new WaitForSeconds(2f); 
        BossRockPool.Instance.AddToPool(drop);
    }
    IEnumerator SadPattern2() {

        // GameObject effect = BossEffectPool.Instance.GetSadPattern2();
        // effect.transform.position = new Vector3(position.x,-2.5f,0);
        // yield return new WaitForSeconds(2f); 
        // BossEffectPool.Instance.ReturnSadPattern2();
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.bossskill = true;
        isRestrict = true;
        
        yield return new WaitForSeconds(0.3f); 

        // GameObject drop = BossRockPool.Instance.GetFromPool();

        // drop.transform.position = position;
        // yield return new WaitForSeconds(2f); 
        // BossRockPool.Instance.AddToPool(drop);
    }
    Vector3 GetRandomPointInBox(){
        BoxCollider2D collider = rockStart.GetComponent<BoxCollider2D>();
        if (collider != null) {
            Vector2 size = collider.size;
            Vector2 offset = collider.offset;
            float x = Random.Range(-size.x / 2, size.x / 2) + offset.x;
            float y = Random.Range(-size.y / 2, size.y / 2) + offset.y;
            Vector3 dropPosition = new Vector3(x, y, 0); 
            
            return rockStart.transform.TransformPoint(dropPosition); 
        }
        return rockStart.transform.position;
    }
    IEnumerator sadfloor(Vector3 position){
        sadflooreffect = BossEffectPool.Instance.GetfloorEffect();
        sadflooreffect.transform.position = new Vector3(position.x,-2.5f,0);
        yield return new WaitForSeconds(1f);
        sadFlooring = sadflooreffect.GetComponent<BoxCollider2D>();
        sadFlooring.enabled = true;

        if(transtimer >= 16f){
            Debug.Log("?");
        }
    }
}
