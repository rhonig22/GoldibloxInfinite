using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRB;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private SpawnManager spawnManager;
    private AudioSource audioSource;
    private int jumpForce = 10;
    private int additionalJumpForce = 8;
    private int speed = 8;
    private int jumps = 0;
    private const int originalGravity = 2;
    private float teleportDistance = 3.5f;
    private float teleportTime = .2f;
    private float maxVelocity = 12f;
    private float deathWaitTime = 1f;
    private float teleportCooldownTime = 1f;
    private float horizontalInput = 0f;
    private bool facingRight = true;
    private bool canTeleport = true;
    private bool grounded = false;
    private bool isTeleporting = false;
    private bool isDead = false;
    private bool jump = false;
    private bool teleport = false;
    private Vector3 currentVelocity = Vector3.zero;
    [SerializeField] Material normalMaterial;
    [SerializeField] Material translucent;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip deathSound;
    [Range(0, .3f)][SerializeField] private float movementSmoothing = .01f;
    public bool isEndgame = false;
    public UnityEvent startEndCredits = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        audioSource = GetComponent<AudioSource>();
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
        if (horizontalInput > 0 )
        {
            facingRight= true;
        }
        else if (horizontalInput < 0 )
        {
            facingRight= false;
        }

        // Control movement and teleporting
        if (Input.GetButtonDown("Teleport") && canTeleport)
        {
            teleport = true;
        }

        // Control jumping
        if (Input.GetButtonDown("Jump") && (grounded || jumps > 0))
        {
            jump = true;
        }
    }

    private void FixedUpdate()
    {
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
        playerRB.AddForce(Vector3.up * (grounded ? jumpForce : additionalJumpForce), ForceMode2D.Impulse);
        if (!grounded)
            jumps--;
        grounded = false;
        jump = false;
    }

    private void Move(float xSpeed)
    {
        Vector3 targetVelocity = new Vector2(xSpeed * 60f, playerRB.velocity.y);
        // And then smoothing it out and applying it to the character
        playerRB.velocity = Vector3.SmoothDamp(playerRB.velocity, targetVelocity, ref currentVelocity, movementSmoothing);
    }

    private float GetRayCastDistance(float testDistance)
    {
        Vector2 boxSize = boxCollider.size;
        boxSize.y = 0;
        Vector2 position = playerRB.position + (testDistance > 0 ? boxSize / 2 : -boxSize / 2);
        RaycastHit2D[] hits = Physics2D.RaycastAll(position, Vector3.right * Math.Abs(testDistance)/testDistance, testDistance);
        int index = 0;
        bool found = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.collider != boxCollider && hit.collider != null && hit.rigidbody != null && !hit.collider.CompareTag("Spike"))
            {
                found = true;
                index = i;
                break;
            }
        }

        if (found)
        {
            float dist = hits[index].distance;
            return dist;
        }
        else
        {
            return testDistance;
        }
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
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            grounded |= Vector2.Angle(normal, Vector2.up) < 90;
        }
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
        StartCoroutine(RefreshTeleport());
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

        if (collision.gameObject.CompareTag("Jump"))
        {
            Destroy(collision.gameObject);
            jumps++;
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
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead)
            return;

        if (collision.gameObject.CompareTag("Spike") && !isTeleporting)
        {
            PlayerDeath();
        }
    }

    public void PlayerDeath()
    {
        isDead= true;
        DataManager.Instance.IncreaseDeath();
        Vector2 currentVelocity = playerRB.velocity;
        currentVelocity.y = 0;
        playerRB.velocity = currentVelocity;
        float xForce = UnityEngine.Random.Range(-2f, 2f);
        playerRB.AddForce(Vector3.up * jumpForce + Vector3.right * xForce, ForceMode2D.Impulse);
        spriteRenderer.flipY = true;
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
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
                canTeleport = false;
                break;
            case 2:
                DataManager.Instance.EndLevelTwo();
                SceneManager.LoadScene(1);
                break;
        }
    }

    public void EndGameLogic()
    {
        isDead = true;
        isEndgame = true;
    }
}
