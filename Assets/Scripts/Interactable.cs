using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Config - Base")]
    [SerializeField] private bool onlyPlayerInteractable = true;
    [SerializeField] private bool interactOnCollide = true;

    protected virtual void Update()
    {
        
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // VALIDITY CHECK
        Character character = other.GetComponent<Character>();
        if (character == null)
            return;

        // ONLY PLAYER INTERACTABLE CHECK
        Player player = character as Player;
        if (player == null && onlyPlayerInteractable)
            return;

        // INTERACTION ON COLLIDE
        if (interactOnCollide)
        {
            Interact(character);
            return;
        }
    }

    protected virtual void Interact(Character character)
    {

    }
}
