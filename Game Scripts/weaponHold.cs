using System.Collections;
using UnityEngine;

public class weaponHold : MonoBehaviour
{
    public SpriteRenderer characterRenderer, weaponRenderer;
    public Vector2 pointerPosition { get; set; }

    public Animator animator;
    public float delay = 0.3f;
    private bool attackBlocked;

    public Transform circleOrigin;
    public float attackRange;
    public bool canAttack = true;

    public bool isAttacking{ get; private set; }

    public void resetIsAttacking()
    {
        isAttacking = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isAttacking)
            return;
        Vector2 dir = (pointerPosition - (Vector2)transform.position).normalized;
        transform.right = dir;

        Vector2 scale = transform.localScale;
        if(dir.x < 0)
        {
            scale.y = -1;
        }
        else
        {
            scale.y = 1;
        }
        transform.localScale = scale;

        if(transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180)
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder - 1;
        }else{
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder + 1;
        }
    }

    public void Attack()
    {
        if (attackBlocked || !canAttack)
            return;
         
        animator.SetTrigger("attack");
        isAttacking = true;
        attackBlocked = true;
        StartCoroutine(delayAtack());
    }

    private IEnumerator delayAtack()
    {
        yield return new WaitForSeconds(delay);
        attackBlocked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 position = circleOrigin == null ? Vector3.zero : circleOrigin.position;
        Gizmos.DrawWireSphere(position, attackRange);
    }

    public void detectColliders()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrigin.position, attackRange))
        {
            Health health;
            if(health = collider.GetComponent<Health>())
            {
                health.getHit(1, transform.parent.gameObject);
            }
        }
    }

}
