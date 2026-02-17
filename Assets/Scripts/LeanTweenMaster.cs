using System;
using System.Collections;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum AnimType
{
    SCALE = 0,
}

public enum AnimMode
{
    IN = 0,
    OUT
}

public class LeanTweenMaster : MonoBehaviour
{
    [Header("Animation IN")]
    [SerializeField] private AnimType AnimationIn = AnimType.SCALE;
    [SerializeField] private LeanTweenType inEase = LeanTweenType.linear;
    [SerializeField] private float inTime = 1.0f;
    [SerializeField] private float inDelay = 0.0f;

    [Header("Animation OUT")]
    [SerializeField] private AnimType AnimationOut = AnimType.SCALE;
    [SerializeField] private LeanTweenType outEase = LeanTweenType.linear;
    [SerializeField] private float outTime = 1.0f;
    [SerializeField] private float outDelay = 0.0f;

    [Header("Settings")]
    [SerializeField] private bool CloseAutomatically = false;

    private RectTransform Object;

    private void Awake()
    {
        Object = GetComponent<RectTransform>();
    }

    private void SelectAnimationIn()
    {
        switch (AnimationIn)
        {
            case AnimType.SCALE: AnimationScale(AnimMode.IN); break;
            default: break;
        }
    }

    private void SelectAnimationOut()
    {
        switch (AnimationOut)
        {
            case AnimType.SCALE: AnimationScale(AnimMode.OUT); break;
            default: break;
        }
    }

    private void AnimationScale(AnimMode mode)
    {
        if (mode == AnimMode.IN)
        {
            Object.localScale = Vector3.zero;
            if (!CloseAutomatically)
            {
                LeanTween.scale(Object, Vector3.one, inTime).setDelay(inDelay).setEase(inEase);
            }
            else
            {
                LeanTween.scale(Object, Vector3.one, inTime).setDelay(inDelay).setEase(inEase).setOnComplete(SelectAnimationOut);
            }
        }
        else
        {
            Object.localScale = Vector3.one;
            LeanTween.scale(Object, Vector3.zero, outTime).setDelay(outDelay).setEase(outEase);
            if (CloseAutomatically)
            {
                Invoke("DisableObject", outTime + outDelay);
            }
        }
    }

    private void DisableObject()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SelectAnimationIn();
    }

    public void OnDisable()
    {
    }
}
