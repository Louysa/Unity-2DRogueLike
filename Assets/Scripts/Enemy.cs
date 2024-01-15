using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{

    public int playerDamage;
    private Animator animator;
    private Transform target;
    private bool skipMove;
    private bool isMoving;

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            target = playerObject.transform;
        }
        else
        {
            // Handle the case when the player object is not found.
            Debug.LogError("Player object not found!");
        }
        base.Start();
        
        
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir,yDir);
        skipMove = true;
        
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;
        if (Math.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        
        AttemptMove<Player>(xDir,yDir);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player hitPlayer = (Player) other.GetComponent<Player>();
            animator.SetTrigger("enemyAttack");
            hitPlayer.loseFood(playerDamage);
        }
    }

     protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        if (!isMoving)
        {
            animator.SetTrigger("enemyAttack");
            hitPlayer.loseFood(playerDamage);
        }
        
        
    }  
}
