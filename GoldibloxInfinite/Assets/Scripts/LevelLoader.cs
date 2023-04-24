using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GameObject crossfade;
    private Animator crossfadeAnimator;
    public static readonly int mainMenu = 1;
    public static readonly int gameOverMenu = 4;
    public static readonly int leaderboard = 2;
    public static readonly int startLevel = 3;
    public static readonly int startTimerLevel = 5;
    public static readonly int credits = 23;
    private static readonly float waitTime = .25f;
    private static readonly int[][] levelTypeList = new int[24][] {
        new int[2] { -1, -1 }, new int[2] { -1, -1 }, new int[2] { -1, -1 }, new int[2] { -1, -1 }, new int[2] { -1, -1 },
        new int[2] { -1, 1 }, new int[2] { 3, 2 }, new int[2] { 0, 1 }, new int[2] { 3, 3 }, new int[2] { 1, 0 },
        new int[2] { 2, 1 }, new int[2] { 0, 1 }, new int[2] { 1, 2 }, new int[2] { 2, 3 }, new int[2] { 0, 3 },
        new int[2] { 0, 0 }, new int[2] { 0, 2 }, new int[2] { 1, 3 }, new int[2] { 1, 1 }, new int[2] { 3, 0 },
        new int[2] { 3, 1 }, new int[2] { 2, 0 }, new int[2] { 2, 2 }, new int[2] { -1, -1 }};
    private static readonly int[] exitToEntrance = new int[] { 2, 3, 0, 1 };
    private static readonly int easyRoomCount = 11;
    private static readonly int totalRoomCount = 19;
    private static readonly int easyBuffer = 4;
    private static int[][] levelMapEasy = new int[4][] {
        new int[] { 7, 11, 14, 16 },
        new int[] { 9, 18 },
        new int[] { 10, 13 },
        new int[] { 6 }};
    private static int[][] levelMapHard = new int[4][] {
        new int[] { 15 },
        new int[] { 12, 17 },
        new int[] { 21, 22 },
        new int[] { 8, 19, 20 }};
    private bool isLoading = false;

    // Start is called before the first frame update
    void Start()
    {
        DataManager.currentLevel = SceneManager.GetActiveScene().buildIndex;
        crossfadeAnimator = crossfade.GetComponent<Animator>();
        DataManager.gameOver.AddListener(GameOver);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            GameOver();
        }
    }

    public void LoadNextLevel()
    {
        if (!isLoading)
        {
            isLoading= true;
            crossfadeAnimator.SetBool("EndLevel", true);
            StartCoroutine(FinishLoading());
        }
    }

    IEnumerator FinishLoading()
    {
        yield return new WaitForSeconds(waitTime);
        isLoading= false;
        int nextLevel;
        DataManager.visitedRooms.Add(DataManager.currentLevel);
        if (DataManager.visitedRooms.Count == totalRoomCount)
            DataManager.Instance.AddBonus();

        if (DataManager.currentLevel == startLevel)
            nextLevel = startTimerLevel;
        else
        {
            int nextLevelType = exitToEntrance[levelTypeList[DataManager.currentLevel][1]];
            List<int> nextOptions = new List<int>(levelMapEasy[nextLevelType]);
            if (DataManager.visitedRooms.Count >= easyRoomCount - easyBuffer)
            {
                nextOptions.AddRange(levelMapHard[nextLevelType]);
            }

            nextOptions.Remove(DataManager.currentLevel);
            int nextIndex = Random.Range(0, nextOptions.Count);
            nextLevel = nextOptions[nextIndex];
        }

        SceneManager.LoadScene(nextLevel);
    }

    public void GameOver()
    {
        SceneManager.LoadScene(gameOverMenu);
    }
}
