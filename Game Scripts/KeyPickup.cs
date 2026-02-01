using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public Level2Manager levelManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (levelManager != null)
            levelManager.AddKey();

        Destroy(gameObject);
    }
}
