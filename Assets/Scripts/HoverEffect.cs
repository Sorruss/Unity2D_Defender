using UnityEngine;
using UnityEngine.EventSystems;

public enum Effect
{
    SCALE = 0,
}

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Effect effect = Effect.SCALE;
    [SerializeField] private float sizeMultiplier = 1.05f;
    [SerializeField] private float animationTimeIn = 0.2f;
    [SerializeField] private float animationTimeOut = 0.2f;
    [Space]
    [SerializeField] private AudioClip hoverSFX;

    private RectTransform Object;
    private Vector3 defaultScale;

    private void Awake()
    {
        Object = GetComponent<RectTransform>();
        defaultScale = Object.localScale;
    }

    private void Scale(AnimMode mode)
    {
        switch (mode)
        {
            case AnimMode.IN:
                LeanTween.scale(Object, defaultScale * sizeMultiplier, animationTimeIn).setIgnoreTimeScale(true);
                break;
            case AnimMode.OUT:
                LeanTween.scale(Object, defaultScale, animationTimeOut).setIgnoreTimeScale(true);
                break;
            default:
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.instance.PlaySFX(ref hoverSFX);
        switch (effect)
        {
            case Effect.SCALE: Scale(AnimMode.IN); break;
            default: break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RevertEffect();
    }

    private void RevertEffect()
    {
        switch (effect)
        {
            case Effect.SCALE: Scale(AnimMode.OUT); break;
            default: break;
        }
    }

    private void OnDisable()
    {
        RevertEffect();
    }
}
