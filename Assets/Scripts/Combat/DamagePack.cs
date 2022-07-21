using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamagePack
{
    public float NormalDamage;
    public float ShieldDamage;
    public float IonDamage;
    public float KnockbackAmount;
    public float ScrapBonus;



    public DamagePack(float normalDamage, float shieldDamage, float ionDamage,
        float knockbackAmount, float scrapBonus)
    {
        NormalDamage = normalDamage;
        ShieldDamage = shieldDamage;
        IonDamage = ionDamage;
        KnockbackAmount = knockbackAmount; 
        ScrapBonus = scrapBonus;
    }
}
