using UnityEngine;
using System.Collections.Generic;

public interface ILevelPopulator {

    void populateLevel(int levelIndex, List<Vector2> openPositions);
}
