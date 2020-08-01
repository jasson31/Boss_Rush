using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public WeaponBehaviour weaponBehaviour;

    public bool isControllable;
    private bool isJumpKeyDown = false;
    private bool isDoubleJump = false;

    private float horizontal;

    #region Physics

    [SerializeField]
    private LayerMask jumpable;
    private Rigidbody2D rb;
    private Collider2D col;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private float doubleJumpSpeed;
    [SerializeField]
    private float rollSpeed;

    #endregion

    IEnumerator RollRoutine()
    {
        Debug.Log("Roll");
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            rb.velocity = new Vector2(rollSpeed * (GetComponent<SpriteRenderer>().flipX ? -1 : 1), rb.velocity.y);
            isControllable = false;
            yield return null;
        }
        isControllable = true;
    }

    private void OnEnable()
    {
        Game.inst.player = transform;

        InputHandler.inst.OnLeftKey += Move;
        InputHandler.inst.OnRightKey += Move;
        InputHandler.inst.OnUpKeyDown += Roll;
        InputHandler.inst.OnJumpKeyDown += () => { Jump(); isJumpKeyDown = true; };

        InputHandler.inst.OnLeftKeyUp += (Vector2 dir) => { horizontal = 0; };
        InputHandler.inst.OnRightKeyUp += (Vector2 dir) => { horizontal = 0; };
        InputHandler.inst.OnJumpKeyUp += () => { isJumpKeyDown = false; };
    }

    private void OnDisable()
    {
        InputHandler.inst.OnLeftKey -= Move;
        InputHandler.inst.OnRightKey -= Move;
        InputHandler.inst.OnUpKeyDown -= Roll;
        InputHandler.inst.OnJumpKeyDown -= () => { Jump(); isJumpKeyDown = true; };

        InputHandler.inst.OnLeftKeyUp -= (Vector2 dir) => { horizontal = 0; };
        InputHandler.inst.OnRightKeyUp -= (Vector2 dir) => { horizontal = 0; };
        InputHandler.inst.OnJumpKeyUp -= () => { isJumpKeyDown = false; };
    }

    private bool IsGrounded()
    {
        Vector3 leftFoot = col.bounds.center - new Vector3(col.bounds.size.x / 2, col.bounds.size.y / 2);
        Vector3 rightFoot = col.bounds.center - new Vector3(-col.bounds.size.x / 2, col.bounds.size.y / 2);
        bool isGrounded = Physics2D.Raycast(leftFoot, Vector2.down, 0.1f, jumpable) || Physics2D.Raycast(rightFoot, Vector2.down, 0.1f, jumpable);
        return rb.velocity.y < 0.01f && isGrounded;
    }

    private void Move(Vector2 direction)
    {
        horizontal = direction.x;
        if (isControllable)
        {
            GetComponent<SpriteRenderer>().flipX = horizontal < 0;
        }
    }

    private void Jump()
    {
        if (isControllable)
        {
            if (IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                isDoubleJump = false;
            }
            else if(!isDoubleJump && !IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, doubleJumpSpeed);
                isDoubleJump = true;
            }
        }

    }

    private void Roll()
    {
        if(isControllable)
        {
            StartCoroutine(RollRoutine());
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if(isControllable)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * 1.5f * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !isJumpKeyDown)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * 0.5f * Time.deltaTime;
        }
    }

    public void GetDamaged(int damage)
    {
        Debug.Log("Player hit, damage " + damage);
        weaponBehaviour.GetDamaged(damage);
    }
}
