using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour{
    public GameObject arrowPrefab;
    public float arrowOffset = 1f;
    public int bulletdamgae;
    public Type type;
    public enum Type{Arrow,Skill}
    public bool isPenetrate;
    public List<GameObject> instatarrow = new List<GameObject>();   
    Player player;
    public GameManager gameManager;
    float playerX;
    public void SetPlayerX(float x)
    {
        playerX = x;
    }

    private void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>(); // 게임 매니저 초기화
    }

    void OnCollisionEnter2D(Collision2D collision) {
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Wall"){
            Destroy(gameObject);
        }
        if(collision.gameObject.tag == "Floor"){
            Destroy(gameObject);
        }
        if(collision.CompareTag("Enemy")){
            if(type == Type.Arrow){

                Vector3 collisionPoint = collision.ClosestPoint(transform.position);
                Vector3 arrowPosition = collisionPoint;

                if(isPenetrate){
                    GameObject newArrow = Instantiate(arrowPrefab,arrowPosition,Quaternion.identity);
                    if(playerX > 0){
                        newArrow.transform.localScale = new Vector3(-1,1,1);
                    }
                    newArrow.transform.parent = collision.transform;
                    gameManager.AddArrow(newArrow);

                    player.a++;
                
                    if (gameManager.ArrowCount() > 1)
                    {
                        GameObject oldestArrow = gameManager.GetOldestArrow();
                        gameManager.RemoveArrow(oldestArrow);
                        Destroy(oldestArrow);
                    }
                Destroy(gameObject);
                }
            Destroy(gameObject,5f);
            }
        }
    }
}
   