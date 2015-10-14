using UnityEngine;
using System.Collections;

public class KirbyAttackingState : State<KirbyAI> {
    private static KirbyAttackingState instance;
    public static KirbyAttackingState Instance
    {
        get
        {
            instance = instance == null ? new KirbyAttackingState() : instance;
            return instance;
        }
    }
}
