using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerMove : MonoBehaviour {

    [Header("Moveme Speed")]
    public float moveSpeed;
    public float acelleration;

    [Header("Attacking")]
    public GameObject player;
    public float damage;
    public float attackRange;

    //internal
    Rigidbody rigid;

	// Use this for initialization
	void Start () {
        rigid = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 playerDirection = (player.transform.position - transform.position).normalized;
        playerDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(playerDirection);
        rigid.AddForce(playerDirection * acelleration);
        
        if((player.transform.position - transform.position).magnitude < attackRange) {
            PlayerHP playerHealth = player.GetComponent<PlayerHP>();
            playerHealth.Damage(damage * Time.deltaTime);
        }
	}
}
