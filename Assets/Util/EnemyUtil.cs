﻿using UnityEngine;
using SpriteTile;
using System.Collections.Generic;

public class EnemyUtil {
    private static int squaredGuidedWanderDistance = 200;
    private static float nearbyEnemyRadius = .35f;
    private static float centerDistanceThreshold = .3f;
    private static float cornerAvoidanceDotProductThreshold = -.1f;
    private static Vector2[] directionVectors = new Vector2[] {Vector2.up, Vector2.down, Vector2.left, Vector2.right};

    public static bool CanSee(Vector2 pos1, Vector2 pos2)
    {
        return Physics2D.Linecast(pos1, pos2, LayerMasks.SightObstructedLayerMask).transform == null;
    }

    public static bool PathIsNotBlocked(BoxCollider2D enemyCollider, Vector2 startingPoint, Vector2 destinationPoint, float colliderSizeMultiplierX = 1.25f,
        float colliderSizeMultiplierY = 1.25f, float boxCastDistance = 1f)
    {
        Vector2 colliderSize = enemyCollider.size;
        Vector2 boxCastSize = new Vector2(colliderSize.x * colliderSizeMultiplierX, colliderSize.y * colliderSizeMultiplierY);
        RaycastHit2D boxHit = Physics2D.BoxCast(startingPoint, boxCastSize, 0f, destinationPoint - startingPoint,
            boxCastDistance, LayerMasks.MovementObstructedLayerMask);

        return boxHit.transform == null;
    }

    public static void ExecuteAStar(Transform enemyTransform, Vector2 target, Rigidbody2D rb2d, ref float lastPathfindTime,
        float pathFindingRate, float speed, bool debug = false, bool useCornerAvoidance = false)
    {
        Vector2 enemyPosition = enemyTransform.position;

        if (Time.time > lastPathfindTime + pathFindingRate)
        {
            lastPathfindTime = Time.time;
            List<AStar.Node> list = AStar.calculatePath(AStar.positionToArrayIndices(enemyPosition),
                AStar.positionToArrayIndices(target));

            if (list.Count > 1)
            {
                Int2 targetGridCoordinates = useCornerAvoidance && (enemyPosition - list[0].point.ToVector2()).magnitude > centerDistanceThreshold 
                    && Vector2.Dot(list[1].point.ToVector2() - enemyPosition, list[0].point.ToVector2() - enemyPosition) > cornerAvoidanceDotProductThreshold
                    ? list[0].point : list[1].point;

                if(debug)
                {
                    Debug.DrawLine(enemyTransform.position, AStar.arrayIndicesToPosition(targetGridCoordinates), Color.red, 1f);
                }
                rb2d.velocity = CalculateVelocity(enemyTransform, AStar.arrayIndicesToPosition(targetGridCoordinates), speed);
            }
        }
    }

    public static Vector2 CalculateVelocity(Transform enemyTransform, Vector2 target, float speed)
    {
        Vector3 enemyPosition = enemyTransform.position;
        return CalculateVelocityFromPullVector(enemyTransform, new Vector2(target.x - enemyPosition.x, target.y - enemyPosition.y).normalized,
            speed);
    }

    public static Vector2 CalculateVelocityFromPullVector(Transform enemyTransform, Vector2 pullVector, float speed)
    {
        Vector3 enemyPosition = enemyTransform.position;
        Vector2 pushVector = Vector2.zero;

        // Find all nearby enemies
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(enemyPosition, nearbyEnemyRadius, LayerMasks.EnemyLayerMask);
        int contenders = 0;

        for (int i = 0; i < nearbyEnemies.Length; i++)
        {
            if (nearbyEnemies[i].transform == enemyTransform)
            {
                continue;
            }

            Vector2 push = enemyPosition - nearbyEnemies[i].transform.position;
            pushVector = push.sqrMagnitude == 0f ? Vector2.zero : push / push.sqrMagnitude;

            contenders++;
        }


        pullVector *= Mathf.Max(1, 4 * contenders);
        pullVector += pushVector;

        return pullVector.normalized * speed;
    }

    public static Vector2 CalculateUnblockedDirection(Vector2 pos, Vector2 colliderSize, float castDistance,
        bool wandering)
    {
        if (wandering && Random.Range(0, 2) < 1
            && (Player.PlayerTransform.position - (Vector3)pos).sqrMagnitude < squaredGuidedWanderDistance)
        {
            List<AStar.Node> list = AStar.calculatePath(AStar.positionToArrayIndices(pos),
                AStar.positionToArrayIndices(Player.PlayerTransform.position));

            if (list.Count > 1)
            {
                return (AStar.arrayIndicesToPosition(list[1].point) - pos).normalized;
            }
        }

        Vector2 possibleDir = Vector2.zero;
        int seekIncrement = 15;
        Vector2 vectorToRotate = VectorUtil.RotateVector(directionVectors[Random.Range(0, 4)],
            seekIncrement * Random.Range(0, 25) * Mathf.Deg2Rad);
        for (int rotation = 0; Mathf.Abs(rotation) < 360; rotation += seekIncrement)
        {
            possibleDir = VectorUtil.RotateVector(vectorToRotate, rotation * Mathf.Deg2Rad).normalized;
            if (Physics2D.BoxCast(pos, colliderSize, 0f, possibleDir,
                2f, LayerMasks.MovementObstructedLayerMask).collider == null)
            {
                break;
            }
            // TODO: If no direction found, decrease the movement distance.
        }

        return possibleDir;
    }
}
