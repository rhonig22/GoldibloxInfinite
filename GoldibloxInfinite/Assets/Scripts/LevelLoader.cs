using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GameObject crossfade;
    private Animator crossfadeAnimator;
    private readonly float waitTime = .25f;
    private readonly int[] levelTypeList = new int[] { -1, 0, 1, 2, 1 };
    private int[][] levelMap = new int[4][];
    private bool isLoading = false;

    // Start is called before the first frame update
    void Start()
    {
        DataManager.currentLevel = SceneManager.GetActiveScene().buildIndex;
        levelMap[0] = new int[] { 2 };
        levelMap[1] = new int[] { 3 };
        levelMap[2] = new int[] { 4 };
        levelMap[3] = new int[] { };
        crossfadeAnimator = crossfade.GetComponent<Animator>();
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
        int[] nextOptions = levelMap[levelTypeList[DataManager.currentLevel]];
        int nextIndex = Random.Range(0, nextOptions.Length);
        SceneManager.LoadScene(nextOptions[nextIndex]);
    }
}
