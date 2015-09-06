using UnityEngine;
using System.Collections;

public class State<T> {
    protected GameObject player;

    public State()
    {
        player = GameObject.Find("Soldier");
    }

    public virtual void Execute(T enemy) {}
}
