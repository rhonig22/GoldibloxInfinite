using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public static int currentCheckPointIndex { get; private set; } = 0;
    public static int deathCount { get; private set; } = 0;
    public static int roomCount { get; private set; } = 0;
    public static int bonusCount { get; private set; } = 0;
    public static int totalTime { get; private set; } = 60;
    public static Vector2 playerVelocity { get; private set; }
    public static Vector3 playerPosition { get; private set; }

    public static int currentLevel = 1;
    public static float timer { get; private set; } = 60;
    public static UnityEvent gameOver = new UnityEvent();
    private static bool countdownOn = false;

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

    private void Update()
    {
        if (countdownOn)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                countdownOn= false;
                gameOver.Invoke();
            }
        }
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

    public void IncreaseRooms()
    {
        roomCount++;
    }

    public void StartTimer()
    {
        countdownOn= true;
    }

    public void Restart()
    {
        currentCheckPointIndex = 0;
        deathCount = 0;
        roomCount = 0;
        totalTime = 60;
        bonusCount = 0;
        countdownOn = false;
        timer = 60;
    }
}
