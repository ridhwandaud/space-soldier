using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyUtil {
    // TODO: Refactor masks into a common place
    private static int WALL_LAYER_MASK = 1 << 8;
    private static float BOX_CAST_DISTANCE = .5f;

    public static bool CanSee(Vector2 pos1, Vector2 pos2)
    {
        return Physics2D.Linecast(pos1, pos2, WALL_LAYER_MASK).transform == null;
    }

    public static bool PathIsNotBlocked(BoxCollider2D enemyCollider, Vector2 pos1, Vector2 pos2, float colliderSizeMultiplierX = 1.25f,
        float colliderSizeMultiplierY = 1.25f)
    {
        Vector2 colliderSize = enemyCollider.size;
        Vector2 boxCastSize = new Vector2(colliderSize.x * colliderSizeMultiplierX, colliderSize.y * colliderSizeMultiplierY);
        RaycastHit2D boxHit = Physics2D.BoxCast(pos1, boxCastSize, 0f, pos2 - pos1,
            BOX_CAST_DISTANCE, WALL_LAYER_MASK);

        return boxHit.transform == null;
    }
}
