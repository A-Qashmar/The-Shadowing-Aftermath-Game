using UnityEngine;

public class HealItem : MonoBehaviour
{
    public int healAmount = 2;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Health h = other.GetComponent<Health>();
        if (h != null)
            h.Heal(healAmount);

        Destroy(gameObject);
    }
}
