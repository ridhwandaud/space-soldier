using UnityEngine;
using System.Collections;

public class ChargeGun : Weapon
{
    private StackPool chargeShotPool;
    private GameObject currentShot;
    private bool charging = false;
    private float chargeStartTime;

    public ChargeGun(SkillTree skillTree)
        : base(skillTree)
    {
        chargeShotPool = GameObject.Find("ChargeShotPool").GetComponent<StackPool>();
    }

    public override bool Fire(Transform transform)
    {
        if (CanFire())
        {
            if (!charging)
            {
                currentShot = chargeShotPool.Pop();
                // Set as child of soldier at the correct offset.
                charging = true;
                chargeStartTime = Time.time;
            }
        }

        return false;
    }

    public int Release(Transform transform)
    {
        return 0;
    }

    public override int GetEnergyCost()
    {
        throw new System.NotImplementedException();
    }

    public override string GetName()
    {
        return "chargeGun";
    }
}
