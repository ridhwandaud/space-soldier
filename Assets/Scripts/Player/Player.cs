using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public static GameObject Instance;
    public static Transform PlayerTransform;
    public static PlayerHealth PlayerHealth;
    public static PlayerExperience PlayerExperience;

	void Awake () {
        Instance = gameObject;
        PlayerTransform = Instance.transform;
        PlayerHealth = Instance.GetComponent<PlayerHealth>();
        PlayerExperience = Instance.GetComponent<PlayerExperience>();
	}
}
