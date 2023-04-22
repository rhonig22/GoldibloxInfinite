using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public bool isVerticalFollow = false;
    [SerializeField] private Rigidbody2D followRb;
    private GameObject player;
    private Rigidbody2D playerRb;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        SetPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            SetPlayer();

        if (player == null)
            return;

        Vector2 newVelocity;
        if (isVerticalFollow)
        {
            newVelocity = new Vector2(0, playerRb.velocity.y);
        }
        else
        {
            newVelocity = new Vector2(playerRb.velocity.x, 0);
        }

        if (!playerController.isTeleporting && !playerController.isDead)
            followRb.velocity = newVelocity;
        else
            followRb.velocity = Vector2.zero;
    }

    private void SetPlayer()
    {
        player = GameObject.FindWithTag("Player");
        playerRb = player?.GetComponent<Rigidbody2D>();
        playerController = player?.GetComponent<PlayerController>();
    }
}
