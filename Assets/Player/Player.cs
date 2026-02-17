using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class Player : Character
{
    private InputSA_Player PlayerInput;
    private InputAction Movement;
    private InputAction JumpAction;
    private InputAction AttackAction;
    private InputAction PauseAction;
    private InputAction ControlsAction;

    private int killCount = 0;

    protected override void Awake()
    {
        base.Awake();

        PlayerInput = new InputSA_Player();
        Movement = PlayerInput.Default.Movement;
        JumpAction = PlayerInput.Default.Jump;
        AttackAction = PlayerInput.Default.Attack;
        PauseAction = PlayerInput.Default.Pause;
        ControlsAction = PlayerInput.Default.Controls;

        maxHealth = 10;
        currentHealth = maxHealth;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        InputUser.onChange += OnInputChange;

        Movement.Enable();
        JumpAction.Enable();
        JumpAction.performed += TryToJump;
        AttackAction.Enable();
        AttackAction.performed += TryToAttack;
        PauseAction.Enable();
        PauseAction.performed += OnPauseAction;
        ControlsAction.Enable();
        ControlsAction.performed += OnControlsAction;
    }

    private void OnControlsAction(InputAction.CallbackContext context)
    {
        UI.instance.ShowControls(!UI.instance.isControlsShown);
    }

    private void OnPauseAction(InputAction.CallbackContext context)
    {
        UI.instance.PauseGame();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        InputUser.onChange -= OnInputChange;

        Movement.Disable();
        JumpAction.performed -= TryToJump;
        JumpAction.Disable();
        AttackAction.performed -= TryToAttack;
        AttackAction.Disable();
        PauseAction.Disable();
        ControlsAction.Disable();
    }

    protected override void CalculationMovementNormal()
    {
        Velocity.x = (Movement.ReadValue<Vector2>().x * MoveSpeed);
        Velocity.y = RigidBody.linearVelocityY;
    }

    public override void FindAttackCollisions()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsEnemy);
        foreach (Collider2D collider in colliders)
        {
            Character character = collider.GetComponent<Character>();
            if (!character.IsDead())
            {
                Vector2 direction = (character.transform.position - transform.position).normalized;
                if (character.TakeDamage(damage, knockbackStrength, direction))
                {
                    UI.instance.UpdateKillCountText(IncrementKillCount());
                }
            }
        }
    }

    private int IncrementKillCount() => ++killCount;
    private void ResetKillCount() => killCount = 0;
    public int GetKillCount() => killCount;

    protected override void Die()
    {
        base.Die();
        ResetKillCount();
        DestroyPowerUps();
        UI.instance.EnableGameOverUI(true, false);
    }

    private void OnInputChange(InputUser user, InputUserChange change, InputDevice device)
    {
        if (change == InputUserChange.ControlSchemeChanged)
        {
            
        }
    }
}
