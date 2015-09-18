using UnityEngine;
using System.Collections.Generic;

public interface ILevelGenerator {
    int[,] GenerateLevel(int levelIndex, out Vector3 playerSpawn);
}
