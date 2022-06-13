using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemCrateHandler : MonoBehaviour
{
    [SerializeField] SpriteRenderer _iconSprite = null;
    public GameObject SystemChunk;
    public Vector2 LocalPosition;

    private void Start()
    {
        _iconSprite.sprite = SystemChunk.GetComponent<SystemIconHandler>().GetIcon();
    }

}
