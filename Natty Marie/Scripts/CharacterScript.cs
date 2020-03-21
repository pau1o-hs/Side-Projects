using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour {

    [Range(1, 4)]
    public int player = 1;

    [HideInInspector] public int currentHealth = 3;
    [HideInInspector] public Rigidbody2D rb;

    [HideInInspector] public Animator anim;
    AudioSource audSource;
    public AudioClip[] clip_Footstep;
    public AudioClip clip_Jump, clip_Land, clip_PushingBox;

    public float moveSpeed = .1f;
    public float jumpPower = 6, airSpeed = 5, airControl = 5;
    public float groudCheckDistance;
    public bool enableMovements = true, deadByFall;
    float stdGCD = 0.3f, jumpGCD = .2f;
   
    bool onGround = true, checkGround = true, isPushing = true;
    bool onLadder;

    enum OffGroundState { Airborne, Ladder };
    OffGroundState offGround;

    public CS_CanvasManager cs_Canvas;
    public GameObject P_Dust;

	// Use this for initialization
	void Start () {

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audSource = GetComponent<AudioSource>();

        offGround = OffGroundState.Airborne;

        cs_Canvas.m_Character = this;
        this.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate () {

        if (currentHealth <= 0 || GameObject.FindObjectOfType<FollowCursor>().GetComponent<Light>().range <= 4) {

            currentHealth = 0;
            enableMovements = false;

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
                anim.SetTrigger("Die");

            this.enabled = false;
            return;
        }

        float h;
        float v;
        bool jump;

        if (enableMovements) {
            //GROUNDED INPUTS
            h = Input.GetAxisRaw("P" + player + "_Horizontal");
            v = Input.GetAxisRaw("P" + player + "_Vertical");
            jump = Input.GetButtonDown("P" + player + "_Jump");
        }
        else {
            h = 0;
            v = 0;
            jump = false;
        }

        if (rb.velocity.y <= -20)
            deadByFall = true;

        if (rb.velocity.y <= -50)
            TakeDamage(3);

        if (checkGround)
            CheckGround(h);
        else onGround = false;

        if (onGround) {

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Land"))
                rb.velocity = new Vector2(h * moveSpeed / 2, rb.velocity.y);
            else
                Grounded(h, v, jump);
        }
        else {

            switch (offGround) {

                case OffGroundState.Airborne:
                    Airborne(h);
                    rb.gravityScale = 3;
                    break;

                case OffGroundState.Ladder:
                    OnLadder();
                    rb.gravityScale = 0;
                    break;
            }
        }

        if (h == 0 && rb.velocity.x != 0) 
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, 10 * Time.deltaTime), rb.velocity.y);

        anim.SetBool("OnGround", onGround);
    }

    void CheckGround(float h)
    {
        //CHECK GROUND
        RaycastHit2D rayA = Physics2D.Raycast(transform.position + Vector3.down * 1f + Vector3.left * .25f, Vector2.down * 1.5f, groudCheckDistance, 1 << LayerMask.NameToLayer("Default"));
        RaycastHit2D rayB = Physics2D.Raycast(transform.position + Vector3.down * 1f + Vector3.right * .25f, Vector2.down * 1.5f, groudCheckDistance, 1 << LayerMask.NameToLayer("Default"));

        Debug.DrawLine(transform.position + Vector3.down * 1f + Vector3.left * .25f, transform.position + (Vector3.left * .25f) + Vector3.down * groudCheckDistance, Color.red);

        bool isOnGround = (rayA && !rayA.collider.isTrigger) || (rayB && !rayB.collider.isTrigger);

        if ((onGround && isOnGround) || (!onGround && isOnGround && rb.velocity.y <= 0)) {

            //JUMP EFFECTS
            if (!onGround) {

                GameObject.Instantiate(P_Dust, transform.position + Vector3.down, transform.rotation, null);
                SELand();
                
                if (rb.velocity.y <= -5)
                    Camera.main.GetComponent<Animator>().SetTrigger("Shake");
            }

            onGround = true;
            groudCheckDistance = stdGCD;
        }
        else
            onGround = false;

        //FLIP CHARACTER
        if (h != 0 && !isPushing) {
            if (h > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    Transform dynamicObj;
    void Grounded(float h, float v, bool jump)
    {
        //bool groundedAnim = anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk");

        //MOVE CHARACTER
        if (h != 0 /*&& groundedAnim*/)
            rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);
        else rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, 10 * Time.deltaTime), rb.velocity.y);

        //SET ANIMATIONS
        if (h != 0)
            anim.SetBool("Moving", true);
        else anim.SetBool("Moving", false);

        //MOVE OBJECT
        Debug.DrawRay(transform.position, Vector2.right * transform.localScale.x, Color.red);
        bool interactInput = Input.GetButton("P" + player + "_Push");

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, .5f, 1 << LayerMask.NameToLayer("Movable"));
        if (hit.collider != null && interactInput) {

            dynamicObj = hit.transform.parent;
            dynamicObj.GetComponent<Rigidbody2D>().velocity = new Vector2(1 * h, dynamicObj.GetComponent<Rigidbody2D>().velocity.y);
            rb.velocity = new Vector2(1 * h, rb.velocity.y);
            isPushing = true;
        }
        else
            isPushing = false;

        if (isPushing && h != 0) {

            anim.SetBool("Pushing", true);
            anim.SetFloat("Right", transform.localScale.x * h);
            dynamicObj.GetComponent<Rigidbody2D>().constraints &= ~RigidbodyConstraints2D.FreezePositionX;
            dynamicObj.GetComponent<Rigidbody2D>().constraints &= ~RigidbodyConstraints2D.FreezePositionY;

            if (!audSource.isPlaying) {
                audSource.pitch = 1;
                audSource.volume = 1;
                audSource.PlayOneShot(clip_PushingBox);
            }
        }
        else {

            anim.SetBool("Pushing", false);

            if (dynamicObj != null)
                dynamicObj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        //JUMP
        if (jump) {
            Jump();
            isPushing = false;

            anim.SetBool("Pushing", false);

            if (dynamicObj != null)
                dynamicObj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void Airborne(float h)
    {
        //CONTROL IN AIR
        if (h != 0)
            rb.velocity = new Vector2(h * airSpeed, rb.velocity.y);

        //CLIMB
        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up * .2f, Vector2.right * h, .75f, 1 << LayerMask.NameToLayer("Climber"));
        if (hit.collider != null && rb.velocity.y <= 0 && h != 0) {
            anim.SetTrigger("Hang");
            Jump();
        }
    }

    void OnLadder()
    {
        float h = Input.GetAxisRaw("P" + player + "_Horizontal");
        float v = Input.GetAxisRaw("P" + player + "_Vertical");
        bool jump = Input.GetButtonDown("P" + player + "_Jump");

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Ladder")) {
            anim.SetBool("Climbing", true);
            anim.SetTrigger("EnterLadder");
        }

        Debug.DrawRay(transform.position + Vector3.up * 1f, Vector2.up * .3f);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position + Vector3.up * 1f, Vector2.up, .3f, 1 << LayerMask.NameToLayer("Ladder"));

        anim.SetFloat("Up", v);
        if (hit2.collider == null) {

            if (v < 0)
                rb.velocity = new Vector2(h / 2 * moveSpeed, v * moveSpeed);
            else {
                rb.velocity = new Vector2(h / 2 * moveSpeed, Mathf.Lerp(rb.velocity.y, 0, 10 * Time.deltaTime));
                anim.SetFloat("Up", 0);
            }
        }
        else 
            rb.velocity = new Vector2(h / 2 * moveSpeed, v * moveSpeed);

        Debug.DrawRay(transform.position + Vector3.down * 1f, Vector2.down *groudCheckDistance);
        //DISABLE LADDER
        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.down * 1f, Vector2.down, groudCheckDistance, 1 << LayerMask.NameToLayer("Default"));
        if (hit.collider != null && !hit.collider.isTrigger && v < 0) {

            onGround = true;
            checkGround = true;
            groudCheckDistance = stdGCD;
            anim.SetBool("Climbing", false);
            offGround = OffGroundState.Airborne;
        }

        if (jump) {
            offGround = OffGroundState.Airborne;
            Jump();
        }
    }

    void Jump()
    {
        onGround = false;
        isPushing = false;
        groudCheckDistance = jumpGCD;
        checkGround = true;
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        rb.gravityScale = 1;

        if (audSource.isPlaying)
            audSource.Stop();

        anim.SetBool("Climbing", false);
        GameObject.Instantiate(P_Dust, transform.position + Vector3.down, transform.rotation, null);
    }

    void TakeDamage(int damage)
    {
        Jump();
        cs_Canvas.DamageDisplay();
        currentHealth -= damage;

        if (currentHealth <= 0)
            enableMovements = false;
    }

    float damageRate = 0.25f;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 11 && Time.time > damageRate) {

            damageRate = Time.time + 0.25f;
            TakeDamage(1);
        }

        if (other.gameObject.layer == 13)
            TakeDamage(1);

        if (deadByFall)
            TakeDamage(3);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        float v = Input.GetAxisRaw("P" + player + "_Vertical");

        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.down * 1f, Vector2.down, groudCheckDistance, 1 << LayerMask.NameToLayer("Default"));
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position + Vector3.up * 1f, Vector2.up, .3f, 1 << LayerMask.NameToLayer("Ladder"));

        if (other.gameObject.layer == 9 && hit2.collider != null && (hit.collider != null && v > 0 || 
            hit.collider == null) && v != 0 && rb.velocity.y <= 0) {

            //ENABLE LADDER
            checkGround = false;
            offGround = OffGroundState.Ladder;
        }

        if (other.transform.name == "Door") {

            if (other.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Opened")) {

                if (other.transform.name != "LinkedDoor") {
                    StartCoroutine(CS_GameManager.instance.EndLevel());
                    enableMovements = false;
                }
            }
            else if (Input.GetButtonDown("P" + player + "_Interact")) CS_GameManager.instance.UpdateEvent();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 9) {

            //DISABLE LADDER
            checkGround = true;
            rb.gravityScale = 3;
            anim.SetBool("Climbing", false);
            offGround = OffGroundState.Airborne;
        }
    }

    public void SEFootstep(int index)
    {
        if (audSource.isPlaying)
            audSource.Stop();

        audSource.volume = .25f;
        audSource.pitch = 1.5f;

        audSource.PlayOneShot(clip_Footstep[index]);
    }

    public void SEJump()
    {
        if (audSource.isPlaying)
            audSource.Stop();

        audSource.volume = .5f;
        audSource.pitch = 1f;

        audSource.PlayOneShot(clip_Jump);
    }

    public void SELand()
    {
        if (audSource.isPlaying)
            audSource.Stop();

        audSource.volume = .5f;
        audSource.pitch = 1.5f;

        audSource.PlayOneShot(clip_Land);
    }
}
