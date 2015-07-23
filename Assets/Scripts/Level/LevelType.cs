using UnityEngine;
using System.Collections;

public class LevelType {
    private ILevelGenerator levelGenerator;
    private ILevelPopulator levelPopulator;

    public LevelType(ILevelGenerator levelGenerator, ILevelPopulator levelPopulator)
    {
        this.levelGenerator = levelGenerator;
        this.levelPopulator = levelPopulator;
    }

    public ILevelGenerator getLevelGenerator()
    {
        return levelGenerator;
    }

    public ILevelPopulator getLevelPopulator()
    {
        return levelPopulator;
    }
}
