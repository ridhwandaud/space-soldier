using UnityEngine;
using System.Collections.Generic;

public interface ILevelGenerator {
    int[,] GenerateLevel(int levelIndex, out List<Vector2> openPositions, out Vector3 playerSpawn);
}
