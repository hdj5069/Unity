using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.Find("Player");

        transform.position = new Vector3(player.transform.position.x,player.transform.position.y,player.transform.position.z - 10);
    }
}
