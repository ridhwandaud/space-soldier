using UnityEngine;
using System.Collections;

public class BasicEnemyAI : MonoBehaviour {

    public int attackDistance = 10;

    private GameObject player;
    private Wander wanderScript;
    private BasicEnemyFire fireScript;
    private Rigidbody2D rb2d;

    void Awake()
    {
        player = GameObject.Find("Soldier");
        wanderScript = GetComponent<Wander>();
        fireScript = GetComponent<BasicEnemyFire>();
        rb2d = GetComponent<Rigidbody2D>();
    }
	
	void Update () {
        if (Vector3.Distance(player.transform.position, gameObject.transform.position) <= attackDistance)
        {
            rb2d.velocity = new Vector2(0, 0);
            fireScript.Fire();
        }
        else
        {
            wanderScript.DoWander();
        }
	}
}
