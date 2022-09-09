using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageReflexable
{
    /// <summary>
    /// This gives a system a chance to modify a received damage pack before it is received by
    /// the entity's health system. Return TRUE if the system does modify incoming damage (ie, activates),
    /// or FALSE if it does not modify damage (ie, fails to activate).
    /// </summary>
    /// <param name="receivedDamagePack"></param>
    /// <returns></returns>
    public bool ModifyDamagePack(DamagePack receivedDamagePack);

    public void ExecuteDamageReflex();

}
