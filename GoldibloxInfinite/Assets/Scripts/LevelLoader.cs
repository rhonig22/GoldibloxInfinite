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
    private readonly float waitTime = .25f;
    private readonly int[] levelTypeList = new int[] { 
        -1, -1, -1, -1, -1, 1, 2, 1, 3, 0,
        1, 1, 2, 3, 3, 0, 2, 3, 1, 0 };
    private int[][] levelMap = new int[4][];
    private bool isLoading = false;

    // Start is called before the first frame update
    void Start()
    {
        DataManager.currentLevel = SceneManager.GetActiveScene().buildIndex;
        levelMap[0] = new int[] { 10, 13 };
        levelMap[1] = new int[] { 6, 8, 19 };
        levelMap[2] = new int[] { 7, 11, 14, 15, 16 };
        levelMap[3] = new int[] { 9, 12, 17, 18 };
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
        if (DataManager.currentLevel == startLevel)
            nextLevel = startTimerLevel;
        else
        {
            int nextLevelType = levelTypeList[DataManager.currentLevel];
            int[] nextOptions = levelMap[nextLevelType];
            int nextIndex = Random.Range(0, nextOptions.Length);
            nextLevel = nextOptions[nextIndex];
        }

        SceneManager.LoadScene(nextLevel);
    }

    public void GameOver()
    {
        SceneManager.LoadScene(gameOverMenu);
    }
}
