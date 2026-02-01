using System.Collections;
using UnityEngine;

public class GreenCircleDamage : MonoBehaviour
{
    public float radius = 3f;
    public int damage = 1;
    public LayerMask hitMask;

    GameObject source;

    public void Init(GameObject src, float r, int dmg, LayerMask mask)
    {
        source = src;
        radius = r;
        damage = dmg;
        hitMask = mask;
    }

    public void TriggerAfterDelay(float delay)
    {
        StartCoroutine(DoDamageAfterDelay(delay));
    }

    IEnumerator DoDamageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DoDamage();
    }

    public void DoDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, hitMask);

        foreach (var h in hits)
        {
            if (h.gameObject == source) continue;

            Health hp = h.GetComponent<Health>();
            if (hp != null)
                hp.getHit(damage, source);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
