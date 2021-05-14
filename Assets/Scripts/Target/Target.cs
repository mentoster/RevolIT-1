using System;
using System.Diagnostics;
using UnityEngine;
interface ITarget
{
    Tuple<bool, float> TakeDamage(float damage);
}
public class Target : MonoBehaviour, ITarget
{
    float health = 100;
    public Tuple<bool, float> TakeDamage(float damage)
    {
        print($"14. {gameObject} [{health}] -> take damage : {damage} -> now have {health - damage}");
        health -= damage;
        if (health < 0)
        {
            Die();
            return Tuple.Create(true, 0);
        }
        return Tuple.Create(false, damage);
    }
    void Die()
    {
        Destroy(gameObject);
    }

}
