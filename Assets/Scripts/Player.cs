﻿using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    private List<Buffable> buffables = new List<Buffable>();
    public WeaponBehaviour weaponBehaviour;
    [SerializeField]
    private Transform handCenter;

    private Animator anim;

    public bool isControllable;
    private bool isJumpKeyDown = false;
    private int maxJumpCount = 2;
    private int curJumpCount = 2;
    public bool isInvincible = false;

    private bool isPlayerLookRight = true;

    private float horizontal;

    #region Physics

    [SerializeField]
    private LayerMask jumpable;
    private Rigidbody2D rb;
    private Collider2D col;
    public float originSpeed;
    public float speed;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private float doubleJumpSpeed;
    [SerializeField]
    private float rollSpeed;

    #endregion


    private void OnEnable()
    {
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

    public void SetPlayerControllable(bool _isControllable)
    {
        isControllable = _isControllable;

        if(!isControllable)
        {
            horizontal = 0;
            anim.SetBool("Running", false);
        }
    }

    public void StopPlayer()
    {
        rb.velocity = Vector2.zero;
    }

    private void PlayerLookAt(bool isRight)
    {
        anim.SetFloat("IsRunForward", (horizontal < 0 ^ isRight) ? 1 : 0);
        
        if(IsGrounded())
        {
            GetComponent<SpriteRenderer>().flipX = horizontal == 0 ? !isRight : horizontal < 0;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = !isRight;
        }
        isPlayerLookRight = isRight;
    }

    private void Move(Vector2 direction)
    {
        if (isControllable)
        {
            horizontal = direction.x;
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

    IEnumerator RollRoutine(float time, bool rollDir)
    {
        SetPlayerControllable(false);
        anim.SetTrigger("Roll");
        Debug.Log("Roll");
        isInvincible = true;
        weaponBehaviour.gameObject.SetActive(false);

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            rb.velocity = new Vector2(rollSpeed * (rollDir ? 1 : -1), rb.velocity.y);
            yield return null;
        }
        SetPlayerControllable(true);
        isInvincible = false;
        anim.SetTrigger("RollEnd");
        weaponBehaviour.gameObject.SetActive(true);
    }

    private void Roll()
    {
        if(isControllable)
        {
            StartCoroutine(RollRoutine(0.5f, horizontal != 0 ? (horizontal > 0) : isPlayerLookRight));
        }
    }

    private IEnumerator DamagedRoutine()
    {
        isInvincible = true;

        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        float startTime = Time.time;
        while(Time.time - startTime < 1)
        {
            foreach (SpriteRenderer child in spriteRenderers)
            {
                child.color = new Color(1, 1, 1, 0);
            }
            yield return new WaitForSeconds(0.1f);

            foreach (SpriteRenderer child in spriteRenderers)
            {
                child.color = new Color(1, 1, 1, 1);
            }
            yield return new WaitForSeconds(0.1f);
        }

        foreach (SpriteRenderer child in spriteRenderers)
        {
            child.color = new Color(1, 1, 1, 1);
        }
        isInvincible = false;
    }

    public void AddBuffable(Buffable buff)
    {
        if(!isInvincible)
        {
            buffables.Add(buff);
            buff.StartDebuff(this);
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
        if(isControllable)
        {
            //FixMe
            Vector3 cursorDir = InputHandler.inst.CursorPos - handCenter.position;
            cursorDir = new Vector3(cursorDir.x, cursorDir.y, transform.position.z);

            float cursorAngle = Mathf.Atan2(cursorDir.y, cursorDir.x) * Mathf.Rad2Deg;
            bool isCursorRight = cursorAngle < 90 && cursorAngle > -90;
            handCenter.localScale = new Vector3(1, isCursorRight ? 1 : -1, 1);
            handCenter.rotation = Quaternion.Euler(0, 0, cursorAngle);

            PlayerLookAt(isCursorRight);
        }

        for (int i = 0; i < buffables.Count; i++)
        {
            buffables[i].Tick();
            buffables[i].Apply(this);

            /// CHECK IF CURRENT TIME IS GREATER THEN FINISHTIME OF BUFF
            if (buffables[i].ended)
            {
                buffables[i].EndDebuff(this);
                buffables.Remove(buffables[i]); /// REMOVE
            }
        }
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

    public void GetDamaged(float damage)
    {
        if(!isInvincible)
        {
            Debug.Log("Player hit, damage " + damage);
            weaponBehaviour.GetDamaged(damage);
            StartCoroutine(DamagedRoutine());
        }
    }
}

public abstract class Buffable
{
    private float duration;
    private float timer;
    public bool ended = false;

    public void Tick()
    {
        timer += Time.deltaTime;
        ended = timer > duration;
    }

    public void Init(float _duration)
    {
        duration = _duration;
    }

    public abstract void StartDebuff(Player player);
    public abstract void Apply(Player player);
    public abstract void EndDebuff(Player player);
}
