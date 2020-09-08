using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void DecreaseHealth(int damage);

    void IncreaseHealth(int amount);
    void Die();
}
