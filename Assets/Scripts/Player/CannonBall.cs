using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour {
    public float Damage;
    public float ExplotionRadius;
    float life;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        life += Time.deltaTime;
	}

    void OnTriggerEnter(Collider other) {
        if(life >= 0.3f) {
            Collider[] hits = Physics.OverlapSphere(transform.position, ExplotionRadius);
            for(int i = 0; i < hits.Length; i++) {
                EnemyHealth enemy = hits[i].gameObject.GetComponent<EnemyHealth>();
                if (enemy) {
                    enemy.damage(Damage);

                }
            }
            Destroy(gameObject);
        }

    }

}
