using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerLibrary : MonoBehaviour
{
    public static int PlayerEnemyNeutralLayerMask =>
        (1 << PlayerLayer) | (1 << EnemyLayer) | (1 << NeutralLayer) | (1 << SpecialEnemyLayer);
    public static int PlayerLayerMask => (1 << PlayerLayer);
    public static int EnemyLayerMask => (1 << EnemyLayer) | (1 << SpecialEnemyLayer);

    public static int PlayerEnemyLayerMask => PlayerLayerMask | EnemyLayerMask;
    public static int EnemyNeutralLayerMask =>
        (1 << EnemyLayer) | (1 << NeutralLayer) | (1 << SpecialEnemyLayer);

    public static int NeutralLayerMask => (1 << NeutralLayer);

    public static int PlayerLayer { get; } = 7;
    public static int EnemyLayer { get; } = 9;
    public static int NeutralLayer { get; } = 11;

    public static int SpecialEnemyLayer { get; } = 20;
}
