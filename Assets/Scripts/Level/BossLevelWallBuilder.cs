using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SpriteTile;

public class BossLevelWallBuilder : MonoBehaviour {

    private static float SecondsBetweenPlacements = .3f;
    private static int BackupSteps = 3;
    private static int TriggerDistanceSquared = 60;
    private static float CameraEventPauseSeconds = 1;
    private static float StartWaitTime = .2f;
    private static float EndWaitTime = 1;
    private static float DampTime = .2f;
    private static float FocusHeadWindow = .6f;
    private static float FocusTailWindow = .5f;

    private List<Int2> DoorPositions { get; set; }
    private Transform BossTransform;
    private bool EventTriggered = false;

    public IEnumerator BuildWall()
    {
        for (int x = 0; x < DoorPositions.Count; x++)
        {
            Int2 pos = DoorPositions[x];
            Tile.SetTile(new Int2(pos.y, pos.x), BasicLevelDecorator.CliffTileLayer, 3, 33, true);

            if (x < DoorPositions.Count - 1)
            {
                yield return new WaitForSeconds(SecondsBetweenPlacements);
            }
        }

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
            if (world[pos.x, pos.y] < 2)
            {
                instance.DoorPositions.Add(pos);
            }
        }
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
            Int2 centerWallTile = DoorPositions[DoorPositions.Count / 2];
            Camera.main.GetComponent<CameraControl>().LoadCameraEvent(
                () => StartCoroutine(BuildWall()), 
                () => BossTransform.GetComponent<PlantBossAI>().Awakened = true,
                StartWaitTime,
                EndWaitTime,
                new Vector2(centerWallTile.y * GameSettings.TileSize,
                    centerWallTile.x * GameSettings.TileSize),
                DampTime,
                FocusHeadWindow,
                FocusTailWindow);
        }
    }
}
