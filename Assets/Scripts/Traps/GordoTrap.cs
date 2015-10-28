using UnityEngine;

public class GordoTrap : MonoBehaviour {
    public float triggerDelay;
    public int numGordosToSpawn;

    private bool triggered = false;
    private GameObject gordoObj;
    private Vector2 colliderSize;
    private Transform enemyContainer;

    void Awake()
    {
        gordoObj = Resources.Load("Gordo") as GameObject;
        colliderSize = gordoObj.GetComponent<BoxCollider2D>().size;
        enemyContainer = GameObject.Find("Enemies").transform;
    }

    void OnTriggerExit2D (Collider2D other) {
        if (!triggered && other.tag == "Player")
        {
            triggered = true;
            Invoke("SpawnGordos", triggerDelay);
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        triggered = false;
        CancelInvoke("SpawnGordos");
    }

    void SpawnGordos()
    {
        for (int x = 0; x < numGordosToSpawn; x++)
        {
            SpawnGordo();
        }

        Destroy(gameObject);
    }

    void SpawnGordo()
    {
        bool gordoSpawned = false;

        while (!gordoSpawned)
        {
            float xOffset = Random.Range(-5, 5) / 10f;
            float yOffset = Random.Range(-5, 5) / 10f;

            Vector2 spawnPos = new Vector2(transform.position.x + xOffset, transform.position.y + yOffset);

            if (Physics2D.BoxCast(spawnPos, colliderSize,
                0f, Vector2.zero, 0, LayerMasks.WallLayerMask).collider == null)
            {
                // Do not count gordo traps in the total enemy count.
                GameObject spawnedGordo = Instantiate(gordoObj, spawnPos, Quaternion.identity) as GameObject;
                gordoSpawned = true;
                spawnedGordo.GetComponent<EnemyAI>().chasing = true;
                spawnedGordo.transform.SetParent(enemyContainer);
            }
        }

        if (GameState.NumEnemiesRemaining > 0)
        {
            GameState.NumEnemiesRemaining++;
        }
    }
}
