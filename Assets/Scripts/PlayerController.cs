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

/*
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public PlayerData Data;

	#region COMPONENTS
	public Rigidbody2D rb { get; private set; } // get takes from the GameObject
	#endregion

	#region STATE PARAMETERS
	//Variables control the various actions the player can perform at any time.
	//These are fields which can are public allowing for other sctipts to read them
	//but can only be privately written to.
	public bool IsFacingRight { get; private set; }
	public bool IsJumping { get; private set; }
	public bool IsWallJumping { get; private set; }
	public bool IsDashing { get; private set; }
	public bool IsSliding { get; private set; }

	//Timers (also all fields, could be private and a method returning a bool could be used)
	public float LastOnGroundTime { get; private set; } 
	public float LastOnWallTime { get; private set; }
	public float LastOnWallRightTime { get; private set; }
	public float LastOnWallLeftTime { get; private set; }

	//Jump
	private bool _isJumpCut;
	private bool _isJumpFalling;

	//Wall Jump
	private float _wallJumpStartTime;
	private int _lastWallJumpDir;

	//Dash
	private int _dashesLeft;
	private bool _dashRefilling;
	private Vector2 _lastDashDir;
	private bool _isDashAttacking;

	#endregion

	#region INPUT PARAMETERS
	private Vector2 _moveInput;

	public float LastPressedJumpTime { get; private set; } // if 0 then can't jump
	public float LastPressedDashTime { get; private set; }
	#endregion

	#region CHECK PARAMETERS
	//Set all of these up in the inspector
	[Header("Checks")]
	[SerializeField] private Transform _groundCheckPoint;
	//Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
	[SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
	[Space(5)]
	[SerializeField] private Transform _frontWallCheckPoint;
	[SerializeField] private Transform _backWallCheckPoint;
	[SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
	#endregion

	#region Layers
	[Header("Layers & Tags")]
	[SerializeField] private LayerMask _groundLayer;
    #endregion

    private void Awake() // Awake starts the moment when the object is made
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void Start() // start is called right before update starts running
	{
		SetGravityScale(Data.gravityScale);
		IsFacingRight = true;
	}


	void Update()
	{
		LastOnGroundTime -= Time.deltaTime;
		LastOnWallTime -= Time.deltaTime;
		LastOnWallRightTime -= Time.deltaTime;
		LastOnWallLeftTime -= Time.deltaTime;

		LastPressedJumpTime -= Time.deltaTime;
		LastPressedDashTime -= Time.deltaTime;

		_moveInput.x = Input.GetAxisRaw("Horizontal");
		_moveInput.y = Input.GetAxisRaw("Vertical");

		if (_moveInput.x != 0)
			CheckDirectionToFace(_moveInput.x > 0);

		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.J))
		{
			OnJumpInput();
		}
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.J))
		{
			OnJumpUpInput();
		}
		if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Mouse0))
		{
			OnDashInput();
		}

		if (!IsDashing && !IsJumping)
		{
			if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping)
			{
				LastOnGroundTime = Data.coyoteTime;
			}
			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
			  || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)) && !IsWallJumping)
				LastOnWallRightTime = Data.coyoteTime;

			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
			  || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)) && !IsWallJumping)
				LastOnWallLeftTime = Data.coyoteTime;

			LastOnWallTime = Mathf.Max(LastOnWallTime, LastOnWallRightTime);

		}

		if (IsJumping && rb.velocity.y < 0)
		{
			IsJumping = false;
			if (!IsWallJumping)
				_isJumpFalling = true;
		}
		if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
		{
			IsWallJumping = false;
		}
		if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
		{
			_isJumpCut = false;
			if (!IsJumping)
				_isJumpFalling = false;
		}
		if (!IsDashing)
		{
			if (CanJump() && LastPressedJumpTime > 0)
			{
				IsJumping = true;
				IsWallJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;
				Jump();
			}
			else if (CanWallJump() && LastPressedJumpTime > 0)
			{
				IsWallJumping = true;
				IsJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;

				_wallJumpStartTime = Time.time;
				_lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

				WallJump(_lastWallJumpDir);
			}
		}

		if (CanDash() && LastPressedDashTime > 0)
		{
			Sleep(Data.dashSleepTime);

			if (_moveInput != Vector2.zero)
				_lastDashDir = _moveInput;
			else
				_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;

			IsDashing = true;
			IsJumping = false;
			IsWallJumping = false;
			_isJumpCut = false;

			StartCoroutine(nameof(StartDash), _lastDashDir);
		}

		if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
			IsSliding = true;
		else
			IsSliding = false;

		if(!_isDashAttacking)
        {
			if(IsSliding) // Increase gravity if we are falling or past jump apex
            {
				SetGravityScale(0);
            }
			else if(rb.velocity.y < 0 && _moveInput.y < 0)
            {
				SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
				rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFastFallSpeed));
            }
			else if (_isJumpCut)
            {
				SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
				rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed));
			}
			else if(rb.velocity.y < 0)
            {
				SetGravityScale(Data.gravityScale * Data.fallGravityMult);
				rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed));
            }
			else
            {
				SetGravityScale(Data.gravityScale);
            }
        }
		else
        {
			SetGravityScale(0);
        }
	}

	private void FixedUpdate()
	{
		//Handle Run
		if (!IsDashing)
		{
			if (IsWallJumping)
				Run(Data.wallJumpRunLerp);
			else
				Run(1);
		}
		else if (_isDashAttacking)
		{
			Run(Data.dashEndRunLerp);
		}

		//Handle Slide
		if (IsSliding)
			Slide();
	}

	#region Input Calls
	public void OnJumpInput()
    {
		LastPressedJumpTime = Data.jumpInputBufferTime; // Allows for early jump press
    }

	public void OnJumpUpInput()
    {
		if (CanJumpCut() || CanWallJumpCut())
			_isJumpCut = true;
    }

	public void OnDashInput()
    {
		LastPressedDashTime = Data.dashInputBufferTime;
    }
    #endregion

    #region Other methods
    public void SetGravityScale(float scale)
	{
		rb.gravityScale = scale;
	}

	public void Sleep(float duration) // Calls StartCoroutine
    {
		StartCoroutine(nameof(PerformSleep), duration);
    }

	private IEnumerator PerformSleep(float duration)
    {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(duration);
		Time.timeScale = 1;
    }
    #endregion

    #region Run Methods
	private void Run(float lerpAmount)
    {
		float targetSpeed = _moveInput.x * Data.runMaxSpeed;
		targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount); // Lerp does something similar to Ease In/Out graphs in After Effects, but with raw numericals

		float accelRate; 
		if (LastOnGroundTime > 0) // WIll make accelRate larger or smaller depending if character is accelerating or slowing down
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;

		if((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold) // increases speed at the top of a jump
        {
			accelRate *= Data.jumpHangAccelerationMult;
			targetSpeed += Data.jumpHangMaxSpeedMult;
        }

		// Keeps player momentum if they're moving in input direction but faster than max speed
		if(Data.doConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) 
			&& Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
			accelRate = 0; // This stops deceleration, since the rate is 0, no change occurs
        }

		float speedDif = targetSpeed - rb.velocity.x;
		float movement = speedDif * accelRate;

		rb.AddForce(movement * Vector2.right, ForceMode2D.Force);


    }
    private void Turn()
    {
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
		IsFacingRight = !IsFacingRight; // changing the current direction the character is facing
    }
    #endregion

    #region Jump Methods
	private void Jump()
    {
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0; // since we set them to 0, they won't jump again

		// We want to increase the force when falling so jump height doesn't shorten due to downwards momentum
		float force = Data.jumpForce;
		if (rb.velocity.y < 0)
			force -= rb.velocity.y;

		rb.AddForce(Vector2.up * force, ForceMode2D.Impulse); // Impulse means it applies hte force instantly
    }

	private void WallJump(int dir)
    {
		//Ensures we can't call Wall Jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;
		LastOnWallRightTime = 0;
		LastOnWallLeftTime = 0;

		Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
		force.x *= dir; 

		if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
			force.x -= rb.velocity.x; // make sure we apply force in opposite direction of wall

		if (rb.velocity.y < 0) //checks whether player is falling. This ensures the player always reaches our desired jump force or greater
			force.y -= rb.velocity.y;

		//Unlike in the run we want to use the Impulse mode.
		rb.AddForce(force, ForceMode2D.Impulse);
	}
    #endregion

    #region Dash methods
	private IEnumerator StartDash(Vector2 dir)
    {
		LastOnGroundTime = 0;
		LastPressedDashTime = 0;

		float startTime = Time.time;

		_dashesLeft--;
		_isDashAttacking = true;

		SetGravityScale(0); // so player doesn't fall mid-dash

		while(Time.time - startTime <= Data.dashAttackTime)
        {
			rb.velocity = dir.normalized * Data.dashSpeed;
			yield return null;
        }

		startTime = Time.time;
		_isDashAttacking = false;

		SetGravityScale(Data.gravityScale);  // return gravity to normal
		rb.velocity = Data.dashEndSpeed * dir.normalized;

		while(Time.time - startTime <= Data.dashEndTime)
        {
			yield return null;
        }
		IsDashing = false; 
    }
    private IEnumerator RefillDash(int amount) 
	{
		_dashRefilling = true;
		yield return new WaitForSeconds(Data.dashRefillTime);
		_dashRefilling = false;
		_dashesLeft = Mathf.Min(Data.dashAmount, _dashesLeft + 1);
	}
	#endregion

	private void Slide()
    {
		float speedDif = Data.slideSpeed - rb.velocity.y;
		float movement = speedDif * Data.slideAccel;

		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

		rb.AddForce(movement * Vector2.up);
    }

	#region Check Methods
	public void CheckDirectionToFace(bool isMovingRight)
    {
		if (isMovingRight != IsFacingRight)
			Turn();
    }

	private bool CanJump()
    {
		return LastOnGroundTime > 0 && !IsJumping;
    }

	private bool CanWallJump() // If able to jump and on wall, not touching ground
    {
		return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 
			&& (!IsWallJumping || (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
    }

	private bool CanJumpCut()
    {
		return IsJumping && rb.velocity.y > 0; // Checks if player is going down by checking speed of vertical movement
    }

	private bool CanWallJumpCut()
    {
		return IsWallJumping && rb.velocity.y > 0;
    }

	private bool CanDash()
    {
		if (!IsDashing && _dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
        {
			StartCoroutine(nameof(RefillDash), 1);
        }
		return _dashesLeft > 0;
    }

	public bool CanSlide()
    {
		if (LastOnWallLeftTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
			return true;
		else
			return false;
    }
	#endregion

	#region Editor Methods
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
		Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
	}
	#endregion

}

 * */