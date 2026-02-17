using UnityEngine;

public enum Modificator
{
    NONE = 0,
    HEALTH,
    SPEED,
    ATTACK,
    KNOCKBACK,
    KNOCKBACK_RES,
    ATTACK_SPEED,
}

public class PowerUpVisual : MonoBehaviour
{
    private SpriteRenderer Renderer;
    public Modificator modificator;
    public LayerMask environmentLayers;
    private BoxCollider2D interactableCollider;

    private void Awake()
    {
        Renderer = GetComponentInChildren<SpriteRenderer>();
        interactableCollider = GetComponent<BoxCollider2D>();
    }

    public void SetModificator(Modificator modificator)
    {
        this.modificator = modificator;
        SetColor();
    }

    public Modificator GetModificator()
    {
        return modificator;
    }

    public void IncreaseSize()
    {
        Renderer.transform.localScale *= 1.05f;
    }

    private void SetColor()
    {
        switch (modificator)
        {
            case Modificator.NONE:          Renderer.color = Color.white; break;
            case Modificator.HEALTH:        Renderer.color = Color.green; break;
            case Modificator.SPEED:         Renderer.color = Color.blue; break;
            case Modificator.ATTACK:        Renderer.color = Color.red; break;
            case Modificator.KNOCKBACK:     Renderer.color = Color.cyan; break;
            case Modificator.KNOCKBACK_RES: Renderer.color = Color.aliceBlue; break;
            case Modificator.ATTACK_SPEED:  Renderer.color = Color.yellow; break;
            default: break;
        }
    }

    public void EnableCollider(bool enable)
    {
        interactableCollider.enabled = enable;
    }
}
