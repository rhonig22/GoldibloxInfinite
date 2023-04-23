using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] TMP_InputField playerNameInput;
    [SerializeField] GameObject ViewName;
    [SerializeField] GameObject EditName;
    [SerializeField] TextMeshProUGUI nameText;

    // Start is called before the first frame update
    void Start()
    {
        DataManager.userDataRetrieved.AddListener(SetCurrentName);
        if (DataManager.playerData != null)
            SetCurrentName();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(LevelLoader.startLevel);
    }

    public void LeaderboardClicked()
    {
        SceneManager.LoadScene(LevelLoader.leaderboard);
    }

    public void EditNameClicked()
    {
        ViewName.SetActive(false);
        EditName.SetActive(true);
    }

    public void SubmitNameClicked()
    {
        DataManager.Instance.SetUserName(playerNameInput.text, (string name) => {
            if (name != null)
            {
                SetCurrentName();
                ViewName.SetActive(true);
                EditName.SetActive(false);
            }
        });
    }

    private void SetCurrentName()
    {
        string newName = DataManager.playerData.UserName;
        if (newName == null)
        {
            newName = "Player" + Random.Range(10000, 100000);
        }

        playerNameInput.text = newName;
        nameText.text = newName;
    }
}