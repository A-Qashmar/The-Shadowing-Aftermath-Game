using UnityEngine;

public class WizardProjectile : MonoBehaviour
{
    public float speed = 6f;
    public float lifeTime = 3f;
    public int damage = 1;

    private Vector2 direction = Vector2.right;
    private GameObject source;

    [Header("Who this projectile can hit")]
    public LayerMask hitMask;    // <-- NEW

    public void Init(Vector2 dir, int dmg, GameObject src, LayerMask mask)
    {
        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector2.right;

        direction = dir.normalized;
        damage = dmg;
        source = src;
        hitMask = mask;

        // rotate sprite toward direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == source) return;

        // check if the collided object is in the allowed hitMask
        if (((1 << other.gameObject.layer) & hitMask) == 0)
            return;

        Health hp = other.GetComponent<Health>();
        if (hp != null)
            hp.getHit(damage, source);

        Destroy(gameObject);
    }
}
