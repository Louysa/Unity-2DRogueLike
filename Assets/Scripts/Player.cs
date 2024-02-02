using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    public int wallDamage = 1;
    public  int pointsForFood = 20;
    public int pointsForSoda = 40;
    public float restartLevelDelay = 1f;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;
    
    public  Text foodText;

    
    private Animator animator;
    private BoxCollider2D boxCollider2D;
    private Animator enemyAnimator;
    public  int food = 1000;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        food = GameManager.instance.playerFoodPoint;
    
        foodText.text = "Food: " + food;
        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoint = food;
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;
        foodText.text = "Food: " + food;
        
        base.AttemptMove<T>(xDir,yDir);
        

        RaycastHit2D hit;

        if (Move(xDir, yDir, out hit))
        {   
            SoundManager.instance.randomizeSfx(moveSound1,moveSound2);
        }

        CheckIfGameOver();
        
        GameManager.instance.playersTurn = false;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        boxCollider2D.enabled = true;
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            SoundManager.instance.playSingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
            
    }

   

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }else if (other.gameObject.CompareTag("Food"))
        {

            food += pointsForFood;
            foodText.text = "+ " + pointsForFood + " Food: "+ food;
            SoundManager.instance.randomizeSfx(eatSound1,eatSound2);
            other.gameObject.SetActive(false);
            
        }else if (other.gameObject.CompareTag("Soda"))
        {
            food += pointsForSoda;
            foodText.text = "+ " + pointsForSoda + " Food: "+ pointsForSoda;
            SoundManager.instance.randomizeSfx(drinkSound1,drinkSound2);
            other.gameObject.SetActive(false);
        }
        
    }  

    protected override void OnCantMove<T>(T component)
    {   
        
        Wall hitwall = component as Wall;
        hitwall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
        
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void loseFood(int lose)
    {
        animator.SetTrigger("playerHit");
        food -= lose;
        foodText.text = "-" + lose + " Food: "+ food;
        CheckIfGameOver();
    }

    void Update()
    {
        if(!GameManager.instance.playersTurn)return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;
        
        if(horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal,vertical);

    }
}
