using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public WeaponBehaviour weaponBehaviour;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private float rollSpeed;

    public bool isControllable;

    private float horizontal;

    #region Physics
    [SerializeField]
    private LayerMask jumpable;
    private Rigidbody2D rb;
    private Collider2D col;

    #endregion

    #region TestFunctions
    IEnumerator Roll()
    {
        rb.velocity = new Vector2(rollSpeed * (GetComponent<SpriteRenderer>().flipX ? -1 : 1), rb.velocity.y);
        Debug.Log("Roll");
        isControllable = false;
        yield return new WaitForSeconds(1);
        isControllable = true;
    }
    #endregion

    private void OnEnable()
    {
        Game.inst.player = transform;
    }

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
        if(isControllable)
        {
            if (Input.GetButtonDown("Roll") && IsGrounded())
            {
                StartCoroutine(Roll());
            }
            else
            {
                horizontal = Input.GetAxisRaw("Horizontal");
                if (horizontal != 0)
                {
                    GetComponent<SpriteRenderer>().flipX = horizontal < 0;
                }
                if (Input.GetButtonDown("Jump") && IsGrounded())
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                }
            }
        }

        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * 1.5f * Time.deltaTime;
        }
        else if(rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * 0.5f * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if(isControllable)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }

    public void GetDamaged(int damage)
    {
        Debug.Log("Player hit, damage " + damage);
        weaponBehaviour.GetDamaged(damage);
    }
}
