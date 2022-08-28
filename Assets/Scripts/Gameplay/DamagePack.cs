using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamagePack
{
    public float NormalDamage;
    public float ShieldBonusDamage;
    public float IonDamage;
    public float KnockbackAmount;
    public float ScrapBonus;



    public DamagePack(float normalDamage, float shieldBonusDamage, float ionDamage,
        float knockbackAmount, float scrapBonus)
    {
        NormalDamage = normalDamage;
        ShieldBonusDamage = shieldBonusDamage;
        IonDamage = ionDamage;
        KnockbackAmount = knockbackAmount; 

        if (normalDamage > 0)
        {
            ScrapBonus = scrapBonus;
        }
        else
        {
            ScrapBonus = 0;
            //Scrap Bonus is zero when normal damage is zero to prevent exploitation
        }

    }
}
