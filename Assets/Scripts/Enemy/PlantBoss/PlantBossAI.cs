using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlantBossAI : MonoBehaviour {

    public FiniteStateMachine<PlantBossAI> fsm;

    private List<Vector2> BulletOffsets = new List<Vector2>() { new Vector2(1f, 0)};
    private float BulletDistanceFromCenter;
    private float RotationAmount = Mathf.PI * 5 / 180;
    private int NumShotsPerVolley;
    private float SecondsBetweenShots;

    public StackPool ProjectilePool;

    private Rigidbody2D rb2d;
    private CircleCollider2D circleCollider;

	void Awake () {
        rb2d = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        fsm = new FiniteStateMachine<PlantBossAI>(this, null);
	}
	
	void Update () {
        fsm.Update();
	}

    public IEnumerator FireVolley()
    {

        int numShotsFired = 0;
        while (numShotsFired < NumShotsPerVolley)
        {
            // fire shot
            for (int i = 0; i < BulletOffsets.Count; i++)
            {
                Vector2 currOffset = BulletOffsets[i];
                float newX = BulletDistanceFromCenter * Mathf.Cos(Mathf.Atan2(currOffset.y, currOffset.x) + RotationAmount);
                float newY = BulletDistanceFromCenter * Mathf.Sin(Mathf.Atan2(currOffset.y, currOffset.x) + RotationAmount);
                BulletOffsets[i] = new Vector2(newX, newY);

            }

            numShotsFired++;
            yield return new WaitForSeconds(SecondsBetweenShots);
        }
    }

    // Refactor
    void FireShot(Vector2 shotOrigin, Vector2 target)
    {
        GameObject projectile = ProjectilePool.Pop();
        projectile.transform.position = shotOrigin;


    }
}
