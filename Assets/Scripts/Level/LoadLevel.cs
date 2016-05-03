﻿using UnityEngine;
using System.Collections.Generic;
using SpriteTile;

public class LoadLevel : MonoBehaviour {
    public static HashSet<int> FloorIndices = new HashSet<int>() {BasicLevelDecorator.BaseDark, BasicLevelDecorator.BaseLight};
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

        //BasicLevelGenerator generator = new BasicLevelGenerator();
        ILevelGenerator generator = new CityGenerator();
        int[,] generatedLevel = generator.GenerateLevel(GameState.LevelIndex, out playerSpawn);
        //Tile.SetColliderLayer(GameSettings.WallLayerNumber, BasicLevelDecorator.CliffTileLayer);
        //Tile.SetColliderLayer(GameSettings.WaterLayer, BasicLevelDecorator.WaterTileLayer);
        player.transform.position = playerSpawn;

        AStar.world = generatedLevel;
    }

    private bool hasAdjacentFloor(int[,] level, int x, int y)
    {
        return (x < level.GetLength(0) - 1 && FloorIndices.Contains(level[x + 1, y]))
            || (x > 0 && FloorIndices.Contains(level[x - 1, y]))
            || (y < level.GetLength(1) - 1 && FloorIndices.Contains(level[x, y + 1]))
            || (y > 0 && FloorIndices.Contains(level[x, y - 1]));
    }
}
