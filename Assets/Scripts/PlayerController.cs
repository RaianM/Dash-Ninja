using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;


    [SerializeField] private float dashingVelocity = 14f;
    [SerializeField] private float dashingTime = 0.5f;
    private Vector2 dashingDir;
    private bool isDashing;
    private bool canDash = true;
    private bool dashInput;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private float wallCooldown;
    private float horizontalInput;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

    }


    // Update is called once per frame
    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        dashInput = Input.GetMouseButtonDown(0);

        if(dashInput && canDash)
        {
            isDashing = true;
            canDash = false;
            dashingDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (dashingDir == Vector2.zero)
            {
                dashingDir = new Vector2(transform.localScale.x, 0);
            }
            StartCoroutine(StopDashing());
        }

        if (isDashing)
        {
            body.velocity = dashingDir.normalized * dashingVelocity;
            return;
        }

        if (isGrounded() || onWall())
        {
            canDash = true;
        }

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        // Handles jumps logic
        if (wallCooldown > 0.2f)
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            if (onWall() && !isGrounded() && horizontalInput != 0)
            { // this makes you hold the wall for a short period, you'll start sliding after.
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
                body.gravityScale = 2.5f;

            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
        }
        else
            wallCooldown += Time.deltaTime;

    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpHeight);
        }
        else if (onWall() && !isGrounded())
        {
            if (horizontalInput == 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * speed, jumpHeight*0.75f);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                body.gravityScale = 1;
            }
            else
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * speed, jumpHeight*0.75f);

            wallCooldown = 0;
        }
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
    }

    private bool isGrounded()
    {
        // Origin of box, size, angle (for rotations), direction, distance from object, layer of objects affected.
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        // Origin of box, size, angle (for rotations), direction, distance from object, layer of objects affected.
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
}