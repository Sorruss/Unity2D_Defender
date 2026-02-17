using UnityEngine;

public class Skeleton : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        maxHealth = 1;
        currentHealth = maxHealth;
    }
}
