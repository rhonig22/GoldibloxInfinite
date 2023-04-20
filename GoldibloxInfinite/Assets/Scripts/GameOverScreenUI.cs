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

    // Start is called before the first frame update
    void Start()
    {
        roomScore.text = DataManager.roomCount + " x " + roomValue;
        deathScore.text = DataManager.deathCount + " x " + deathValue;
        timeScore.text = DataManager.totalTime + " x " + timeValue;
        bonusScore.text = DataManager.bonusCount + " x " + bonusValue;
        totalScore.text = "" + (DataManager.roomCount * roomValue + DataManager.deathCount * deathValue + DataManager.totalTime * timeValue + DataManager.bonusCount * bonusValue);
    }

    public void StartGame()
    {
        DataManager.Instance.Restart();
        SceneManager.LoadScene(1);
    }
}
