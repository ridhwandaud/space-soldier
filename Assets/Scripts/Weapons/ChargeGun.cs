using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChargeGun : Weapon
{
    private StackPool chargeShotPool;
    private GameObject currentShot;
    private Transform playerTransform;

    private bool charging = false;
    private float chargeStartTime;
    private float bulletSpeed = 15;

    private List<float> thresholds = new List<float> { .2f, .8f };
    private List<string> animationLevels = new List<string> { "SmallShotFired", "MediumShotFired", "LargeShotFired" };

    private Vector3 chargeShotRelativePosition = new Vector3(.18f, 1.082f, 0);

    public ChargeGun(SkillTree skillTree)
        : base(skillTree)
    {
        chargeShotPool = GameObject.Find("ChargeBlastPool").GetComponent<StackPool>();
        playerTransform = GameObject.Find("Soldier").transform;
    }

    public override int Click(Transform transform)
    {
        if (CanFire())
        {
            if (!charging)
            {
                currentShot = chargeShotPool.Pop();
                currentShot.transform.SetParent(playerTransform);
                currentShot.transform.localPosition = chargeShotRelativePosition;
                currentShot.transform.localRotation = Quaternion.Euler(0, 0, 90);

                charging = true;
                chargeStartTime = Time.time;

                currentShot.SetActive(true);
            }
        }

        return 0;
    }

    public override int Release(Transform transform)
    {
        ChargeBlastProperties chargeBlastProperties = currentShot.GetComponent<ChargeBlastProperties>();

        charging = false;
        float chargeDuration = Time.time - chargeStartTime;
        int chargeLevel = getChargeLevel(chargeDuration);
        chargeBlastProperties.ChargeLevel = chargeLevel;
        chargeBlastProperties.Fired = true;

        currentShot.transform.SetParent(chargeShotPool.transform);
        currentShot.GetComponent<Animator>().SetTrigger(getShotAnimationTrigger(chargeLevel));

        Vector3 mouse = Input.mousePosition;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 direction = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y).normalized;

        currentShot.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

        return 0;
    }

    private string getShotAnimationTrigger(int chargeLevel)
    {
        return animationLevels[chargeLevel];
    }

    private int getChargeLevel(float chargeTime)
    {
        for (int x = 0; x < thresholds.Count; x++)
        {
            if (chargeTime < thresholds[x])
            {
                return x;
            }
        }

        return thresholds.Count;
    }

    public override int GetEnergyRequirement()
    {
        return 0;
    }

    public override string GetName()
    {
        return "chargeGun";
    }
}
