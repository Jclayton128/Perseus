using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class SystemSelectorDriver : MonoBehaviour
{
    RectTransform _rt;
    [SerializeField] SystemIconDriver _associatedIconDriver = null;

    //settings
    float _traverseAmount = 50f;
    float _traverseTime = 0.7f; // should be same as upgrade wings
    [SerializeField] bool _deploysDown = true;
    [SerializeField] string _selectText = "Select";
    [SerializeField] string _emptyText = "";

    //state
    Button _button;
    TextMeshProUGUI _tmp;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>(); 
        _button = GetComponentInChildren<Button>();
        _tmp = GetComponentInChildren<TextMeshProUGUI>();
    }

    //private void Start()
    //{
    //    _associatedIconDriver.IconCleared += HandleIconCleared;
    //    _associatedIconDriver.IconSet += HandleIconSet;
    //}

    private void HandleIconCleared()
    {
        _button.interactable = false;
    }

    private void HandleIconSet()
    {
        _button.interactable = true;
    }

    public void DeploySelector()
    {
        _button.interactable = _associatedIconDriver.IsOccupied;
        if (_associatedIconDriver.IsOccupied)
        {
            _tmp.text = _selectText;
        }
        else
        {
            _tmp.text = _emptyText;
        }

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
