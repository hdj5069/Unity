using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rock : MonoBehaviour
{
    public int BossRockDmg;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "RockEnd"){
            BossRockPool.Instance.AddToPool(gameObject);
        }
    }
}
