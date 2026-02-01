using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;
    public weaponHold weaponHolder;

    Vector2 moveInput, pointerInput;
    Vector2 lastMoveDir = Vector2.down;

    Health health;
    bool hasTriggeredReload = false;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    void Update()
    {
        if (health != null && health.IsDead)
        {
            moveInput = Vector2.zero;
            animator.SetFloat("Speed", 0f);

            if (weaponHolder != null)
                weaponHolder.canAttack = false;

            if (!hasTriggeredReload)
            {
                hasTriggeredReload = true;
                StartCoroutine(ReloadAfterDelay(2f));   // wait 2 seconds then restart
            }

            return;
        }

        if (moveInput.sqrMagnitude > 0.001f)
        {
            lastMoveDir = moveInput.normalized;
        }

        animator.SetFloat("Horizontal", lastMoveDir.x);
        animator.SetFloat("Vertical", lastMoveDir.y);
        animator.SetFloat("Speed", moveInput.sqrMagnitude);

        pointerInput = GetPointerPosition();

        if (weaponHolder != null)
        {
            weaponHolder.pointerPosition = pointerInput;
        }
    }

    private Vector2 GetPointerPosition()
    {
        if (Camera.main == null || Mouse.current == null)
            return transform.position;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    void FixedUpdate()
    {
        if (health != null && health.IsDead)
            return;

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnAttack(InputValue value)
    {
        if (health != null && health.IsDead)
            return;

        if (weaponHolder != null && value.isPressed)
        {
            weaponHolder.Attack();
        }
    }

    System.Collections.IEnumerator ReloadAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
}
