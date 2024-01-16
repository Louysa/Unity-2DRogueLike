using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;  
public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;
        public Count(int min,int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public Camera mainCamera;
    
    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] innerWallTiles;
    public GameObject[] enemyTiles;
    public GameObject[] foodTiles;
    public GameObject[] outerWallTiles;

    public Transform boardHolder;
    public List<Vector3> gridPositions = new List<Vector3>();

    void InitalialiseList()
    {
        gridPositions.Clear();

        for (int i = 0; i < columns-1; i++)
        {
            for (int j = 0; j < rows-1; j++)
            {
                gridPositions.Add(new Vector3(i, j, 0f));
            }
        }

    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        for (int i = -1; i < columns+1; i++)
        {
            for (int j = -1; j < rows+1; j++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if(i == -1 || i == columns ||  j == -1 || j == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                GameObject instance = Instantiate(toInstantiate, new Vector2(i, j), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPositon = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPositon;

    }
    void LayoutObjectAtRandom(GameObject[]tileArray,int min,int max)
    {
        int objectCount = Random.Range(min, max + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        mainCamera = Camera.main;
        
        
        if (level > 5)
        {
            wallCount = new Count(10, 18);
            foodCount = new Count(3, 8);
            columns = 16;
            rows = 16;
            if (mainCamera != null)
            {
                mainCamera.orthographicSize = 10f;
                mainCamera.transform.position = new Vector3(columns / 2f - 0.5f, rows / 2f - 0.5f, -10f);
            }
            else
            {
                Debug.LogError("Main camera not found!");
            }
        }
        BoardSetup();
        InitalialiseList();
        LayoutObjectAtRandom(innerWallTiles,wallCount.minimum,wallCount.maximum);
        LayoutObjectAtRandom(foodTiles,foodCount.minimum,foodCount.maximum);
        
        int enemyCount = (int)Math.Log(level,2f);
        if (level > 5)
            enemyCount = enemyCount * 3;
            
        
        Debug.Log(enemyCount);
        LayoutObjectAtRandom(enemyTiles,enemyCount,enemyCount);
        Instantiate(exit,new Vector3(columns-1,rows-1,0f),Quaternion.identity);
        
    }

    
}
