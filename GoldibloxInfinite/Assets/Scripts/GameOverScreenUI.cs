using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreenUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomScore;
    [SerializeField] TextMeshProUGUI deathScore;
    [SerializeField] TextMeshProUGUI timeScore;
    [SerializeField] TextMeshProUGUI bonusScore;
    [SerializeField] TextMeshProUGUI totalScore;
    private int roomValue = 100;
    private int deathValue = -20;
    private int timeValue = 1;
    private int bonusValue = 200;
    private float waitTime = .5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScores());
    }

    private IEnumerator LoadScores()
    {
        yield return new WaitForSeconds(waitTime);
        roomScore.text = DataManager.roomCount + " x " + roomValue;
        yield return new WaitForSeconds(waitTime);
        deathScore.text = DataManager.deathCount + " x " + deathValue;
        yield return new WaitForSeconds(waitTime);
        timeScore.text = DataManager.totalTime + " x " + timeValue;
        yield return new WaitForSeconds(waitTime);
        bonusScore.text = DataManager.bonusCount + " x " + bonusValue;
        yield return new WaitForSeconds(waitTime);
        int total = (DataManager.roomCount * roomValue + DataManager.deathCount * deathValue + DataManager.totalTime * timeValue + DataManager.bonusCount * bonusValue);
        totalScore.text = "" + total;
        DataManager.Instance.SubmitLootLockerScore(total);
    }

    public void StartGame()
    {
        DataManager.Instance.Restart();
        SceneManager.LoadScene(LevelLoader.startLevel);
    }

    public void BackToMenu()
    {
        DataManager.Instance.Restart();
        SceneManager.LoadScene(LevelLoader.mainMenu);
    }
}
