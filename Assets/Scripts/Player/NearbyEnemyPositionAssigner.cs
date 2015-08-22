using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NearbyEnemyPositionAssigner : MonoBehaviour {

    public float timeBetweenReassignments = .5f;
    // TODO: Tweak.
    public float meleeEnemyAssignmentRadius = 5f;
    public float attackOffset = .5f;

    private List<MeleeEnemyAI> nearbyMeleeEnemies = new List<MeleeEnemyAI>();
    private List<Vector2> unclaimedTokens;
    private List<Vector2> claimedTokens = new List<Vector2>();

    private float lastAssignmentTime;

    void Awake()
    {
        unclaimedTokens = new List<Vector2> { 
            new Vector2(0, attackOffset), // Top
            new Vector2(0, -attackOffset), // Bottom
            new Vector2(-attackOffset, 0), // Left
            new Vector2(attackOffset, 0), // Right
            new Vector2(-attackOffset, attackOffset), // Top Left
            new Vector2(attackOffset, attackOffset), // Top Right
            new Vector2(-attackOffset, -attackOffset), // Bottom Left
            new Vector2(attackOffset, -attackOffset) // Bottom Right
        };
    }

    void Update()
    {
        if (Time.time > lastAssignmentTime + timeBetweenReassignments)
        {
            nearbyMeleeEnemies.Clear();
            lastAssignmentTime = Time.time;
            Collider2D[] overlappingEnemies = Physics2D.OverlapCircleAll(transform.position, meleeEnemyAssignmentRadius);
            for (int x = 0; x < overlappingEnemies.Length; x++)
            {
                MeleeEnemyAI potentialMeleeEnemy = overlappingEnemies[x].GetComponent<MeleeEnemyAI>();
                if (potentialMeleeEnemy != null)
                {
                    nearbyMeleeEnemies.Add(potentialMeleeEnemy);
                }
            }
        }
    }


}
