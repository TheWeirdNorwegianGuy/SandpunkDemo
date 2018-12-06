using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour {

    public Slider healthBar;
    public float maxHealth;
    float health;

	// Use this for initialization
	void Start () {
        health = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
        healthBar.value = (health / maxHealth);
	}

    public void Damage(float amount) {
        health -= amount;
        if(health <= 0) {
            Destroy(gameObject);
        }
    }
}
