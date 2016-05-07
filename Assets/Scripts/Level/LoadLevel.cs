using UnityEngine;
using System.Collections.Generic;
using SpriteTile;

public class LoadLevel : MonoBehaviour {
    public static HashSet<int> FloorIndices = new HashSet<int>() {BasicLevelDecorator.BaseDark, BasicLevelDecorator.BaseLight};
    public static bool IsFirstLoad = true;

    public static bool TestingCityLevel = true;

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

        if (TestingCityLevel)
        {
            AStar.world = new CityGenerator().GenerateLevel(GameState.LevelIndex, out playerSpawn); 
        } else
        {
            AStar.world = new BasicLevelGenerator().GenerateLevel(GameState.LevelIndex, out playerSpawn);
            Tile.SetColliderLayer(GameSettings.WallLayerNumber, BasicLevelDecorator.CliffTileLayer);
            Tile.SetColliderLayer(GameSettings.WaterLayer, BasicLevelDecorator.WaterTileLayer);
        }

        player.transform.position = playerSpawn;
    }

    private bool hasAdjacentFloor(int[,] level, int x, int y)
    {
        return (x < level.GetLength(0) - 1 && FloorIndices.Contains(level[x + 1, y]))
            || (x > 0 && FloorIndices.Contains(level[x - 1, y]))
            || (y < level.GetLength(1) - 1 && FloorIndices.Contains(level[x, y + 1]))
            || (y > 0 && FloorIndices.Contains(level[x, y - 1]));
    }

    // Debug code
    void OnDrawGizmos()
    {
        return;
        foreach (CityGenerator.PerimeterRect r in CityGenerator.PerimeterRects)
        {
            foreach (CityGenerator.PerimeterPoint p in r.points)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(new Vector2(p.x, p.y), new Vector3(1, 1, 1));
            }
        }
    }
}
