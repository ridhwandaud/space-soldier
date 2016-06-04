using UnityEngine;
using System.Collections;

public class BearAI : MonoBehaviour {
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

	void Update()
    {
        float xOffset = transform.position.x - Player.PlayerTransform.position.x;
        if (xOffset > 0)
        {
            animator.SetFloat("ToPlayer", 0);
        } else if (xOffset < 0)
        {

            animator.SetFloat("ToPlayer", 1);
        }
    }
}
