using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UIElements;

public class Boss : MonoBehaviour
{
    [SerializeField]private GameObject rockStart;
    [SerializeField]private GameObject tlaser;
    [SerializeField]private GameObject laser;
    public float dropInterval = 5f;
    public float laserinterval = 6f;
    public int laserDmg;
    bool islaser;
    int lasery;
    float timer;
    float laserTimer;
    Rigidbody2D rigid;
    void Update() {
        timer += Time.deltaTime;
        laserTimer += Time.deltaTime;
        

        if(laserTimer >= laserinterval){
            if(!islaser)
                StartCoroutine("Laser");
            laserTimer = 0;
        }
        if(timer >= dropInterval){
            
            Vector3 dropPosition = GetRandomPointInBox();
            StartCoroutine(DropWithEffect(dropPosition));
            timer = 0;
        }
    }
    private void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        
    }
    IEnumerator Laser(){
        int y = Random.Range(0,3);
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
        yield return new WaitForSeconds(4f);
        tlaser.SetActive(false);
        laser.transform.position = new Vector3(laser.transform.position.x,lasery,0);
        rigid.AddForce(transform.position.normalized);
        laser.SetActive(true);
        yield return new WaitForSeconds(1f);
        laser.SetActive(false);
        laserTimer = 0;
    }
    IEnumerator DropWithEffect(Vector3 position) {
        GameObject effect = BossRockEffectPool.Instance.GetEffect();
        effect.transform.position = new Vector3(position.x,0,0);
        yield return new WaitForSeconds(2f); 
        BossRockEffectPool.Instance.ReturnEffect(effect);
        yield return new WaitForSeconds(0.3f); 
        GameObject drop = BossRockPool.Instance.GetFromPool();
        drop.transform.position = position;
        yield return new WaitForSeconds(2f); 
        BossRockPool.Instance.AddToPool(drop);
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
}
