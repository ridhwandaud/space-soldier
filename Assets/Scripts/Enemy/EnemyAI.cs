using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour {
    public float chaseActivationRadius;
    public float chaseCheckInterval;
    public float chaseCheckCooldown;
    public bool chasing = false;

    //TODO: Add jitter so everyone isn't checking at the same time.
    private float nextChaseCheckTime = 0;

    protected void Update()
    {
        if (chasing == false && Time.time > nextChaseCheckTime)
        {
            nextChaseCheckTime += chaseCheckInterval;
            Collider2D[] otherEnemies = Physics2D.OverlapCircleAll(transform.position, chaseActivationRadius, LayerMasks.EnemyLayerMask);
            for (int x = 0; x < otherEnemies.Length; x++)
            {
                if (otherEnemies[x].GetComponent<EnemyAI>().chasing)
                {
                    chasing = true;
                    break;
                }
            }
        }
    }

    protected void DeactivateChase()
    {
        chasing = false;
        // To prevent enemies from perpetually chasing due to immediately reactivating chase after idling.
        nextChaseCheckTime += chaseCheckCooldown;
    }
}
