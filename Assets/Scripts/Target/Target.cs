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
        health -= damage;
        if (health < 0)
        {
            Die();
            return Tuple.Create(true, damage);
        }
        return Tuple.Create(false, damage);
    }
    void Die()
    {
        Destroy(gameObject);
    }

}
