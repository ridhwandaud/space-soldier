using System.Collections.Generic;

public class TileDictionaries {

    public static Dictionary<int, int> BarrierTileDictionary = new Dictionary<int, int>
    {
        {2, 26}, // SingleElevated
        {1012, 31}, // LowerLeftElevated
        {1112, 0}, // LowerElevated
        {1102, 34}, // LowerRightElevated
        {12, 22}, // LeftThinElevated
        {112, 23}, // HorizontalThinElevated
        {102, 24}, // RightThinElevated
        {1000, 33}, // SingleWall
        {1010, 36}, // LeftWall
        {1110, 37}, // MiddleWall
        {1100, 38}, // RightWall
        {1, 26}, // TopThinElevated
        {11, 22}, // TopLeftElevated
        {1101, 32}, // RightEdgeElevated
        {1011, 31}, // LeftEdgeElevated
        {1111, 0}, // Elevated
        {1001, 25}, // VerticalThinElevated - there are now different types for when it joins with stuff (35 vs. 37) - account for this later.
        {1002, 25}, // BottomThinElevated
        {111, 23}, // TopEdgeElevated
        {101, 24}, // TopRightElevated
        {1212, 51}, // LowerLeftElevatedNextToWall
        {1122, 50}, // RightEdgeElevatedNextToWall
        {1222, 25}, // BottomThinElevated
        {1221, 25}, // VerticalThinElevated
        {1211, 51}, // LowerLeftElevatedNextToWall
        {1121, 50}, // RightEdgeElevatedNextToWall
        {1020, 36}, // LeftWall
        {1021, 25}, // VerticalThinElevated
        {1022, 25}, // BottomThinElevated
        {1200, 38}, // RightWall
        {1201, 25}, // VerticalThinElevated
        {1202, 25}, // BottomThinElevated
        {1120, 37}, // MiddleWall
        {1210, 37}, // MiddleWall
        {121, 24}, // TopRightElevated
        {211, 22}, // TopLeftElevated
        {221, 25}, // TopThinElevated
        {222, 26}, // SingleElevated
        {202, 26}, // SingleElevated
        {212, 22}, // LeftThinElevated
        {201, 26}, // TopThinElevated
        {1220, 37}, // MiddleWall
        {21, 25}, // TopThinElevated
        {22, 26}, // SingleElevated
        {122, 24}, // RightThinElevated

        // TODO: These next ones are lone blocks. Make them into rocks or other obstacles.
        {0, 33},
        {10, 33},
        {100, 33},
        {110, 33},
        {120, 33},
        {200, 33},
        {20, 33},
        {210, 33},
        {220, 33}
    };

    public static Dictionary<int, GrassObj[]> DarkGrassDictionary = new Dictionary<int, GrassObj[]>
    {
        {0000, new GrassObj[] { new GrassObj(1, false) }},
        {0001, new GrassObj[] { new GrassObj(96, false) }},
        {0010, new GrassObj[] { new GrassObj(97, false) }},
        {0011, new GrassObj[] { new GrassObj (3, false) }},
        {0100, new GrassObj[] { new GrassObj(99, false) }},
        {0101, new GrassObj[] { new GrassObj (5, false) }},
        {0110, new GrassObj[] { new GrassObj(1, true) }},
        {0111, new GrassObj[] { new GrassObj (4, false), new GrassObj(1, true) }},
        {1000, new GrassObj[] { new GrassObj(98, false) }},
        {1001, new GrassObj[] { new GrassObj(1, true) }},
        {1010, new GrassObj[] { new GrassObj(15, false) }},
        {1011, new GrassObj[] { new GrassObj (11, false), new GrassObj(1, true) }},
        {1100, new GrassObj[] { new GrassObj(16, false) }},
        {1101, new GrassObj[] { new GrassObj (10, false), new GrassObj(1, true) }},
        {1110, new GrassObj[] { new GrassObj (2, false), new GrassObj(1, true) }},
        {1111, new GrassObj[] { new GrassObj(1, false) }},
    };

    public static Dictionary<int, int> ShoreDictionary = new Dictionary<int, int>
    {
        {0001, 100},
        {0010, 43},
        {0011, 43},
        {0100, 45},
        {0101, 45},
        {0110, 44},
        {0111, 44},
        {1000, 102},
        {2000, 104},
        {3000, 101},
        {4000, 103},
        {1001, 102},
        {2001, 104},
        {3001, 101},
        {4001, 103},
        {1010, 54},
        {2010, 54},
        {3010, 55},
        {4010, 55},
        {1011, 54},
        {2011, 54},
        {3011, 55},
        {4011, 55},
        {1100, 52},
        {2100, 56},
        {3100, 52},
        {4100, 56},
        {1101, 52},
        {2101, 56},
        {3101, 52},
        {4101, 56}
    };
}
