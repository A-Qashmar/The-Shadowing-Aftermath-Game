using UnityEngine;

public class BossGroundFire : MonoBehaviour
{
    public float lifeTime = 1.2f;
    public float damageDelay = 0.3f;
    public float damageRadius = 0.7f;
    public int damage = 1;

    public LayerMask targetMask;   // was: playerLayer
    GameObject source;

    // call this right after Instantiate
    public void Init(GameObject src, LayerMask mask, int dmg)
    {
        source = src;
        targetMask = mask;
        damage = dmg;
    }

    void Start()
    {
        StartCoroutine(DoDamageThenDie());
    }

    System.Collections.IEnumerator DoDamageThenDie()
    {
        yield return new WaitForSeconds(damageDelay);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            damageRadius,
            targetMask
        );

        foreach (var h in hits)
        {
            if (h.gameObject == source) continue;   // don’t hurt the caster

            Health hp = h.GetComponent<Health>();
            if (hp != null)
            {
                hp.getHit(damage, source);
                print("hit " + h.name);
            }
        }

        yield return new WaitForSeconds(lifeTime - damageDelay);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
