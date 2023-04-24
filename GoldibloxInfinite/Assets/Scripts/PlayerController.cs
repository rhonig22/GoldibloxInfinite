using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRB;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private SpawnManager spawnManager;
    private Transform eyeTransform;
    private LevelLoader levelLoader;
    private List<GameObject> jumpsToReset = new List<GameObject>();
    private int jumpForce = 11;
    private int additionalJumpForce = 9;
    private int speed = 8;
    private int _jumps = 0;
    private int additionalTime = 0;
    private const int originalGravity = 2;
    private float teleportDistance = 2.5f;
    private float teleportTime = .2f;
    private float maxVelocity = 12f;
    private float deathWaitTime = 1f;
    private float teleportCooldownTime = .8f;
    private float horizontalInput = 0f;
    private float beepWaitTime = .7f;
    private float movementSmoothing = .001f;
    private bool facingRight = true;
    private bool canTeleport = true;
    private bool grounded = false;
    private bool jump = false;
    private bool teleport = false;
    private Vector3 currentVelocity = Vector3.zero;
    [SerializeField] Material normalMaterial;
    [SerializeField] Material translucent;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip plusTimeSound;
    [SerializeField] private AudioClip beepSound;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject eye;
    [SerializeField] private GameObject plusTime;
    [SerializeField] private GameObject[] extraJumps;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource beepAudioSource;
    public bool isEndgame { get; private set; } = false;
    public bool isTeleporting { get; private set; } = false;
    public bool isDead { get; private set; } = false;
    public UnityEvent startEndCredits = new UnityEvent();
    public int Jumps
    {
        get
        {
            return _jumps;
        }
        private set
        {
            _jumps = value;
            for (int i = 0; i < extraJumps.Length; i++)
            {
                if (i < _jumps)
                    extraJumps[i].SetActive(true);
                else
                    extraJumps[i].SetActive(false);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        eyeTransform = eye.GetComponent<Transform>();
        levelLoader = GameObject.Find("LevelLoader")?.GetComponent<LevelLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;

        if (isTeleporting)
            return;

        horizontalInput = Input.GetAxis("Horizontal");
        // Control the sprite for the character
        if (horizontalInput > 0 && !facingRight)
        {
            facingRight= true;
            animator.SetBool("facingRight", true);
            FlipEye(false);
        }
        else if (horizontalInput < 0 && facingRight)
        {
            facingRight= false;
            animator.SetBool("facingRight", false);
            FlipEye(false);
        }

        // Control movement and teleporting
        if (Input.GetButtonDown("Teleport") && canTeleport)
        {
            teleport = true;
        }

        // Control jumping
        if (Input.GetButtonDown("Jump") && (grounded || Jumps > 0))
        {
            jump = true;
        }
    }

    private void FixedUpdate()
    {
        if (isDead)
            return;

        if (isTeleporting)
        {
            Move((facingRight ? 1 : -1) * teleportDistance * Time.fixedDeltaTime / teleportTime);
            return;
        }

        // Control movement and teleporting
        if (teleport)
        {
            StartTeleport();
        }
        else
        {
            Move(horizontalInput * speed * Time.fixedDeltaTime);
        }

        // Cap the player's max velocity
        CapYVelocity();

        // Control jumping
        if (jump)
        {
            StartJump();
        }
    }

    private void CapYVelocity()
    {
        currentVelocity = playerRB.velocity;
        if (currentVelocity.y > maxVelocity)
        {
            currentVelocity.y = maxVelocity;
            playerRB.velocity = currentVelocity;
        }
        else if (currentVelocity.y < -maxVelocity)
        {
            currentVelocity.y = -maxVelocity;
            playerRB.velocity = currentVelocity;
        }
    }

    private void StartJump()
    {
        audioSource.clip = jumpSound;
        audioSource.Play();
        currentVelocity = playerRB.velocity;
        currentVelocity.y = 0;
        playerRB.velocity = currentVelocity;
        if (!grounded)
        {
            Jumps--;
            animator.ResetTrigger("jump");
        }

        animator.SetTrigger("jump");
        playerRB.AddForce(Vector3.up * (grounded ? jumpForce : additionalJumpForce), ForceMode2D.Impulse);

        jump = false;
    }

    private void Move(float xSpeed)
    {
        Vector3 targetVelocity = new Vector2(xSpeed * 60f, playerRB.velocity.y);
        // And then smoothing it out and applying it to the character
        playerRB.velocity = Vector3.SmoothDamp(playerRB.velocity, targetVelocity, ref currentVelocity, movementSmoothing);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGrounding(collision);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckGrounding(collision);
    }

    private void CheckGrounding(Collision2D collision)
    {
        bool startGrounded = grounded;
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            grounded |= Vector2.Angle(normal, Vector2.up) < 90;
        }

        if (!startGrounded && grounded && !isDead)
        {
            animator.ResetTrigger("jump");
            Jumps = 0;
            CompleteAddTimeLogic();
            ResetJumps();
        }
    }

    private void ResetJumps()
    {
        for (int i = 0; i < jumpsToReset.Count; i++)
            jumpsToReset[i].SetActive(true);
        
        jumpsToReset.Clear();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
    }

    private void StartTeleport()
    {
        currentVelocity = playerRB.velocity;
        currentVelocity.y = 0;
        playerRB.velocity = currentVelocity;
        isTeleporting = true;
        spriteRenderer.material = translucent;
        playerRB.gravityScale = 0;
        audioSource.clip = dashSound;
        audioSource.Play();
        animator.SetBool("isDashing", true);
        animator.ResetTrigger("jump");
        StartCoroutine(EndTeleport());
        teleport = false;
    }

    private IEnumerator EndTeleport()
    {
        yield return new WaitForSeconds(teleportTime);
        isTeleporting = false;
        canTeleport = false;
        spriteRenderer.material = normalMaterial;
        playerRB.gravityScale = originalGravity;
        currentVelocity = playerRB.velocity;
        currentVelocity.x = 0;
        playerRB.velocity = currentVelocity;
        animator.SetBool("isDashing", false);
        StartCoroutine(RefreshTeleport());
    }

    private void FlipEye(bool verticalFlip)
    {
        Vector3 pos = eyeTransform.position;
        eyeTransform.GetLocalPositionAndRotation(out pos, out Quaternion rotation);
        if (!verticalFlip)
            pos.x = -1 * pos.x;
        if (verticalFlip)
            pos.y = -1 * pos.y;
        eyeTransform.SetLocalPositionAndRotation(pos, rotation);
    }

    private IEnumerator RefreshTeleport()
    {
        yield return new WaitForSeconds(teleportCooldownTime);
        canTeleport = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead)
            return;

        if (collision.gameObject.CompareTag("Spike") && !isTeleporting)
        {
            PlayerDeath();
        }

        if (collision.gameObject.CompareTag("Enemy") && !isTeleporting)
        {
            TestLightCollision(collision);
        }

        if (collision.gameObject.CompareTag("Jump"))
        {
            collision.gameObject.SetActive(false);
            jumpsToReset.Add(collision.gameObject);
            Jumps++;
        }

        if (collision.gameObject.CompareTag("EndTrigger"))
        {
            EndLevelLogic();
        }

        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            spawnManager.SetCheckPoint(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("EndGame"))
        {
            EndGameLogic();
        }

        if (collision.gameObject.CompareTag("AddTime"))
        {
            AddTimeLogic();
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("StartTimer"))
        {
            StartCoroutine(StartTimerLogic());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead)
            return;

        if (collision.gameObject.CompareTag("Spike") && !isTeleporting)
        {
            PlayerDeath();
        }

        if (collision.gameObject.CompareTag("Enemy") && !isTeleporting)
        {
            TestLightCollision(collision);
        }
    }

    private void AddTimeLogic()
    {
        additionalTime++;
    }

    private void CompleteAddTimeLogic()
    {
        if (additionalTime > 0)
        {
            DataManager.Instance.AddTime(additionalTime);
            additionalTime = 0;
            plusTime.SetActive(true);
            audioSource.clip = plusTimeSound;
            audioSource.Play();
            StartCoroutine(HidePlusTime());
        }
    }

    private IEnumerator HidePlusTime()
    {
        yield return new WaitForSeconds(1f);
        plusTime.SetActive(false);
    }

    private IEnumerator StartTimerLogic()
    {
        if (!DataManager.countdownOn)
        {
            beepAudioSource.clip = beepSound;
            beepAudioSource.pitch = .5f;
            beepAudioSource.Play();
            yield return new WaitForSeconds(beepWaitTime);
            beepAudioSource.Play();
            yield return new WaitForSeconds(beepWaitTime);
            beepAudioSource.pitch = 2f;
            beepAudioSource.Play();
            beepAudioSource.pitch = 1f;
            DataManager.Instance.StartTimer();
        }
    }

    private void TestLightCollision(Collider2D collision)
    {
        Vector3 enemyPos = collision.transform.parent.transform.position;
        Vector3 pos = transform.position;
        enemyPos.z = 0;
        pos.z = 0;
        bool blocked = true;
        Vector2 size = boxCollider.size;
        // Check each corner of the player, if the corner is within the collision, raytrace to the source to test if the line of site is free
        // top left
        pos += new Vector3(-size.x/2, size.y/2, 0);
        if (collision.OverlapPoint(pos))
        {
            blocked &= FindNonCollisionHit(collision, pos, enemyPos - pos);
        }

        // top right
        pos += new Vector3(size.x, 0, 0);
        if (collision.OverlapPoint(pos))
        {
            blocked &= FindNonCollisionHit(collision, pos, enemyPos - pos);
        }

        // bottom right
        pos += new Vector3(0, -size.y, 0);
        if (collision.OverlapPoint(pos))
        {
            blocked &= FindNonCollisionHit(collision, pos, enemyPos - pos);
        }

        // bottom left
        pos += new Vector3(-size.x,0, 0);
        if (collision.OverlapPoint(pos))
        {
            blocked &= FindNonCollisionHit(collision, pos, enemyPos - pos);
        }

        if (!blocked)
        {
            PlayerDeath();
        }
    }

    private bool FindNonCollisionHit(Collider2D collision, Vector3 position, Vector3 direction)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(position, direction, direction.magnitude);
        bool blocked = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.collider != boxCollider && hit.collider != collision)
            {
                blocked = true;
                break;
            }
        }

        return blocked;
    }

    public void PlayerDeath()
    {
        isDead= true;
        DataManager.Instance.IncreaseDeath();
        playerRB.velocity = Vector2.zero;
        playerRB.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        FlipEye(true);
        animator.SetBool("isDead", true);
        audioSource.clip = deathSound;
        audioSource.Play();
        StartCoroutine(Restart());
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(deathWaitTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void EndLevelLogic()
    {
        CompleteAddTimeLogic();
        DataManager.Instance.IncreaseRooms();
        levelLoader.LoadNextLevel();
    }

    public void EndGameLogic()
    {
        isDead = true;
        isEndgame = true;
    }
}
