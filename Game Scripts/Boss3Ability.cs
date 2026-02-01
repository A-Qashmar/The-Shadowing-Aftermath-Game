using System.Collections;
using UnityEngine;

[RequireComponent(typeof(enemyAI))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody2D))]
public class Boss3Ability : MonoBehaviour
{
    public GameObject greenCirclePrefab;
    public float radius = 3f;
    public int damage = 2;
    public float cooldown = 5f;
    public float castDuration = 1f;
    public float minCastDistance = 2f;
    public LayerMask playerMask;

    enemyAI ai;
    Animator animator;
    Rigidbody2D rb;
    Health health;

    float lastCastTime = -999f;
    bool isCasting;

    public bool IsCasting => isCasting;

    void Awake()
    {
        ai = GetComponent<enemyAI>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
    }

    public void Tick(Transform target, float chaseRange)
    {
        if (health != null && health.IsDead) return;
        if (isCasting) return;
        if (Time.time < lastCastTime + cooldown) return;
        if (target == null) return;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist > chaseRange) return;
        if (dist < minCastDistance) return;

        StartCoroutine(CastRoutine());
    }

    IEnumerator CastRoutine()
    {
        isCasting = true;
        lastCastTime = Time.time;

        if (ai != null) ai.ForceStopMovement();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetTrigger("greenCircle");
            animator.SetFloat("Speed", 0f);
        }

        GameObject fx = Instantiate(greenCirclePrefab, transform.position, Quaternion.identity);
        GreenCircleDamage dmg = fx.GetComponent<GreenCircleDamage>();
        if (dmg != null)
        {
            dmg.Init(gameObject, radius, damage, playerMask);
            dmg.TriggerAfterDelay(castDuration);
        }

        yield return new WaitForSeconds(castDuration);
        isCasting = false;
    }
}
