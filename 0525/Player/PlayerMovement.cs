using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool canControl = true;
    public bool isTalking = false;

    public float speed;
    public float jumpPower;
    public float Move;

    public bool isFacingRight;
    private Rigidbody2D rb;
    public Animator anim;

    private bool jumping;

    public float dashSpeed = 20f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 0.8f;
    
    // 【修改】：改為 public 讓 PlayerHP 腳本可以讀取這個狀態
    public bool isDashing; 
    bool canDash = true;
    
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private LayerMask groundLayer;
    private BoxCollider2D boxCollider;
    private float defaultGravityScale;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        defaultGravityScale = rb.gravityScale;
    }

    void Start()
    {
        isFacingRight = true;
    }

    public void SetControl(bool value)
    {
        canControl = value;

        if (!value)
        {
            ResetDashState();
            if (rb != null) rb.velocity = new Vector2(0f, rb.velocity.y);
            if (anim != null) anim.SetBool("isRunning", false);
        }
    }

    private void ResetDashState()
    {
        if (tr != null) tr.emitting = false;
        if (rb != null) rb.gravityScale = defaultGravityScale;
        isDashing = false;
        canDash = true;
        Physics2D.IgnoreLayerCollision(8, 9, false);
    }

    private void OnDisable()
    {
        ResetDashState();
    }

    private void OnDestroy()
    {
        Physics2D.IgnoreLayerCollision(8, 9, false);
    }

    void Update()
    {
        if (!canControl || isTalking)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y); 
            if (anim != null) anim.SetBool("isRunning", false); 
            return; 
        }
        
        if (isDashing)
        {
            return;
        }
        
        Move = Input.GetAxis("Horizontal");

        rb.velocity = new Vector2(Move * speed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && !jumping)
            Jump();
        
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);

        if (Input.GetButtonDown("Dash") && canDash)
        {
            StartCoroutine(Dash());
            if (anim != null) anim.SetTrigger("Dashing");
        }

        if(Move >= 0.1f || Move <= -0.1f)
        {
            if (anim != null) anim.SetBool("isRunning", true);
        }
        else 
        {
            if (anim != null) anim.SetBool("isRunning", false);
        }

        if (!isFacingRight && Move > 0f)
        {
            Flip();
        }
        else if (isFacingRight && Move < 0f)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        if (anim != null) anim.SetBool("isJumping", true);
    }

    private IEnumerator Dash()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        float dashDirection = isFacingRight ? 1f :-1f;
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);
        if (tr != null) tr.emitting = true;
        
        yield return new WaitForSeconds(dashDuration);

        if (tr != null) tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        Physics2D.IgnoreLayerCollision(8, 9, false);
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            if (anim != null) anim.SetBool("isJumping", false);
            jumping = false;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            if (anim != null) anim.SetBool("isJumping", true);
            jumping = true;
        }
    }
}
