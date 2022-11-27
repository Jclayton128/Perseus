using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScanReticleHandler : MonoBehaviour
{
    [SerializeField] GameObject _tabTip = null;

    private void Start()
    {
        //transform.DOScale(0.3f, 1f).SetEase(Ease.InOutElastic, 9999f);
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void ToggleTabTip(bool shouldShowTip)
    {
        _tabTip.SetActive(shouldShowTip);
    }
 
}
