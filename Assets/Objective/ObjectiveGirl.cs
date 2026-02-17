using UnityEngine;

public class ObjectiveGirl : Character
{
    private Transform playerPos;
    public Vector3 diff;

    protected override void Awake()
    {
        base.Awake();
        playerPos = FindFirstObjectByType<Player>().transform;
        maxHealth = 10;
        currentHealth = maxHealth;
        isKnockbackResistant = true;
    }

    protected override void Update()
    {
        HandleAnimator();
        HandleFlipCharacter();
    }

    protected override void HandleAnimator()
    {
        diff = playerPos.position - transform.position;
        Animator.SetFloat("PlayerPos", diff.x);
    }

    protected override void Die()
    {
        base.Die();
        UI.instance.EnableGameOverUI(true, false);
    }

    protected override void OnDrawGizmos()
    {
    }
}
