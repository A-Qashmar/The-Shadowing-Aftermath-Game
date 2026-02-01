using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int maxHealth, currentHealth;

    public UnityEvent<GameObject> onHitWithReference, onDeathWithReference;

    [SerializeField]
    private bool isDead = false;
    public Animator animator;

    [SerializeField]
    private float deathDestroyDelay = 3f;

    public bool IsDead => isDead;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;



    public void initiakizeHealth(int healthValue)
    {
        maxHealth = healthValue;
        currentHealth = maxHealth;
        isDead = false;
    }

    public void getHit(int damage, GameObject source)
    {
        if (isDead)
            return;

        // Prevent friendly fire (same layer)
        if (source.layer == gameObject.layer)
            return;

        currentHealth -= damage;

        // Play hit animation
        animator.SetTrigger("hit");

        if (currentHealth > 0)
        {
            onHitWithReference?.Invoke(source);
        }
        else
        {
            isDead = true;

            // Play death animation
            animator.SetTrigger("death");

            onDeathWithReference?.Invoke(source);

            // Delay before destruction so animation plays
            StartCoroutine(DestroyAfterDelay());
        }
    }
    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }


    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(deathDestroyDelay);
        Destroy(gameObject);
    }
}
