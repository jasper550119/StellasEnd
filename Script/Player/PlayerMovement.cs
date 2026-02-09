using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float jumpPower;
    public float Move;

    public bool isFacingRight;
    private Rigidbody2D rb;
    public Animator anim;

    private bool jumping;

    public float dashSpeed = 20f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 1f;
    bool isDashing;
    bool canDash = true;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private LayerMask groundLayer;
    private BoxCollider2D boxCollider;


    // Start is called before the first frame update
    void Start()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
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

            anim.SetTrigger("Dashing");
        }

        if(Move >= 0.1f || Move <= -0.1f)
        {
            anim.SetBool("isRunning", true);
        }
        else 
        {
            anim.SetBool("isRunning", false);
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
        if (isDashing)
        {
            return;
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        
        transform.Rotate(0f, 180f, 0f);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        anim.SetBool("isJumping", true);
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
        tr.emitting = true;
        
        yield return new WaitForSeconds(dashDuration);

        tr.emitting = false;
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
            anim.SetBool("isJumping", false);

            jumping = false;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            anim.SetBool("isJumping", true);

            jumping = true;
        }
    }
}
