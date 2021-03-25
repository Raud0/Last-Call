using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class FudgeHeight : MonoBehaviour
{
    private RectTransform myTransform;
    private RectTransform childTransform;
    private void Awake()
    {
        myTransform = GetComponent<RectTransform>();
        childTransform = null;
        GetChildRectangle();
    }

    private bool GetChildRectangle()
    {
        Option option = GetComponentInChildren<Option>();
        if (option == null) return false;
        
        childTransform = option.gameObject.GetComponent<RectTransform>();
        return true;
    }
    
    private void Update()
    {
        if (childTransform == null && !GetChildRectangle()) return;
        myTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,-childTransform.rect.y);
    }
}
