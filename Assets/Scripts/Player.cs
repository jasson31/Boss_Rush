using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpPower;
    [SerializeField]
    private float rollPower;

    private float horizontal;

    #region Physics
    [SerializeField]
    private LayerMask jumpable;
    private Rigidbody2D rb;
    private Collider2D col;
    #endregion

    /*private void OnEnable()
    {
        InputHandler.inst.OnLeftKey += Move;
        InputHandler.inst.OnRightKey += Move;
    }

    private void OnDisable()
    {
        InputHandler.inst.OnLeftKey -= Move;
        InputHandler.inst.OnRightKey -= Move;
    }

    private void Move(Vector2 direction)
    {
        Vector3 moveDirection = direction;
        transform.position += moveDirection * speed * Time.deltaTime;
    }*/


    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, col.bounds.extents.y, jumpable);
        return hit.collider != null;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Roll") && IsGrounded())
        {
            rb.AddForce(new Vector2(rollPower * (GetComponent<SpriteRenderer>().flipX ? -1 : 1), 0));
        }
        else
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            if(horizontal != 0)
            {
                GetComponent<SpriteRenderer>().flipX = horizontal < 0;
            }
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            }
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }
}
