using UnityEngine;
interface ITarget
{
    void TakeDamage(float damage);
}
public class Target : MonoBehaviour, ITarget
{
    float health = 100;
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0)
            Die();

    }
    void Die()
    {
        // Animations to die
    }

}
