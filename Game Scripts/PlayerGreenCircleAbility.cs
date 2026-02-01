using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGreenCircleAbility : MonoBehaviour
{
    public GameObject greenCirclePrefab;
    public float radius = 3f;
    public int damage = 2;
    public float cooldown = 5f;
    public float castDuration = 1f;
    public LayerMask enemyMask;

    public bool unlocked = false;

    float lastCastTime = -999f;
    Health health;

    float cooldownTimer;
    public float CooldownDuration => cooldown;
    public float CooldownRemaining => cooldownTimer;

    [SerializeField] AudioSource aSource;
    [SerializeField] AudioClip abilityClip;

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    void Awake()
    {
        health = GetComponent<Health>();
    }

    public void Unlock(GameObject source)
    {
        unlocked = true;
    }

    public void OnAbility3(InputValue value)
    {
        if (!unlocked) return;
        if (!value.isPressed) return;
        if (health != null && health.IsDead) return;
        if (cooldownTimer > 0f) return;
        if (Time.time < lastCastTime + cooldown) return;

        cooldownTimer = cooldown;

        Cast();
    }

    void Cast()
    {
        lastCastTime = Time.time;

        GameObject fx = Instantiate(greenCirclePrefab, transform.position, Quaternion.identity);
        GreenCircleDamage dmg = fx.GetComponent<GreenCircleDamage>();
        aSource.PlayOneShot(abilityClip);
        if (dmg != null)
        {
            dmg.Init(gameObject, radius, damage, enemyMask);
            dmg.TriggerAfterDelay(castDuration);
        }
    }
}
