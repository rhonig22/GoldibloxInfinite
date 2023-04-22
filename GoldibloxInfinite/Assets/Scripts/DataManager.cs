using LootLocker.Requests;
using Newtonsoft.Json;
using System;
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
    public static bool countdownOn { get; private set; } = false;
    public static UserData playerData { get; private set; }
    public static Vector2 playerVelocity { get; private set; }
    public static Vector3 playerPosition { get; private set; }

    public static int currentLevel = 1;
    public static float timer { get; private set; } = 60;
    public static UnityEvent gameOver = new UnityEvent();
    public static UnityEvent userDataRetrieved = new UnityEvent();
    private readonly int plusTime = 10;
    private readonly string leaderboardID = "goldiblox_leaderboard";

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
        StartLootLockerSession();
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
        countdownOn = true;
    }

    public void AddTime(int count)
    {
        int addedTime = count * plusTime;
        timer += addedTime;
        totalTime += addedTime;
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

    private void StartLootLockerSession()
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (!response.success)
            {
                Debug.Log("error starting LootLocker session");
                return;
            }

            SetUserData("", response.player_identifier);
            LootLockerSDKManager.GetPlayerName((response) =>
            {
                if (!response.success)
                {
                    return;
                }

                playerData.UserName = response.name;
                userDataRetrieved.Invoke();
                Debug.Log("successfully started LootLocker session");
            });

            Debug.Log("successfully started LootLocker session");
        });
    }

    public void SubmitLootLockerScore(int score)
    {
        LootLockerSDKManager.SubmitScore(playerData.UserId, score, leaderboardID, (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful");
            }
            else
            {
                Debug.Log("failed: " + response.Error);
            }
        });
    }

    public void SetUserName(string username, Action<string> callback)
    {
        LootLockerSDKManager.SetPlayerName(username, (response) =>
        {
            if (!response.success)
            {
                callback(null);
                return;
            }

            playerData.UserName = username;
            callback(username);
            Debug.Log("successfully started LootLocker session");
        });
    }

    private void SetUserData(string name, string id)
    {
        playerData = new UserData {
            UserName = name,
            UserId = id
        };
    }

    public void GetHighScores(int count, Action<LootLockerLeaderboardMember[]> callback)
    {
        LootLockerSDKManager.GetScoreList(leaderboardID, count, 0, (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful");
                callback(response.items);
            }
            else
            {
                Debug.Log("failed: " + response.Error);
            }
        });
    }
}
