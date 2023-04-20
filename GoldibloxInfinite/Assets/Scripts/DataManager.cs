using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public static int currentCheckPointIndex { get; private set; } = 0;
    public static int deathCount { get; private set; } = 0;
    public static Vector2 playerVelocity { get; private set; }
    public static Vector3 playerPosition { get; private set; }

    public static int currentLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        // Set up singleton
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetCheckPointIndex(int index)
    {
        currentCheckPointIndex= index;
    }

    public void SetPlayerStartValues(Vector2 velocity, Vector3 position)
    {
        playerVelocity = velocity;
        playerPosition = position;
    }

    public void IncreaseDeath()
    {
        deathCount++;
    }
}
