using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MovingObject
{

    public int playerDamage;
    private Animator animator;
    private Transform target;
    private BoxCollider2D boxCollider2D;
    private bool skipMove;
    private GameObject playerObject = null;

    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;
    

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        playerObject = GameObject.FindGameObjectWithTag("Player");

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

        Debug.Log("Enemy is trying to move!");
        base.AttemptMove<T>(xDir,yDir);
        Debug.Log("Enemy moved!");
        skipMove = true;
        
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;
        if (Math.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.x > transform.position.x ? 1 : -1;
        }
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        
        AttemptMove<Player>(xDir,yDir);
    }
    
     protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        hitPlayer.loseFood(playerDamage);
        animator.SetTrigger("enemyAttack");
        
        SoundManager.instance.randomizeSfx(enemyAttack1,enemyAttack2);
        
        
        Debug.Log("You are in Enemy OnCantMove");
    }  
}
