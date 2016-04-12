using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlantBossAI : MonoBehaviour {

    public FiniteStateMachine<PlantBossAI> Fsm;
    public bool Firing = false;
    public List<Seed> Seeds = new List<Seed>();
    public float initialSeedSpeed;
    public bool Awakened = false;

    private List<Vector2> SeedOffsets = new List<Vector2>() {
        new Vector2(1f, 0),
        new Vector2(0, 1f),
        new Vector2(-1f, 0),
        new Vector2(0, -1f)};
    [SerializeField]
    private float bulletDistanceFromCenter;
    [SerializeField]
    private float degreesBetweenSpores;
    // Make sure degreesBetweenMissiles is a factor of 360
    [SerializeField]
    private float degreesBetweenMissiles;
    [SerializeField]
    private float timeBetweenVolleys;
    [SerializeField]
    private int numShotsPerVolley;
    [SerializeField]
    private float secondsBetweenSporeShots;
    [SerializeField]
    private float secondsBetweenCircleAttacks;
    [SerializeField]
    private float projectileSpeed;
    [SerializeField]
    private string projectilePoolName;
    [SerializeField]
    private int seedCount;

    private StackPool projectilePool;
    private Rigidbody2D rb2d;
    private GameObject seedPrefab;

	void Awake () {
        seedPrefab = Resources.Load("Seed") as GameObject;
        rb2d = GetComponent<Rigidbody2D>();
        projectilePool = GameObject.Find(projectilePoolName).GetComponent<StackPool>();
        Fsm = new FiniteStateMachine<PlantBossAI>(this, PlantBossSleepState.Instance);

        for (int x = 0; x < seedCount; x++)
        {
            GameObject seed = Instantiate(seedPrefab, transform.position, Quaternion.identity) as GameObject;
            seed.SetActive(false);
            Seeds.Add(seed.GetComponent<Seed>());
        }
	}
	
	void Update () {
        Fsm.Update();
	}

    public IEnumerator FireSporeVolley()
    {
        yield return new WaitForSeconds(timeBetweenVolleys);

        if (GameState.NumEnemiesRemaining == 0)
        {
            EndSporeVolley();
            yield break;
        }

        int numShotsFired = 0;
        int rotationCoefficient = Random.Range(0, 2) == 0 ? 1 : -1;
        while (numShotsFired < numShotsPerVolley)
        {
            // fire shot
            for (int i = 0; i < SeedOffsets.Count; i++)
            {
                Vector2 currOffset = SeedOffsets[i];
                float rotationRadians = rotationCoefficient * Mathf.PI * degreesBetweenSpores / 180;

                // This is a bit of overkill if I decide to stick with a single origin point.
                float newX = bulletDistanceFromCenter * Mathf.Cos(Mathf.Atan2(currOffset.y, currOffset.x) + rotationRadians);
                float newY = bulletDistanceFromCenter * Mathf.Sin(Mathf.Atan2(currOffset.y, currOffset.x) + rotationRadians);
                SeedOffsets[i] = new Vector2(newX, newY);
                FireShot(transform.position, new Vector2(transform.position.x + SeedOffsets[i].x, transform.position.y + SeedOffsets[i].y));
            }

            numShotsFired++;
            yield return new WaitForSeconds(secondsBetweenCircleAttacks);
        }


        StartCoroutine(FireSporeVolley());
    }

    void EndSporeVolley()
    {
        Firing = false;
        Fsm.ChangeState(PlantBossSeedState.Instance);
        StopCoroutine(FireSporeVolley());
    }

    public IEnumerator CircleAttack()
    {
        float initialOffset = Random.Range(0, 360);
        for (float x = 0; x < 360; x += degreesBetweenMissiles)
        {
            float rotationRadians = Mathf.PI * x / 180;
            Vector2 offset = bulletDistanceFromCenter * new Vector2(Mathf.Cos(rotationRadians), Mathf.Sin(rotationRadians));
            FireShot((Vector2)transform.position + offset, (Vector2)transform.position + 2 * offset);
        }
        yield return new WaitForSeconds(timeBetweenVolleys);

        StartCoroutine(CircleAttack());
    }

    // Refactor forrealsies.
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
