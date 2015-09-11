using UnityEngine;
using System.Collections;

public class State<T> {
    public virtual void Execute(T enemy) {}
}
