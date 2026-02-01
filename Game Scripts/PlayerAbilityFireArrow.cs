using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilityFireArrow : MonoBehaviour
{
    public weaponHold weaponHolder;
    public Transform firePoint;
    public GameObject fireArrowPrefab;

    public float cooldown = 2f;
    public float projectileSpeed = 8f;
    public int damage = 1;

    public bool unlocked;

    float cooldownTimer;
    Health health;

    [SerializeField] AudioSource aSource;
    [SerializeField] AudioClip abilityClip;
    public float CooldownRemaining => cooldownTimer;
    public float CooldownDuration => cooldown;

    void Awake()
    {
        health = GetComponent<Health>();
        aSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    public void UnlockFireArrow(GameObject source)
    {
        unlocked = true;
    }

    public void OnAbility1(InputValue value)
    {
        if (!unlocked) return;
        if (health != null && health.IsDead) return;
        if (!value.isPressed) return;
        if (cooldownTimer > 0f) return;
        if (fireArrowPrefab == null || firePoint == null) return;

        cooldownTimer = cooldown;

        Vector2 dir = Vector2.right;
        if (weaponHolder != null)
            dir = weaponHolder.transform.right;

        GameObject projGO = Instantiate(fireArrowPrefab, firePoint.position, Quaternion.identity);
        WizardProjectile proj = projGO.GetComponent<WizardProjectile>();
        aSource.PlayOneShot(abilityClip);
        if (proj != null)
        {
            proj.Init(dir, damage, gameObject, LayerMask.GetMask("Enemy"));
            proj.speed = projectileSpeed;
        }
    }
}
