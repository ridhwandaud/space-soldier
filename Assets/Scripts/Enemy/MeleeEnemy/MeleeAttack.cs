using UnityEngine;
using System.Collections;

public class MeleeAttack : MonoBehaviour {
    public float strikeDistance;
    public int attackStrength;

    public void Strike()
    {
        if ((transform.position - Player.PlayerTransform.position).sqrMagnitude < strikeDistance * strikeDistance)
        {
            Player.PlayerHealth.InflictDamage(1);
        }
    }
}
