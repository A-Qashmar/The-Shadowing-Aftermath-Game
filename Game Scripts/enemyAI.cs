using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class enemyAI : MonoBehaviour
{
    public enum EnemyType
    {
        Melee,
        Wizard,
        BossFlame   // second boss that has ground-fire ability
    }
    public EnemyType enemyType = EnemyType.Melee;

    [Header("Detection & Attack")]
    public float detectionRadius = 6f;
    public float attackRadius = 1.5f;
    public float moveSpeed = 3f;
    public float attackCooldown = 1f;
    public int attackDamage = 1;

    [Header("Layers & LOS")]
    public LayerMask obstacleMask;
    public LayerMask targetMask;
    public Transform attackOrigin;

    [Header("Wizard Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 8f;

    [Header("Boss 2 Ground Fire Ability")]
    public bool hasGroundFireAbility = false;   // ON only for boss 2
    public GameObject groundFirePrefab;         // flame column prefab
    public float groundFireRadius = 4f;         // how far around boss flames can appear
    public int groundFireCount = 6;             // how many flames per cast
    public float groundFireCooldown = 6f;       // seconds between casts
    public float minAbilityDistance = 2f;       // don’t cast if player is too close
    public float abilityWindupTime = 0.6f;      // delay before flames appear
    public float betweenFlamesDelay = 0.15f;    // delay between each spawned flame

    [Header("Boss 3 Green Circle Ability")]
    public bool hasGreenCircleAbility = false;
    public Boss3Ability boss3Ability;

    [Header("Boss Camera Zoom")]
    public bool isBoss = false;          // tick this ONLY on the boss
    public float bossCameraSize = 7f;    // how far to zoom out during boss
    public float cameraLerpSpeed = 3f;   // how fast the camera changes

    [Header("Final Boss Settings")]
    public bool isFinalBoss = false;   // tick only on the LAST boss
    public string victorySceneName = "Victory";   // change to your scene name if different

    Camera mainCam;
    float normalCameraSize;
    bool bossZoomActive = false;
    bool victoryStarted = false;


    bool isCastingAbility = false;
    float lastGroundFireTime = -999f;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    Transform target;

    bool isDead;
    bool isAttacking;
    Vector2 moveDir;
    float lastAttackTime;

    Health health;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        health = GetComponent<Health>();
        if (health != null && health.onDeathWithReference != null)
            health.onDeathWithReference.AddListener(OnDie);

        if (hasGreenCircleAbility)
            boss3Ability = GetComponent<Boss3Ability>();

        if (isBoss)
        {
            mainCam = Camera.main;
            if (mainCam != null)
                normalCameraSize = mainCam.orthographicSize;
        }
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            target = player.transform;
    }

    void Update()
    {
        if (isDead)
        {
            animator.SetFloat("Speed", 0f);
            moveDir = Vector2.zero;
            return;
        }

        if (health.IsDead)
        {
            DisablePhysicsAndColliders();
            isDead = true;
            return;
        }

        if (target == null) return;

        if (hasGreenCircleAbility && boss3Ability != null)
        {
            boss3Ability.Tick(target, detectionRadius);

            if (boss3Ability.IsCasting)
            {
                moveDir = Vector2.zero;
                animator.SetFloat("Speed", 0f);
                return;
            }
        }


        float dist = Vector2.Distance(transform.position, target.position);
        bool inRange = dist <= detectionRadius;
        bool hasLOS = inRange && HasLineOfSight();

        if (isBoss && mainCam != null)
        {
            // Zoom while player is detected and boss not dead
            if (hasLOS && !health.IsDead)
                bossZoomActive = true;
            else
                bossZoomActive = false;

            float targetSize = bossZoomActive ? bossCameraSize : normalCameraSize;
            mainCam.orthographicSize = Mathf.Lerp(
                mainCam.orthographicSize,
                targetSize,
                Time.deltaTime * cameraLerpSpeed
            );
        }

        // ---------- BOSS ABILITY CHECK ----------
        if (hasGroundFireAbility && !isCastingAbility)
        {
            bool abilityReady = Time.time >= lastGroundFireTime + groundFireCooldown;

            // Boss can cast only when:
            // - ability ready
            // - has line of sight
            // - player is not too close
            if (abilityReady && hasLOS && dist > minAbilityDistance)
            {
                StartCoroutine(GroundFireRoutine());
                return; // skip normal behaviour while casting
            }
        }

        // If we don’t see the player, just idle
        if (!hasLOS)
        {
            moveDir = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            return;
        }

        // If we are currently casting, don't move/attack
        if (isCastingAbility)
        {
            moveDir = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            return;
        }

        // ---------- NORMAL MELEE / WIZARD MOVEMENT ----------
        if (dist > attackRadius)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            moveDir = dir;

            animator.SetFloat("Speed", 1f);

            if (spriteRenderer != null && Mathf.Abs(dir.x) > 0.01f)
                spriteRenderer.flipX = dir.x < 0f;
        }
        else
        {
            moveDir = Vector2.zero;
            animator.SetFloat("Speed", 0f);

            if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
                StartCoroutine(AttackRoutine());
        }
    }

    public void ForceStopMovement()
    {
        moveDir = Vector2.zero;
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }




    void FixedUpdate()
    {
        if (isDead || isCastingAbility) return;

        if (moveDir.sqrMagnitude > 0.001f)
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }

    bool HasLineOfSight()
    {
        if (target == null) return false;

        Vector2 dir = (target.position - transform.position);
        float dist = dir.magnitude;
        dir.Normalize();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, obstacleMask);
        return hit.collider == null;
    }

    IEnumerator AttackRoutine()
    {
        if (isCastingAbility) yield break;   // don’t melee while casting

        isAttacking = true;
        lastAttackTime = Time.time;
        animator.SetTrigger("attack");

        yield return new WaitForSeconds(attackCooldown * 0.4f);
        DoAttackHit();

        yield return new WaitForSeconds(attackCooldown * 0.6f);
        isAttacking = false;
    }

    IEnumerator GroundFireRoutine()
    {
        isCastingAbility = true;
        moveDir = Vector2.zero;
        animator.SetFloat("Speed", 0f);

        // small wind-up
        yield return new WaitForSeconds(abilityWindupTime);

        for (int i = 0; i < groundFireCount; i++)
        {
            // random point in a circle around the boss
            Vector2 offset = Random.insideUnitCircle * groundFireRadius;
            Vector3 spawnPos = transform.position + (Vector3)offset;

            Instantiate(groundFirePrefab, spawnPos, Quaternion.identity);

            if (betweenFlamesDelay > 0f)
                yield return new WaitForSeconds(betweenFlamesDelay);
        }

        lastGroundFireTime = Time.time;
        isCastingAbility = false;
    }

    void DoAttackHit()
    {
        // BossFlame uses normal melee as primary attack
        if (enemyType == EnemyType.Melee || enemyType == EnemyType.BossFlame)
        {
            if (attackOrigin == null)
                attackOrigin = transform;

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                attackOrigin.position,
                attackRadius,
                targetMask
            );

            foreach (var h in hits)
            {
                Health hp = h.GetComponent<Health>();
                if (hp != null)
                    hp.getHit(attackDamage, gameObject);
            }
        }
        else if (enemyType == EnemyType.Wizard)
        {
            if (projectilePrefab == null || firePoint == null || target == null)
                return;

            GameObject projGO = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            Vector2 dir = (target.position - firePoint.position).normalized;

            WizardProjectile proj = projGO.GetComponent<WizardProjectile>();
            if (proj != null)
            {
                // use your current Init signature here
                proj.Init(dir, attackDamage, gameObject, targetMask);
            }
        }
    }

    public void OnDie(GameObject source)
    {
        if (isDead) return;

        moveDir = Vector2.zero;
        moveSpeed = 0f;
        isDead = true;
        animator.SetTrigger("death");
        DisablePhysicsAndColliders();
        if (rb != null) rb.simulated = false;

        if (isBoss)
            bossZoomActive = false;

        if (isFinalBoss && !victoryStarted)
        {
            victoryStarted = true;
            StartCoroutine(LoadVictoryAfterDelay(5f));
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Vector3 origin = attackOrigin == null ? transform.position : attackOrigin.position;
        Gizmos.DrawWireSphere(origin, attackRadius);

        // visualize ground-fire radius for boss
        if (hasGroundFireAbility)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, groundFireRadius);
        }
    }

    void DisablePhysicsAndColliders()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        foreach (var col in GetComponents<Collider2D>())
            col.enabled = false;

        foreach (var script in GetComponents<MonoBehaviour>())
        {
            if (script != this)
                script.enabled = false;
        }
    }

    IEnumerator LoadVictoryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(victorySceneName);
    }

}
