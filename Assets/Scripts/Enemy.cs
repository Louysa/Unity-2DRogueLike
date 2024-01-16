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
    
   

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
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
        
        AttemptMove<Enemy>(xDir,yDir);
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
       /* Player hitPlayer = component as Player;
        animator.SetTrigger("enemyAttack");
        hitPlayer.loseFood(playerDamage);
        
        */
        Debug.Log("You are in Enemy OnCantMove");
    }  
}
