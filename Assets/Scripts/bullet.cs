using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour{
    public GameObject arrowPrefab;
    public float arrowOffset = 0.5f;
    public int bulletdamgae;
    public Type type;
    public enum Type{Arrow,Skill}

    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Floor"){
            Destroy(gameObject,3);
        }

    }
    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Wall"){
            Destroy(gameObject);
        }
        if(collision.CompareTag("Enemy")){
            if(type == Type.Arrow){
                Vector3 collisionPoint = collision.ClosestPoint(transform.position);
                Vector3 direction = (collisionPoint - transform.position).normalized;
                Vector3 arrowPosition = collisionPoint - direction * arrowOffset;

                GameObject newArrow = Instantiate(arrowPrefab,arrowPosition,Quaternion.identity);

                newArrow.transform.parent = collision.transform;

                Destroy(gameObject);
                Destroy(newArrow,5);
            }
        }
    }
}
   