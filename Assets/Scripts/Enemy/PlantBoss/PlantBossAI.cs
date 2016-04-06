using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlantBossAI : MonoBehaviour {

    public FiniteStateMachine<PlantBossAI> fsm;
    public bool firing = false;

    private List<Vector2> BulletOffsets = new List<Vector2>() {
        new Vector2(1f, 0),
        new Vector2(0, 1f),
        new Vector2(-1f, 0),
        new Vector2(0, -1f)};
    [SerializeField]
    private float bulletDistanceFromCenter;
    [SerializeField]
    private float degreesBetweenShots;
    [SerializeField]
    private float timeBetweenVolleys;

    [SerializeField]
    private int numShotsPerVolley;
    [SerializeField]
    private float secondsBetweenShots;
    [SerializeField]
    private float projectileSpeed;
    [SerializeField]
    private string projectilePoolName;

    private StackPool projectilePool;
    private Rigidbody2D rb2d;

	void Awake () {
        rb2d = GetComponent<Rigidbody2D>();
        projectilePool = GameObject.Find(projectilePoolName).GetComponent<StackPool>();
        fsm = new FiniteStateMachine<PlantBossAI>(this, PlantBossAttackState.Instance);
	}
	
	void Update () {
        fsm.Update();
	}

    public IEnumerator FireVolley()
    {

        int numShotsFired = 0;
        int rotationCoefficient = Random.Range(0, 2) == 0 ? 1 : -1;
        while (numShotsFired < numShotsPerVolley)
        {
            // fire shot
            for (int i = 0; i < BulletOffsets.Count; i++)
            {
                Vector2 currOffset = BulletOffsets[i];
                float rotationRadians = rotationCoefficient * Mathf.PI * degreesBetweenShots / 180;
                float newX = bulletDistanceFromCenter * Mathf.Cos(Mathf.Atan2(currOffset.y, currOffset.x) + rotationRadians);
                float newY = bulletDistanceFromCenter * Mathf.Sin(Mathf.Atan2(currOffset.y, currOffset.x) + rotationRadians);
                BulletOffsets[i] = new Vector2(newX, newY);
                FireShot(transform.position, new Vector2(transform.position.x + BulletOffsets[i].x, transform.position.y + BulletOffsets[i].y));
            }

            numShotsFired++;
            yield return new WaitForSeconds(secondsBetweenShots);
        }
        Invoke("EndVolley", timeBetweenVolleys);
    }

    void EndVolley()
    {
        firing = false;
    }

    void FireShot(Vector2 shotOrigin, Vector2 target)
    {
        GameObject projectile = projectilePool.Pop();
        projectile.transform.position = shotOrigin;
        Vector3 offset = target - shotOrigin;
        float rotation = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation - 90));
        projectile.SetActive(true);

        projectile.GetComponent<Rigidbody2D>().velocity = projectileSpeed * offset.normalized;
    }
}
