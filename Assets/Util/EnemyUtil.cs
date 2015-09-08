using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyUtil {
    public static bool CanSee(Vector2 pos1, Vector2 pos2)
    {
        return Physics2D.Linecast(pos1, pos2, LayerMasks.WALL_LAYER_MASK).transform == null;
    }

    public static bool PathIsNotBlocked(BoxCollider2D enemyCollider, Vector2 pos1, Vector2 pos2, float colliderSizeMultiplierX = 1.25f,
        float colliderSizeMultiplierY = 1.25f, float boxCastDistance = 1f)
    {
        Vector2 colliderSize = enemyCollider.size;
        Vector2 boxCastSize = new Vector2(colliderSize.x * colliderSizeMultiplierX, colliderSize.y * colliderSizeMultiplierY);
        RaycastHit2D boxHit = Physics2D.BoxCast(pos1, boxCastSize, 0f, pos2 - pos1,
            boxCastDistance, LayerMasks.WALL_LAYER_MASK);

        return boxHit.transform == null;
    }

    public static void ExecuteAStar(Transform enemyTransform, Vector2 target, Rigidbody2D rb2d, ref float lastPathfindTime,
        float pathFindingRate, float speed, float nearbyEnemyRadius)
    {
        Vector2 enemyPosition = enemyTransform.position;

        if (Time.time > lastPathfindTime + pathFindingRate)
        {
            lastPathfindTime = Time.time;
            List<AStar.Node> list = AStar.calculatePath(AStar.positionToArrayIndices(enemyPosition),
                AStar.positionToArrayIndices(target));

            if (list.Count > 1)
            {
                rb2d.velocity = CalculateVelocity(enemyTransform, AStar.arrayIndicesToPosition(list[1].point), speed, nearbyEnemyRadius);
            }
        }
    }

    public static Vector2 CalculateVelocity(Transform enemyTransform, Vector2 target, float speed, float nearbyEnemyRadius)
    {
        Vector3 enemyPosition = enemyTransform.position;
        Vector2 pullVector = new Vector2(target.x - enemyPosition.x,
            target.y - enemyPosition.y).normalized * speed;
        Vector2 pushVector = Vector2.zero;

        // Find all nearby enemies
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(enemyPosition, nearbyEnemyRadius, LayerMasks.ENEMY_LAYER_MASK);
        int contenders = 0;

        for (int i = 0; i < nearbyEnemies.Length; i++)
        {
            if (nearbyEnemies[i].transform == enemyTransform)
            {
                continue;
            }

            Vector2 push = enemyPosition - nearbyEnemies[i].transform.position;
            pushVector += push / push.sqrMagnitude;

            contenders++;
        }


        pullVector *= Mathf.Max(1, 4 * contenders);
        pullVector += pushVector;

        return pullVector.normalized * speed;
    }
}
