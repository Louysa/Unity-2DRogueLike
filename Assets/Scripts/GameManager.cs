    using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public bool gameIsOn = false;
    public float levelStartsDelay = 2f;
    public float turnDelay = .1f;
    public BoardManager boardScript;
    public static int level = 5;

    public static GameManager instance = null;

    public int playerFoodPoint = 100;
    [HideInInspector] public bool playersTurn = true;

    private List<Enemy> enemies = new List<Enemy>();
    private bool enemiesMoving;
    public Text levelText;
    
    
    private GameObject levelImage;
    private bool doingSetup;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        
      //  InitGame();
    }
    
     void OnEnable()
     {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
       
    }
     
        
    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function
        //to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        
    }
      
   
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        level++;
        Debug.Log("Loaded level: "+ level);
        InitGame();
       
    }

    

    private void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
            return;
        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    public void GameOver()
    {
        //Set levelText to display number of levels passed and game over message
        levelText.text = "After " + level + " days, you starved.";

        //Enable black background image gameObject.
        levelImage.SetActive(true);

        //Disable this GameManager.
        enabled = false;
    }
    
    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
       
        
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        
        levelText.text = "Day " + level;
        
        levelImage.SetActive(true);
        

        
        Invoke("HideLevelImage",levelStartsDelay);
        
        
        enemies.Clear();
        boardScript.SetupScene(level);
        
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(turnDelay);
        }

        playersTurn = true;
        enemiesMoving = false;
        
    }
}
