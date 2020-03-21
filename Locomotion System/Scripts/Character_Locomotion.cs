using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Locomotion : MonoBehaviour
{
	internal Manager_Camera m_Camera;
	internal Vector3 moveInput, moveInputRaw;

	internal float currMoveSpeed;

	internal bool autoInput = true;
	internal bool canMove = true;

	public float moveSpeed;
	public float rotateSpeed;
	public float jumpForce, gravityMultiplier;
	public float groundDistance = .8f;
	public float slopeLimit = .7f;

	public PhysicMaterial zeroFric;
	public PhysicMaterial maxFric;

	RaycastHit groundInfo;
	RaycastHit slopeInfo;
	internal RaycastHit climbInfo;

	Rigidbody rb;
	internal Collider coll;
	Animator animator;

	Vector3 moveDirection, velocity;
	Vector3 groundNormal;
	Vector3 slopeNormal;

	internal float currGroundDistance;
	float slopeAngle;

	internal bool isGrounded = true;
	bool jumpInput;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		coll = GetComponent<Collider>();
		animator = GetComponent<Animator>();
		m_Camera = GetComponentInChildren<Manager_Camera>();

		currMoveSpeed = moveSpeed;
		currGroundDistance = groundDistance;

		m_Camera.transform.parent = null;
	}

	void FixedUpdate()
	{
		Inputs();
		GroundCheck();

		if (isGrounded)
		{
			if (jumpInput)
				Jump();
		}
		else
		{
			Airborne();
			ClimbCheck();
		}

		if (canMove && moveDirection.magnitude > 0)
		{
			FreeMove();
		}

		SetColliderMaterial();
		UpdateAnimator();
	}

	void Inputs()
	{
		if (autoInput)
		{
			moveInput.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveInputRaw.Set(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		}

		moveDirection = moveInput.z * m_Camera.transform.forward + moveInput.x * m_Camera.transform.right;

		if (moveDirection.magnitude > 1)
			moveDirection.Normalize();

		jumpInput = Input.GetButtonDown("Jump");

	}

	void GroundCheck()
	{
		// GET SLOPE
		#region
		Ray slopeCheck0 = new Ray(transform.position + .65f * Vector3.up, transform.forward);
		Ray slopeCheck1 = new Ray(transform.position + .65f * Vector3.up, transform.forward + .25f * transform.right);
		Ray slopeCheck2 = new Ray(transform.position + .65f * Vector3.up, transform.forward - .25f * transform.right);

		float slopeCheckDist;

		if (isGrounded) slopeCheckDist = .65f;
		else slopeCheckDist = 1f;

		Debug.DrawRay(slopeCheck0.origin, slopeCheck0.direction * slopeCheckDist, Color.green);
		//Debug.DrawRay(slopeCheck1.origin, slopeCheck1.direction * slopeCheckDist, Color.green);
		//Debug.DrawRay(slopeCheck2.origin, slopeCheck2.direction * slopeCheckDist, Color.green);

		if (Physics.Raycast(slopeCheck0, out slopeInfo, slopeCheckDist) /*||
			Physics.Raycast(slopeCheck1, out slopeInfo, slopeCheckDist) ||
			Physics.Raycast(slopeCheck1, out slopeInfo, slopeCheckDist)	*/) slopeNormal = slopeInfo.normal;

		else slopeNormal = Vector3.up;

		slopeAngle = Vector3.Dot(Vector3.up, slopeNormal);
		//print(slopeAngle);
		#endregion

		// GROUND
		#region
		float r = ((CapsuleCollider)coll).radius - .2f;

		Vector3 feet = transform.position + .5f * Vector3.up;

		Debug.DrawRay(feet + r * transform.right, Vector3.down * currGroundDistance, Color.red);
		Debug.DrawRay(feet - r * transform.right, Vector3.down * currGroundDistance, Color.red);
		Debug.DrawRay(feet + r * transform.forward, Vector3.down * currGroundDistance, Color.red);
		Debug.DrawRay(feet - r * transform.forward, Vector3.down * currGroundDistance, Color.red);

		if ((Physics.Raycast(feet, Vector3.down, out groundInfo, currGroundDistance) ||
			 Physics.Raycast(feet + r * transform.right, Vector3.down, out groundInfo, currGroundDistance) ||
			 Physics.Raycast(feet - r * transform.right, Vector3.down, out groundInfo, currGroundDistance) ||
			 Physics.Raycast(feet + r * transform.forward, Vector3.down, out groundInfo, currGroundDistance) ||
			 Physics.Raycast(feet - r * transform.forward, Vector3.down, out groundInfo, currGroundDistance)) &&
			(isGrounded || (!isGrounded && rb.velocity.y <= 1.5f)))
		{
			isGrounded = true;
			currGroundDistance = groundDistance;

			// GROUND NORMAL
			groundNormal = groundInfo.normal;

			// GROUND STICK
			if (slopeInfo.collider == null)
				rb.position = Vector3.Lerp(rb.position, new Vector3(rb.position.x, groundInfo.point.y, rb.position.z), 10 * Time.deltaTime);
		}
		else
		{
			groundNormal = Vector3.up;
			isGrounded = false;
		}
		#endregion
	}

	public void ClimbCheck()
	{
		if (animator.GetBool("Climb")) return;

		float sideCheck = .75f;
		Ray climbCheckUp = new Ray(transform.position + 1.8f * Vector3.up, transform.forward);
		Ray climbCheckUpR = new Ray(transform.position + 1.8f * Vector3.up, transform.forward + sideCheck * transform.right);
		Ray climbCheckUpL = new Ray(transform.position + 1.8f * Vector3.up, transform.forward - sideCheck * transform.right);

		Ray climbCheckDown = new Ray(transform.position + 1.5f * Vector3.up, transform.forward);
		Ray climbCheckDownR = new Ray(transform.position + 1.5f * Vector3.up, transform.forward + sideCheck * transform.right);
		Ray climbCheckDownL = new Ray(transform.position + 1.5f * Vector3.up, transform.forward - sideCheck * transform.right);


		Debug.DrawRay(climbCheckUp.origin, climbCheckUp.direction * 1.5f, Color.yellow);
		Debug.DrawRay(climbCheckUpR.origin, climbCheckUpR.direction * 1.5f, Color.yellow);
		Debug.DrawRay(climbCheckUpL.origin, climbCheckUpL.direction * 1.5f, Color.yellow);

		Debug.DrawRay(climbCheckDown.origin, climbCheckDown.direction * 1f, Color.yellow);
		Debug.DrawRay(climbCheckDownR.origin, climbCheckDownR.direction * 1f, Color.yellow);
		Debug.DrawRay(climbCheckDownL.origin, climbCheckDownL.direction * 1f, Color.yellow);

		if ((!Physics.Raycast(climbCheckUp, out climbInfo, 2.5f) && !Physics.Raycast(climbCheckUpR, out climbInfo, 2.5f) && !Physics.Raycast(climbCheckUpL, out climbInfo, 2.5f)) &&
			 (Physics.Raycast(climbCheckDown, out climbInfo, 1f) || Physics.Raycast(climbCheckDownR, out climbInfo, 1f) || Physics.Raycast(climbCheckDownL, out climbInfo, 1f)) &&
			 velocity.y <= 1f)
		{
			climbInfo.point = new Vector3(climbInfo.point.x, transform.position.y + 1.65f, climbInfo.point.z);
			climbInfo.point += .25f * transform.right;

			rb.velocity = Vector3.up * -1;
			animator.SetBool("Climb", true);
		}
	}

	void FreeMove()
	{
		// VELOCITY
		velocity = moveDirection * currMoveSpeed * Time.deltaTime;
		velocity.y = rb.velocity.y;

		// SLOPE LIMIT
		if (slopeAngle < slopeLimit)
		{
			if (slopeAngle > 0.01)
			{
				//if (!isGrounded && velocity.y > 1.5f)
					//	velocity.y = Mathf.MoveTowards(velocity.y, -5, 5 * Time.deltaTime);

				velocity.x /= 5;
				velocity.z /= 5;
			}
		}

		// APPLY VELOCITY
		rb.velocity = Vector3.Lerp(rb.velocity, velocity, 20 * Time.deltaTime);

		// ROTATION
		Quaternion lookForward = Quaternion.LookRotation(moveDirection, Vector3.up);
		rb.rotation = Quaternion.Lerp(transform.rotation, lookForward, rotateSpeed * Time.deltaTime);
	}

	void Jump()
	{
		//if (slopeAngle > 0.2 && slopeAngle < 0.5) return;

		if (animator.IsInTransition(0) || !AnimState().IsName("Locomotion")) return;

		isGrounded = false;
		rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);

		currGroundDistance = .6f;
	}

	void Airborne()
	{
		Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
		rb.AddForce(extraGravityForce);
	}

	void SetColliderMaterial()
	{
		if (!isGrounded || moveDirection.magnitude > 0)
			coll.material = zeroFric;
		else coll.material = maxFric;
	}

	void UpdateAnimator()
	{
		Vector3 relativeFwd = transform.InverseTransformDirection(moveDirection);
		Vector3 relativeUp = transform.InverseTransformDirection(groundNormal);

		animator.SetFloat("Forward", moveInputRaw.normalized.magnitude, 0.05f, Time.deltaTime);
		animator.SetFloat("Turn", Mathf.Atan2(relativeFwd.x, relativeFwd.z), 0.2f, Time.deltaTime);
		animator.SetFloat("Slope", -Mathf.Atan2(relativeUp.z, relativeUp.y), 0.2f, Time.deltaTime);
		animator.SetFloat("Jump", rb.velocity.y);

		animator.SetBool("OnGround", isGrounded);
	}

	public AnimatorStateInfo AnimState()
	{
		return animator.GetCurrentAnimatorStateInfo(0);
	}

	public void JumpLeg(int n)
	{
		if (!animator.IsInTransition(0))
			animator.SetFloat("JumpLeg", n);
	}

}
