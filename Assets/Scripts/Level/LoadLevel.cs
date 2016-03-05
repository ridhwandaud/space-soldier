using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;

public class LoadLevel : MonoBehaviour {
    public static HashSet<int> WallIndices = new HashSet<int>() {2};
    public static HashSet<int> FloorIndices = new HashSet<int>() {0, 1};
    public static bool IsFirstLoad = true;

    public static LoadLevel instance = null;

    void Awake ()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        if (IsFirstLoad)
        {
            InitLevel();
        }
    }

    void Start()
    {
        IsFirstLoad = false;
    }

    void OnLevelWasLoaded(int index)
    {
        if (!IsFirstLoad)
        {
            InitLevel();
        }
    }

    void InitLevel()
    {
        Vector3 playerSpawn;
        GameObject player = GameObject.Find("Soldier");

        Tile.SetCamera();

        BasicLevelGenerator generator = new BasicLevelGenerator();
        int[,] generatedLevel = generator.GenerateLevel(GameState.LevelIndex, out playerSpawn);
        StartCoroutine(ConfigureColliders());
        player.GetComponent<Rigidbody2D>().position = playerSpawn;

        AStar.world = generatedLevel;
    }

    public static IEnumerator ConfigureColliders()
    {
        yield return new WaitForEndOfFrame();

        PolygonCollider2D[] polygonColliders = GameObject.Find("SpriteTileColliders").GetComponentsInChildren<PolygonCollider2D>();
        foreach (PolygonCollider2D collider in polygonColliders)
        {
            collider.tag = "Wall";
        }

        GameState.WallCollidersInitialized = true;
    }

    private bool hasAdjacentFloor(int[,] level, int x, int y)
    {
        return (x < level.GetLength(0) - 1 && FloorIndices.Contains(level[x + 1, y]))
            || (x > 0 && FloorIndices.Contains(level[x - 1, y]))
            || (y < level.GetLength(1) - 1 && FloorIndices.Contains(level[x, y + 1]))
            || (y > 0 && FloorIndices.Contains(level[x, y - 1]));
    }
}
