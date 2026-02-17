using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Character
{
    [Header("Drop Config")]
    [SerializeField] private float dropPowerUpChance = 1.0f;

    private bool isTargetInReach = false;

    protected override void Awake()
    {
        base.Awake();
        MoveSpeed = 4.0f;
    }

    protected override void HandleAnimator()
    {
        Animator.SetFloat("xVelocity", Velocity.x);
        if (isTargetInReach)
        {
            Animator.SetTrigger("Attack");
        }
    }

    protected override void SetMovement()
    {
        if (isTargetInReach && !isKnockbackActive)
        {
            Velocity = Vector2.zero;
        }
        RigidBody.linearVelocity = Velocity;
    }

    protected override void CalculationMovementNormal()
    {
        Velocity.x = facingDirection * MoveSpeed;
        Velocity.y = RigidBody.linearVelocityY;
    }

    protected override void HandleCollision()
    {
        base.HandleCollision();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsEnemy);
        bool anyAlive = colliders.Any(collider => !collider.GetComponent<Character>().IsDead());
        isTargetInReach = colliders.Length != 0 && anyAlive;
    }

    protected override void Die()
    {
        base.Die();
        DropPowerUp();
        Destroy(gameObject, 5);
    }

    private void DropPowerUp()
    {
        float chanceRoll = Random.Range(0.0f, 1.0f);
        if (chanceRoll > dropPowerUpChance)
            return;

        Modificator randomModificator = (Modificator)Random.Range(1, 7);

        PowerUpVisual puv = Instantiate(powerUpPrefub, transform.position, Quaternion.identity);
        InteractablePowerUp interactablePUV = puv.AddComponent<InteractablePowerUp>();
        puv.SetModificator(randomModificator);
        interactablePUV.powerUp = puv;
        puv.EnableCollider(true);
        puv.gameObject.layer = 0;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}
