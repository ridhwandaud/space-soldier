using UnityEngine;
using System.Collections;
using System;

public class EnemySpawnData {

    public int min;
    public int max;
    public GameObject enemyType;

    public EnemySpawnData(int min, int max, GameObject enemyType)
    {
        this.min = min;
        this.max = max;
        this.enemyType = enemyType;
    }
}
