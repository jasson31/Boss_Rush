﻿using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    public WeaponBehaviour weaponBehaviour;
    [SerializeField]
    private Transform handCenter;

    private Animator anim;

    public bool isControllable;
    private bool isJumpKeyDown = false;
    private int maxJumpCount = 2;
    private int curJumpCount = 2;

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
        anim.SetTrigger("Roll");
        Debug.Log("Roll");
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            rb.velocity = new Vector2(rollSpeed * (GetComponent<SpriteRenderer>().flipX ? -1 : 1), rb.velocity.y);
            isControllable = false;
            yield return null;
        }
        isControllable = true;
        anim.SetTrigger("RollEnd");
    }

    private void OnEnable()
    {
        Game.inst.player = transform;

        InputHandler.inst.OnLeftKey += Move;
        InputHandler.inst.OnRightKey += Move;
        InputHandler.inst.OnUpKeyDown += Roll;
        InputHandler.inst.OnJumpKeyDown += () => { Jump(); isJumpKeyDown = true; };

        InputHandler.inst.OnLeftKeyUp += (Vector2 dir) => { horizontal = 0; anim.SetBool("Running", false); };
        InputHandler.inst.OnRightKeyUp += (Vector2 dir) => { horizontal = 0; anim.SetBool("Running", false); };
        InputHandler.inst.OnJumpKeyUp += () => { isJumpKeyDown = false; };
    }

    private void OnDisable()
    {
        InputHandler.inst.OnLeftKey -= Move;
        InputHandler.inst.OnRightKey -= Move;
        InputHandler.inst.OnUpKeyDown -= Roll;
        InputHandler.inst.OnJumpKeyDown -= () => { Jump(); isJumpKeyDown = true; };

        InputHandler.inst.OnLeftKeyUp -= (Vector2 dir) => { horizontal = 0; anim.SetBool("Running", false); };
        InputHandler.inst.OnRightKeyUp -= (Vector2 dir) => { horizontal = 0; anim.SetBool("Running", false); };
        InputHandler.inst.OnJumpKeyUp -= () => { isJumpKeyDown = false; };
    }

    private bool IsGrounded()
    {
        Vector3 leftFoot = col.bounds.center - new Vector3(col.bounds.size.x / 2, col.bounds.size.y / 2);
        Vector3 rightFoot = col.bounds.center - new Vector3(-col.bounds.size.x / 2, col.bounds.size.y / 2);
        bool isGrounded = Physics2D.Raycast(leftFoot, Vector2.down, 0.1f, jumpable) || Physics2D.Raycast(rightFoot, Vector2.down, 0.1f, jumpable);
        return rb.velocity.y < 0.01f && isGrounded;
    }

    private void PlayerLookAt(bool isRight)
    {
        GetComponent<SpriteRenderer>().flipX = !isRight;
    }

    private void Move(Vector2 direction)
    {
        if (isControllable)
        {
            horizontal = direction.x;
            //PlayerLookAt(horizontal < 0);
            anim.SetBool("Running", true);
        }
    }

    private void Jump()
    {
        if (isControllable)
        {
            if (IsGrounded())
            {
                curJumpCount = maxJumpCount;
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                curJumpCount--;
                anim.SetTrigger("Jump");
            }
            else if(curJumpCount > 0 && !IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, doubleJumpSpeed);
                curJumpCount--;
                anim.SetTrigger("Jump");
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
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        //FixMe
        Vector3 cursorDir = Camera.main.ScreenToWorldPoint(TestScript.inst.cursor.transform.position) - handCenter.position;
        cursorDir = new Vector3(cursorDir.x, cursorDir.y, transform.position.z);

        float weaponAngle = (transform.localScale.x > 0 ? 1 : -1) * Mathf.Atan2(cursorDir.y, cursorDir.x) * Mathf.Rad2Deg;
        bool isCursorRight = weaponAngle < 90 && weaponAngle > -90;
        handCenter.localScale = new Vector3(1, isCursorRight ? 1 : -1, 1);
        handCenter.rotation = Quaternion.Euler(0, 0, weaponAngle);

        PlayerLookAt(isCursorRight);

        Debug.Log(cursorDir);


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

    private void LateUpdate()
    {
        anim.SetBool("Landed", IsGrounded());
    }

    public void GetDamaged(int damage)
    {
        Debug.Log("Player hit, damage " + damage);
        weaponBehaviour.GetDamaged(damage);
    }
}
