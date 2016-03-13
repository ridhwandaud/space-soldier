﻿using System.Collections.Generic;

public class GameSettings {
    public static float TileSize = 1f;
    public static int WallLayerNumber = 8;
    public static HashSet<int> WallIndices = new HashSet<int>() { 2 };
    public static float KnockbackVelocity = 7f;
    public static float KnockbackDuration = .05f;
}
