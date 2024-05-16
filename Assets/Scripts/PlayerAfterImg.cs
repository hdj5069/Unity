using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImg : MonoBehaviour
{
    [SerializeField]
    private float activetime = 0.1f;
    private float timeActivated;
    private float alpha;
    
    [SerializeField]
    private float alphaSet = 0.8f;
    private float alphaMultiplier = 0.98f;
    private Transform player;
    private SpriteRenderer SR;
    private SpriteRenderer playerSR;
    private Color color;
    private void OnEnable() {
        SR = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSR = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        SR.sprite = playerSR.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        timeActivated = Time.time;
    }
    private void Update() {
        alpha *= alphaMultiplier;
        color = new Color(0,0,0,alpha);
        SR.color = color;
        if(Time.time >= (timeActivated + activetime)){
            PlayerAfterImgPool.Instance.AddToPool(gameObject);
        }
    }
}
