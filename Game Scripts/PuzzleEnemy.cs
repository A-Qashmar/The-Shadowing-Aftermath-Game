using UnityEngine;

public class PuzzleEnemy : MonoBehaviour
{
    public enum PuzzleType { Skeleton, Zombie, Spider }
    public PuzzleType type;

    PuzzleManager manager;
    Health health;

    void Awake()
    {
        health = GetComponent<Health>();
        manager = FindFirstObjectByType<PuzzleManager>();
    }

    void OnEnable()
    {
        if (health != null)
            health.onDeathWithReference.AddListener(OnDeath);
    }

    void OnDisable()
    {
        if (health != null)
            health.onDeathWithReference.RemoveListener(OnDeath);
    }

    void OnDeath(GameObject src)
    {
        if (manager != null)
            manager.OnPuzzleEnemyKilled(this);
    }
}
