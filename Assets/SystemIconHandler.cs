using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemIconHandler : MonoBehaviour
{
    [SerializeField] Sprite _icon = null;


    public Sprite GetIcon()
    {
        if (_icon == null)
        {
            _icon = GetComponent<SpriteRenderer>().sprite;
        }
        return _icon;
    }
}
