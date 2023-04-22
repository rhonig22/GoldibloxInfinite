using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GameObject crossfade;
    private Animator crossfadeAnimator;
    private readonly int startLevel = 1;
    private readonly int startTimerLevel = 3;
    private readonly float waitTime = .25f;
    private readonly int[] levelTypeList = new int[] { -1, -1, -1, 1, 2, 1, 3, 0, 1, 1, 2, 3 };
    private int[][] levelMap = new int[4][];
    private bool isLoading = false;

    // Start is called before the first frame update
    void Start()
    {
        DataManager.currentLevel = SceneManager.GetActiveScene().buildIndex;
        levelMap[0] = new int[] { 8, 11 };
        levelMap[1] = new int[] { 4, 6 };
        levelMap[2] = new int[] { 5, 9 };
        levelMap[3] = new int[] { 7, 10 };
        crossfadeAnimator = crossfade.GetComponent<Animator>();
        DataManager.gameOver.AddListener(GameOver);
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
        SceneManager.LoadScene(2);
    }
}
