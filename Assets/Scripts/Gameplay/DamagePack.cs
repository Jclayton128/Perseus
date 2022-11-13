using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePack : object
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

    public DamagePack(DamagePack dpToClone)
    {
        NormalDamage = dpToClone.NormalDamage;
        ShieldBonusDamage = dpToClone.ShieldBonusDamage;
        IonDamage = dpToClone.IonDamage;
        KnockbackAmount = dpToClone.KnockbackAmount;

        if (dpToClone.NormalDamage > 0)
        {
            ScrapBonus = dpToClone.ScrapBonus;
        }
        else
        {
            ScrapBonus = 0;
            //Scrap Bonus is zero when normal damage is zero to prevent exploitation
        }
    }

    public void NullifyDamage()
    {
        NormalDamage = 0;
        ShieldBonusDamage = 0;
        IonDamage = 0;
        KnockbackAmount = 0;
        ScrapBonus = 0;
    }

    public void FadeDamage(float fadeFactor)
    {
        NormalDamage *= fadeFactor;
        ShieldBonusDamage *= fadeFactor;
        IonDamage *= fadeFactor;
        KnockbackAmount *= fadeFactor;
        ScrapBonus *= fadeFactor;
    }
}
