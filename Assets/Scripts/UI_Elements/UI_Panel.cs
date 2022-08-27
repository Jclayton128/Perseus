using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class UI_Panel : MonoBehaviour
{
    List<GameObject> elements = new List<GameObject>();
    protected UI_Controller uic;
    public UI_Controller.Context Context = UI_Controller.Context.None;
    public Action<bool> OnShowHidePanel;

    protected virtual void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.tag == "StartOff")
            {
                continue;
            }
            elements.Add(transform.GetChild(i).gameObject);
        }

        uic = FindObjectOfType<UI_Controller>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void ShowHideElements(bool shouldShow)
    {
        foreach (var elem in elements)
        {
            elem.SetActive(shouldShow);
        }
        OnShowHidePanel?.Invoke(shouldShow);
    }

    public virtual void Activate()
    {
        ShowHideElements(true);
    }

    public virtual void Deactivate()
    {
        ShowHideElements(false);
    }

}
