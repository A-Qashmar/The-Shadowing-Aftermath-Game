using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBoss2Ability : MonoBehaviour
{
    [Header("Ability Settings")]
    public GameObject groundFirePrefab;
    public float castRange = 4f;
    public float cooldown = 5f;

    [Header("State")]
    public bool isUnlocked = false;

    float cooldownTimer;
    Health health;

    public int damage = 1;
    public LayerMask enemyMask;

    public float CooldownDuration => cooldown;
    public float CooldownRemaining => cooldownTimer;

    [SerializeField] AudioSource aSource;
    [SerializeField] AudioClip abilityClip;


    void Awake()
    {
        health = GetComponent<Health>();
    }

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    // Called by Boss 2 Health.onDeathWithReference
    public void UnlockAbility(GameObject source)
    {
        if (isUnlocked) return;

        isUnlocked = true;
        Debug.Log("Boss 2 ability unlocked!");
    }

    // New Input System action: Ability2
    public void OnAbility2(InputValue value)
    {
        if (!isUnlocked) return;
        if (!value.isPressed) return;
        if (health != null && health.IsDead) return;
        if (cooldownTimer > 0f) return;
        if (groundFirePrefab == null) return;
        if (Camera.main == null) return;

        cooldownTimer = cooldown;

        // Get mouse position in world
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = -Camera.main.transform.position.z;
        Vector3 world = Camera.main.ScreenToWorldPoint(mousePos);
        world.z = 0f;

        aSource.PlayOneShot(abilityClip);

        // Clamp to cast range from the player
        Vector3 dir = world - transform.position;
        if (dir.magnitude > castRange)
            world = transform.position + dir.normalized * castRange;

        GameObject fire = Instantiate(groundFirePrefab, world, Quaternion.identity);

        BossGroundFire gf = fire.GetComponent<BossGroundFire>();
        if (gf != null)
        {
            gf.Init(gameObject, enemyMask, damage);   // hit enemies, source = player
        }

    }
}
