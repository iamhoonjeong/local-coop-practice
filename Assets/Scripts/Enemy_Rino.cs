using UnityEngine;

public class Enemy_Rino : Enemy
{
    [Header("Rino details")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedUpRate = 0.6f;
    private float defaultSpeed;
    [SerializeField] private Vector2 impactPower;

    protected override void Start()
    {
        base.Start();

        defaultSpeed = moveSpeed;
    }

    protected override void Update()
    {
        base.Update();

        HandleCharge();
    }

    private void HandleCharge()
    {
        if (canMove == false)
            return;

        HandleSpeedUp();

        rb.linearVelocityX = moveSpeed * facingDir;

        if (isWallDetected)
            WallHit();

        if (!isGroundInFrontDetected)
            TurnAround();
    }

    private void HandleSpeedUp()
    {
        moveSpeed = moveSpeed + Time.deltaTime * speedUpRate;

        if (moveSpeed >= maxSpeed)
            maxSpeed = moveSpeed;
    }

    private void TurnAround()
    {
        SpeedReset();
        canMove = false;
        rb.linearVelocityX = 0;
        Flip();
    }

    private void WallHit()
    {
        canMove = false;
        SpeedReset();
        anim.SetBool("hitWall", true);
        rb.linearVelocity = new Vector2(impactPower.x * -facingDir, impactPower.y);
    }

    private void SpeedReset() => moveSpeed = defaultSpeed;

    private void ChargeIsOver()
    {
        anim.SetBool("hitWall", false);
        Invoke(nameof(Flip), 1);
    }

    protected override void HandleCollision()
    {
        base.HandleCollision();

        if (isPlayerDetected && isGrounded)
            canMove = true;
    }
}
