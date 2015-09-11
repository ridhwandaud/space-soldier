using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public static GameObject PlayerObject;
    public static Transform PlayerTransform;
    public static PlayerHealth PlayerHealth;
    public static PlayerExperience PlayerExperience;

	void Awake () {
        PlayerObject = GameObject.Find("Soldier");
        PlayerTransform = PlayerObject.transform;
        PlayerHealth = PlayerObject.GetComponent<PlayerHealth>();
        PlayerExperience = PlayerObject.GetComponent<PlayerExperience>();
	}
}
