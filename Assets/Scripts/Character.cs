using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    protected Rigidbody2D RigidBody;
    protected Animator Animator;
    protected SpriteRenderer Renderer;

    protected Vector2 Velocity = Vector2.zero;
    private bool isFacingRight = true;
    protected int facingDirection = 1;
    private Material defaultMaterial;

    [Header("Stats")]
    [SerializeField] protected int maxHealth = 1;
    [SerializeField] protected int currentHealth = 0;
    [SerializeField] protected int damage = 1;

    [Header("Collision")]
    [SerializeField] private float groundCheckDistance = 1.39f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private bool isGrounded = false;

    [Header("Movement")]
    [SerializeField] protected float MoveSpeed = 8.0f;
    [SerializeField] private float JumpForce = 15.0f;

    [Header("States")]
    [SerializeField] protected bool canMove = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canAttack = true;
    [SerializeField] private bool isDead = false;
    [SerializeField] protected bool isKnockbackResistant = false;

    [Header("Attack Params")]
    [SerializeField] protected Transform attackPoint = null;
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] protected float attackRadius = 0.86f;
    [Space]
    [SerializeField] protected Vector2 knockbackStrength = new Vector2(6.0f, 5.0f);
    private Vector2 knockbackVelocity = Vector2.zero;
    protected bool isKnockbackActive = false;
    private float knockbackDuration = 0.2f;
    private float knockbackTimer = 0.0f;

    [Header("Damage Taken VFX")]
    [SerializeField] private Material GetDamagedMaterial;
    [SerializeField] private float materialApplyTime = 0.1f;
    private Coroutine onDamageVFXco;

    [Header("SFX")]
    [SerializeField] private AudioClip[] swingSFX = null;
    [SerializeField] private AudioClip[] walkSFX = null;
    [SerializeField] private AudioClip[] deathSFX = null;
    [SerializeField] private AudioClip[] getHitSFX = null;

    [Header("PowerUp Setting")]
    [SerializeField] private Material outlineMaterial;
    [SerializeField] protected PowerUpVisual powerUpPrefub;
    [SerializeField] private Transform powerUpsContainer;
    private List<PowerUpVisual> powerUps;

    protected virtual void Awake()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        Animator = GetComponentInChildren<Animator>();
        Renderer = Animator.GetComponent<SpriteRenderer>();

        defaultMaterial = Renderer.material;
        currentHealth = maxHealth;
        powerUps = new List<PowerUpVisual>();
    }

    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        HandleCollision();
        HandleAnimator();
        HandleFlipCharacter();
        HandleKnockbackTimer();
    }

    private void HandleKnockbackTimer()
    {
        if (isKnockbackActive)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0.0f)
            {
                knockbackTimer = 0.0f;
                Velocity.x = 0;
                isKnockbackActive = false;
                knockbackVelocity = Vector2.zero;
                EnableAnimator(true);
            }
        }
    }

    protected virtual void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    protected virtual void HandleAnimator()
    {
        Animator.SetBool("isGrounded", isGrounded);
        Animator.SetFloat("yVelocity", Velocity.y);
        Animator.SetFloat("xVelocity", Velocity.x);
    }

    public bool IsDead()
    {
        return isDead;
    }

    protected virtual void HandleFlipCharacter()
    {
        if (isKnockbackActive)
        {
            return;
        }

        if (Velocity.x < 0 && isFacingRight)
        {
            FlipCharacter();
        }
        else if (Velocity.x > 0 && !isFacingRight)
        {
            FlipCharacter();
        }
    }

    public void FlipCharacter()
    {
        transform.Rotate(0.0f, 180.0f, 0.0f);
        isFacingRight = !isFacingRight;
        facingDirection *= -1;
    }

    protected virtual void FixedUpdate()
    {
        CalculationMovement();
        SetMovement();
    }

    protected virtual void CalculationMovement()
    {
        if (knockbackTimer != 0.0f)
        {
            Velocity = knockbackVelocity;
        }
        else if (!canMove)
        {
            Velocity.x = 0;
            Velocity.y = RigidBody.linearVelocityY;
        }
        else
        {
            CalculationMovementNormal();
        }
    }

    protected virtual void CalculationMovementNormal()
    {
        Velocity.x = RigidBody.linearVelocityX;
        Velocity.y = RigidBody.linearVelocityY;
    }

    protected virtual void SetMovement()
    {
        RigidBody.linearVelocity = Velocity;
    }

    protected virtual void TryToAttack(InputAction.CallbackContext context)
    {
        if (isGrounded && canAttack)
        {
            Animator.SetTrigger("Attack");
        }
    }

    public virtual bool TakeDamage(int damage, Vector2 knockbackStrength, Vector2 direction)
    {
        DamageVFX();
        DamageSFX();
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            ApplyKnockback(knockbackStrength, direction);
        }
        return isDead;
    }

    private void DamageSFX()
    {
        if (getHitSFX != null && getHitSFX.Length > 0)
        {
            SoundManager.instance.PlayRandomSFX(getHitSFX);
        }
    }

    public void AttackFX()
    {
        AttackSFX();
    }

    private void AttackSFX()
    {
        if (swingSFX != null && swingSFX.Length > 0)
        {
            SoundManager.instance.PlayRandomSFX(swingSFX);
        }
    }

    public void MakeStep()
    {
        MakeStepSFX();
    }

    private void MakeStepSFX()
    {
        if (walkSFX != null && walkSFX.Length > 0)
        {
            SoundManager.instance.PlayRandomSFX(walkSFX);
        }
    }

    private void ApplyKnockback(Vector2 knockbackStrength, Vector2 direction)
    {
        if (isKnockbackResistant)
            return;

        EnableAnimator(false);
        knockbackVelocity = direction.normalized * knockbackStrength;
        knockbackTimer = knockbackDuration;
        isKnockbackActive = true;
    }

    protected virtual void Die()
    {
        knockbackTimer = 0.0f;
        DestroyPowerUps();
        ResetMaterial();
        EnableStunned(true);
        Animator.SetTrigger("Death");
        isDead = true;

        if (deathSFX != null && deathSFX.Length > 0)
        {
            SoundManager.instance.PlayRandomSFX(deathSFX);
        }
    }

    protected void DestroyPowerUps()
    {
        foreach (PowerUpVisual puv in powerUps)
        {
            Destroy(puv.gameObject);
        }
    }

    private void EnableStunned(bool enable)
    {
        EnableJump(!enable);
        EnableMovement(!enable);
        EnableAttacking(!enable);
    }

    public void EnableAttacking(bool enable)
    {
        canAttack = enable;
    }

    private void DamageVFX()
    {
        if (onDamageVFXco != null)
        {
            StopCoroutine(onDamageVFXco);
            ResetMaterial();
        }
        onDamageVFXco = StartCoroutine(DamageVFXCo());
    }

    private IEnumerator DamageVFXCo()
    {
        Renderer.material = GetDamagedMaterial;
        yield return new WaitForSeconds(materialApplyTime);
        ResetMaterial();
    }

    private void ResetMaterial()
    {
        Renderer.material = defaultMaterial;
    }

    public virtual void FindAttackCollisions()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsEnemy);
        foreach (Collider2D collider in colliders)
        {
            Character character = collider.GetComponent<Character>();
            if (!character.IsDead())
            {
                Vector2 direction = (character.transform.position - transform.position).normalized;
                character.TakeDamage(damage, knockbackStrength, direction);
            }
        }
    }

    protected virtual void TryToJump(InputAction.CallbackContext context)
    {
        if (isGrounded && canJump)
        {
            RigidBody.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
        }
    }

    public void EnableJump(bool enable)
    {
        canJump = enable;
    }

    public void EnableMovement(bool enable)
    {
        canMove = enable;
    }

    [ContextMenu("Kill")]
    public void Kill()
    {
        Die();
    }

    private void EnableAnimator(bool enable)
    {
        Animator.enabled = enable;
    }

    private bool PowerUpExists(Modificator modificator)
    {
        return powerUps.Any(powerUp => powerUp.GetModificator() == modificator);
    }

    public void AddPowerUp(Modificator mod)
    {
        if (PowerUpExists(mod))
        {
            PowerUpVisual puv = powerUps.Find(powerUp => powerUp.GetModificator() == mod);
            puv.IncreaseSize();
        }
        else
        {
            Vector3 pos = powerUpsContainer.position + new Vector3(0.0f, powerUps.Count / 2.0f, 0.0f);
            PowerUpVisual puv = Instantiate(powerUpPrefub, pos, Quaternion.identity);
            puv.transform.SetParent(powerUpsContainer, true);
            puv.SetModificator(mod);
            powerUps.Add(puv);
        }
    }

    public float GetHeight()
    {
        return Renderer.sprite.rect.size.y;
    }

    public void IncreaseSpeed(float speedIncrease)
    {
        MoveSpeed += speedIncrease;
        AddPowerUp(Modificator.SPEED);
    }

    public void IncreaseHealth(int healthIncrease)
    {
        maxHealth += healthIncrease;
        currentHealth = maxHealth;
        AddPowerUp(Modificator.HEALTH);
    }

    public void IncreaseDamage(int damageIncrease)
    {
        damage += damageIncrease;
        AddPowerUp(Modificator.ATTACK);
    }

    public void IncreaseKnockback(Vector2 knockbackIncrease)
    {
        knockbackStrength += knockbackIncrease;
        AddPowerUp(Modificator.KNOCKBACK);
    }

    public void IncreaseAttackSpeed(float multiplier)
    {
        Animator.speed *= multiplier;
        AddPowerUp(Modificator.ATTACK_SPEED);
    }

    public void EnableKnockbackResistance(bool enable)
    {
        isKnockbackResistant = enable;
        AddPowerUp(Modificator.KNOCKBACK_RES);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
