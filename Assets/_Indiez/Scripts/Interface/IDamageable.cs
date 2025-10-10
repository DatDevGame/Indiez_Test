using Unity.Mathematics;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amount, Vector3 hitPos);
}
