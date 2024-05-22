using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour{
    
    public int ArrowDMG = 10;
    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Wall"){
            Destroy(gameObject);
        }
        if(collision.gameObject.tag == "Floor"){
            Destroy(gameObject);
        }
        if(collision.gameObject.tag == "Player"){
            Destroy(gameObject);
            Debug.Log("플레이어 공격");

        }
        
        Destroy(gameObject,5f);
    }
    
}
   