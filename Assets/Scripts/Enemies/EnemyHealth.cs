using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    public float health;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void damage(float amount) {
        health -= amount;
        if(health <= 0) {
            Destroy(gameObject);
        }
    }
}
