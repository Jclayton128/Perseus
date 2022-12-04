using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingSpriteSwapper : MonoBehaviour
{
    SpriteRenderer _sr;

    [SerializeField] Sprite[] _spriteMenu = null;

    private void Awake()
    {
        _sr  = GetComponent<SpriteRenderer>();

        if (_spriteMenu.Length > 0)
        {
            int rand = UnityEngine.Random.Range(0,_spriteMenu.Length);
            _sr.sprite = _spriteMenu[rand];
        }

    
    }

}
