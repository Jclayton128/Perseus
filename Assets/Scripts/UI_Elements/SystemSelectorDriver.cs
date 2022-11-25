using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  

public class SystemSelectorDriver : MonoBehaviour
{
    RectTransform _rt;
    [SerializeField] SystemIconDriver _associatedIconDriver = null;

    //settings
    float _traverseAmount = 50f;
    float _traverseTime = 0.7f; // should be same as upgrade wings
    [SerializeField] bool _deploysDown = true;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();    
    }

    public void DeploySelector()
    {
        if (!_deploysDown)
        {
            _rt.DOAnchorPosY(_traverseAmount, _traverseTime).SetEase(Ease.InOutQuad).SetUpdate(true); ;
        }
        else
        {
            _rt.DOAnchorPosY(-_traverseAmount, _traverseTime).SetEase(Ease.InOutQuad).SetUpdate(true); ;
        }
    }

    public void RetractSelector()
    {
        if (!_deploysDown)
        {
            _rt.DOAnchorPosY(-_traverseAmount, _traverseTime).SetEase(Ease.InOutQuad).SetUpdate(true); ;
        }
        else
        {
            _rt.DOAnchorPosY(_traverseAmount, _traverseTime).SetEase(Ease.InOutQuad).SetUpdate(true); ;
        }
    }

    public void RetractSelectorWhilePaused()
    {
        if (!_deploysDown)
        {
            _rt.DOAnchorPosY(-_traverseAmount, _traverseTime).SetEase(Ease.InOutQuad).SetUpdate(true);
        }
        else
        {
            _rt.DOAnchorPosY(_traverseAmount, _traverseTime).SetEase(Ease.InOutQuad).SetUpdate(true);
        }
    }

    public void HandleSelect()
    {
        if (_associatedIconDriver)
        {
            _associatedIconDriver.PushHeldSystemWeaponAsSelection();
        }
    }

}
