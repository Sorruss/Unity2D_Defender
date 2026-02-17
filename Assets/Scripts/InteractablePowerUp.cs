using System.Collections;
using UnityEngine;

public class InteractablePowerUp : Interactable
{
    [Header("Info - PowerUp")]
    public PowerUpVisual powerUp;

    [Header("Spawn Config")]
    [SerializeField] private bool goUpOnSpawn = true;
    [SerializeField] private float goUpHeight = 2.0f;
    [SerializeField] private float launchSpeed = 0.3f;
    [SerializeField] private float fallingSpeed = 0.4f;
    [SerializeField] private float distanceToHitEnvironment = 0.001f;

    private Vector3 velocityThing;
    private bool shouldGoDown = false;

    protected override void Update()
    {
        base.Update();

        if (goUpOnSpawn)
        {
            // 1. Launch this gameObject into the air
            if (!shouldGoDown && transform.position.y < goUpHeight - 0.1f)
            {
                Vector3 targetDestination = new Vector3(transform.position.x, goUpHeight, transform.position.z);
                transform.position = Vector3.SmoothDamp(transform.position, targetDestination, ref velocityThing, launchSpeed);
                return;
            }

            shouldGoDown = true;

            // 2. When it's at the goUpHeight, it should start going down
            // 3. Go down till it hits any of environment layers
            bool hitTheGround = Physics2D.Raycast(transform.position, Vector2.down, distanceToHitEnvironment, powerUp.environmentLayers);
            if (shouldGoDown && !hitTheGround)
            {
                Vector3 targetDestination = new Vector3(transform.position.x, -2.0f, transform.position.z);
                transform.position = Vector3.SmoothDamp(transform.position, targetDestination, ref velocityThing, fallingSpeed);
                return;
            }

            goUpOnSpawn = false;
        }
    }

    protected override void Interact(Character character)
    {
        base.Interact(character);
        powerUp.EnableCollider(false);

        character.AddPowerUp(powerUp.modificator);
        WaveManager.instance.ApplyModificator(character, powerUp.modificator, true);
        Destroy(gameObject);
    }
}
