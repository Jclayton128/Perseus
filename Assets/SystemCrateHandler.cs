using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemCrateHandler : MonoBehaviour
{
    [SerializeField] SpriteRenderer _iconSprite = null;
    public GameObject SystemOrWeaponChunk;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        _iconSprite.sprite = SystemOrWeaponChunk.GetComponent<SystemHandler>().GetIcon();
    }

}
