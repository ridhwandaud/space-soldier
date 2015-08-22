using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyUtil {
    // TODO: Refactor masks into a common place
    private static int WALL_LAYER_MASK = 1 << 8;

    public static bool CanSeePlayer(Transform enemyTransform, Transform playerTransform)
    {
        return Physics2D.Linecast(enemyTransform.position, playerTransform.transform.position, 
            WALL_LAYER_MASK).transform == null;
    }

    public static bool PathToPlayerIsNotBlocked(BoxCollider2D enemyCollider, Transform enemyTransform, Transform playerTransform)
    {
        Vector2 colliderSize = enemyCollider.size;
        Vector2 boxCastSize = new Vector2(colliderSize.x * 1.25f, colliderSize.y * 1.25f);
        RaycastHit2D boxHit = Physics2D.BoxCast(enemyTransform.position, boxCastSize, 0f, playerTransform.position - enemyTransform.position,
            1, WALL_LAYER_MASK);

        return boxHit.transform == null;
    }
}
