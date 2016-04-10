using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SpriteTile;

public class BossLevelWallBuilder : MonoBehaviour {

    private static float SecondsBetweenPlacements = .3f;
    private static int BackupSteps = 3;

    private List<Int2> DoorPositions { get; set; }

    public IEnumerator BuildWall()
    {
        // Disable inputs
        foreach(Int2 position in DoorPositions) {
            Tile.SetTile(position, new TileInfo());

            yield return new WaitForSeconds(SecondsBetweenPlacements);
        }
        // Re-enable inputs
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
        foreach(Int2 pos in DoorPositions)
        {
            Tile.SetTile(new Int2(pos.y, pos.x), 0, 2, 3, false);
        }
    }
}
