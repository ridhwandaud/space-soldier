using UnityEngine;
using System.Collections;

public class MeleeAttack : MonoBehaviour {
    public float strikeDistance;
    public int attackStrength;

    private GameObject player;
    private PlayerHealth playerHealth;

	void Awake () {
        player = GameObject.Find("Soldier");
        playerHealth = player.GetComponent<PlayerHealth>();
	}

    public void Strike()
    {
        if ((transform.position - player.transform.position).sqrMagnitude < strikeDistance * strikeDistance)
        {
            playerHealth.InflictDamage(1);
        }
    }
}
