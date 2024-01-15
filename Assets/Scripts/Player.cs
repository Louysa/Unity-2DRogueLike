using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    public int wallDamage = 1;
    public  int pointsForFood = 10;
    public int pointsForSoda = 20;
    public float restartLevelDelay = 1f;

    public  Text foodText;
    public Text levelText;
    private Animator animator;

    public  int food = 100;

    private bool isMoving;
    protected override void Start()
    {
        animator = GetComponent<Animator>();
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
        CheckIfGameOver();
        GameManager.instance.playersTurn = false;
        
    }

    private void CheckIfGameOver()
    {
        if(food <= 0)
            GameManager.instance.GameOver();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            Debug.Log("You hit Exit!");
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }else if (other.gameObject.CompareTag("Food"))
        {
            Debug.Log("You hit Food!");

            food += pointsForFood;
            foodText.text = "+ " + pointsForFood + " Food: "+ food;

            other.gameObject.SetActive(false);
            
        }else if (other.gameObject.CompareTag("Soda"))
        {
            Debug.Log("You hit Soda!");

            food += pointsForSoda;
            foodText.text = "+ " + pointsForSoda + " Food: "+ pointsForSoda;
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
