using UnityEngine;

public class Door : MonoBehaviour
{
    SpriteRenderer sr;
    Collider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    // Open = remove the wall/door
    public void Open()
    {
        Destroy(gameObject);
        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;
    }

    // Close = block the path again (optional)
    public void Close()
    {
        if (sr != null) sr.enabled = true;
        if (col != null) col.enabled = true;
    }
}
