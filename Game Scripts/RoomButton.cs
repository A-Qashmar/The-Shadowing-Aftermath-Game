using UnityEngine;

public class RoomButton : MonoBehaviour
{
    public Door doorToOpen;
    bool used = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        used = true;
        if (doorToOpen != null)
            doorToOpen.Open();
    }
}
