﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SpriteTile;

public class BossLevelWallBuilder : MonoBehaviour {

    private static float SecondsBetweenPlacements = .3f;
    private static int BackupSteps = 3;
    private static int TriggerDistanceSquared = 40;
    private static float CameraEventPauseSeconds = 1;

    private List<Int2> DoorPositions { get; set; }
    private Transform BossTransform;
    private bool EventTriggered = false;

    public IEnumerator BuildWall()
    {
        yield return new WaitForSeconds(CameraEventPauseSeconds);

        foreach (Int2 pos in DoorPositions)
        {
            Tile.SetTile(new Int2(pos.y, pos.x), 0, 2, 3, true);

            yield return new WaitForSeconds(SecondsBetweenPlacements);
        }

        yield return new WaitForSeconds(CameraEventPauseSeconds);

        // Make this generic for all bosses.
        BossTransform.GetComponent<PlantBossAI>().Awakened = true;

        Camera.main.GetComponent<CameraControl>().UnloadCameraEvent();
    }

    public static void Initialize (Int2 startingPoint, int[,] world, BasicLevelAlgorithm.Direction dir)
    {
        BossLevelWallBuilder instance = Camera.main.gameObject.AddComponent<BossLevelWallBuilder>();
        instance.DoorPositions = new List<Int2>();
        bool verticalWall = dir.Equals(BasicLevelAlgorithm.Direction.Left) || dir.Equals(BasicLevelAlgorithm.Direction.Right);
        int dimensionLength = verticalWall ? world.GetLength(0) : world.GetLength(1);

        switch (dir)
        {
            case BasicLevelAlgorithm.Direction.Left:
                startingPoint.y += BackupSteps;
                startingPoint.x = 0;
                break;
            case BasicLevelAlgorithm.Direction.Right:
                startingPoint.y -= BackupSteps;
                startingPoint.x = 0;
                break;
            case BasicLevelAlgorithm.Direction.Up:
                startingPoint.y = 0;
                startingPoint.x -= BackupSteps;
                break;
            case BasicLevelAlgorithm.Direction.Down:
                startingPoint.y = 0;
                startingPoint.x += BackupSteps;
                break;
        }

        for (int offset = 0; offset < dimensionLength; offset++)
        {
            Int2 pos = verticalWall ? new Int2(startingPoint.x + offset, startingPoint.y)
                : new Int2(startingPoint.x, startingPoint.y + offset);
            if (world[pos.x, pos.y] != 2)
            {
                instance.DoorPositions.Add(pos);
            }
        }

        Debug.Log(instance.DoorPositions);
    }

    void Start()
    {
        BossTransform = (GameObject.FindGameObjectWithTag("Boss") as GameObject).transform;
    }

    void Update ()
    {
        if (!EventTriggered && (BossTransform.position - Player.PlayerTransform.position).sqrMagnitude < TriggerDistanceSquared)
        {
            EventTriggered = true;
            CameraControl.CameraFunction func = () => StartCoroutine(BuildWall());
            Int2 centerWallTile = DoorPositions[DoorPositions.Count / 2];
            Camera.main.GetComponent<CameraControl>().LoadCameraEvent(func, new Vector2(centerWallTile.y * GameSettings.TileSize,
                centerWallTile.x * GameSettings.TileSize));
        }
    }
}
