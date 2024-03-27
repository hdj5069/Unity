using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour{
    public int bulletdamgae;


    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "Floor"){
            Destroy(gameObject,3);
        }
    }
    void OnTriggerEnter(Collider collision) {
        if(collision.gameObject.tag == "Wall"){
            Destroy(gameObject);
        }
    }
}
   