using UnityEngine;

public class CharacterAnimatorEvents : MonoBehaviour
{
    private Character character;

    private void Awake()
    {
        character = GetComponentInParent<Character>();
    }

    private void AttackStart()
    {
        character.EnableMovement(false);
    }

    private void AttackEnd()
    {
        character.EnableMovement(true);
    }

    private void AttackHit()
    {
        character.FindAttackCollisions();
        character.AttackFX();
    }

    private void Step()
    {
        character.MakeStep();
    }
}
