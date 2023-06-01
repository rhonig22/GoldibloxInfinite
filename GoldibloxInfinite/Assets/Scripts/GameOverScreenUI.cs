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
    private int bonusValue = 2000;
    private float waitTime = .5f;

    [SerializeField] TMP_InputField playerNameInput;
    [SerializeField] GameObject ViewName;
    [SerializeField] GameObject EditName;
    [SerializeField] TextMeshProUGUI nameText;
    private readonly int maxLength = 24;

    // Start is called before the first frame update
    void Start()
    {
        if (DataManager.playerData != null && DataManager.playerData.UserName != string.Empty && DataManager.dataRetrieved)
            SetCurrentName();
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

    public void EditNameClicked()
    {
        ViewName.SetActive(false);
        EditName.SetActive(true);
    }

    private void ShowName()
    {
        ViewName.SetActive(true);
        EditName.SetActive(false);
    }

    public void SubmitNameClicked()
    {
        string newName = playerNameInput.text;
        if (newName == null || newName.Length > maxLength)
            newName = newName.Substring(0, maxLength);

        if (newName != DataManager.playerData.UserName)
        {
            DataManager.Instance.SetUserName(newName, (string name) =>
            {
                if (name != null)
                {
                    SetCurrentName();
                    ShowName();
                }
            });
        }
        else
        {
            ShowName();
        }
    }

    private void SetCurrentName()
    {
        string newName = DataManager.playerData.UserName;
        bool submit = false;
        if (newName == null || newName == string.Empty)
        {
            newName = "Player" + Random.Range(10000, 100000);
            submit = true;
        }

        playerNameInput.text = newName;
        nameText.text = newName;
        if (submit)
            SubmitNameClicked();
    }
}
