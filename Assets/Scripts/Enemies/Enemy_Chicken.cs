using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Chicken : Enemy
{
    [Header("Chicken details")]
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
            return;

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
        if (!isGroundInfrontDetected || isWallDetected)
        {
            Flip();
            canMove = false;
            rb.velocity = Vector2.zero;
        }
    }

    private void HandleMovement()
    {
        if (canMove == false)
            return;

        HandleFlip(player.position.x);

        rb.velocity = new Vector2(moveSpeed * facingDir, rb.velocity.y);
    }

    protected override void HandleFlip(float xValue)
    {
        if (xValue < transform.position.x && facingRight || xValue > transform.position.x && !facingRight)
        {
            if (canFlip)
            {
                canFlip = false;
                Invoke(nameof(Flip), .3f);
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
