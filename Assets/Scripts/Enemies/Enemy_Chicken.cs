using UnityEngine;

public class Enemy_Chicken : Enemy
{
    [Header("Chicken Details")]
    [SerializeField] private float aggroDuration;
    [SerializeField] private float detectionRange;

    private Transform player;
    private float aggroTimer;
    private bool canFlip = true;

    protected override void Update()
    {
        base.Update();

        aggroTimer -= Time.deltaTime;

        if (isDead)
        {
            canFlip = false;
            return;
        }

        if (isPlayerDetected)
        {
            canMove = true;
            aggroTimer = aggroDuration;
        }

        if (aggroTimer < 0)
            canMove = false;

        HandleMovement();

        if (isGrounded)
            HandleTurnAround();
    }

    private void HandleTurnAround()
    {
        if (!isGroundInFrontDetected || isWallDetected)
        {
            Flip();
            canMove = false;
            rb.linearVelocityX = 0;
        }
    }

    private void HandleMovement()
    {
        if (canMove == false)
            return;

        HandleFlip(player.transform.position.x);

        rb.linearVelocity = new Vector2(moveSpeed * facingDir, rb.linearVelocityY);
    }

    protected override void HandleFlip(float xValue)
    {
        if (xValue < transform.position.x && facingRight || xValue > transform.position.x && !facingRight)
        {
            if (canFlip)
            {
                canFlip = false;
                Invoke(nameof(Flip), 0.3f);
            }
        }
    }

    protected override void Flip()
    {
        base.Flip();
        canFlip = true;
        FindClosestPlayer();
    }

    private void FindClosestPlayer()
    {
        float closestDistance = float.MaxValue;

        foreach (Player p in playerList)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, p.transform.position);

            if (distanceToPlayer < closestDistance)
            {
                closestDistance = distanceToPlayer;
                player = p.transform;
            }
        }
    }
}
