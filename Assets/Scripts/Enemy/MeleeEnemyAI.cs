using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeEnemyAI : MonoBehaviour {
    public int chargeDistance = 12;
    public float speed = 2f;
    public float pathFindingRate = 2f;
    public float chaseTime = 3f;

    private int wallLayerMask = 1 << 8;

    private GameObject player;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider2d;

    private bool chasing = false;
    private bool isFirstFrame = true;
    private float lastPathfindTime = 0;

	void Awake () {
        // Can't set this in inspector because these are generated via prefabs.
        player = GameObject.Find("Soldier");
        rb2d = GetComponent<Rigidbody2D>();
	}
	
	void Update () {
        if (isFirstFrame)
        {
            isFirstFrame = false;
            return;
        }

        Vector2 enemyPosition = gameObject.transform.position;
        Vector2 playerPosition = player.transform.position;

        float distanceFromPlayer = Vector3.Distance(playerPosition, enemyPosition);

        if (EnemyUtil.CanSeePlayer(transform, player.transform))
        {
            // Just realized this is not quite true because the player might not be in range, but functionally the result is the
            // same.
            chasing = true;
            CancelInvoke("DeactivateChase");
            if (distanceFromPlayer <= chargeDistance)
            {
                
            }
        }
        else
        {
            Invoke("DeactivateChase", chaseTime);
        }
	}

    void ExecuteAStar(Vector2 enemyPosition, Vector2 playerPosition)
    {
        if (Time.time > lastPathfindTime + pathFindingRate)
        {
            lastPathfindTime = Time.time;
            List<AStar.Node> list = AStar.calculatePath(AStar.positionToArrayIndices(enemyPosition),
                AStar.positionToArrayIndices(playerPosition));

            if (list.Count > 1)
            {
                Vector2 target = AStar.arrayIndicesToPosition(list[1].point);
                rb2d.velocity = new Vector2(target.x - transform.position.x, target.y - transform.position.y);
            }
        }
    }

    void DeactivateChase()
    {
        chasing = false;
    }

    // Refactor this into a common class. Will be using this a lot.
    bool CanSeePlayer()
    {
        RaycastHit2D linecastHit = Physics2D.Linecast(transform.position, player.transform.position, wallLayerMask);

        return linecastHit.transform == null;
    }
}
