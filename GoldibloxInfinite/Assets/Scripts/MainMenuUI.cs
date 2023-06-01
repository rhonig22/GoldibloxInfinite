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
    private readonly int maxLength = 24;
    private readonly float maxMusicVolume = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        DataManager.userDataRetrieved.AddListener(SetCurrentName);
        if (DataManager.playerData != null && DataManager.playerData.UserName != string.Empty && DataManager.dataRetrieved)
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

    public void CreditsClicked()
    {
        SceneManager.LoadScene(LevelLoader.credits);
    }

    public void ReturnToMenuClicked()
    {
        SceneManager.LoadScene(LevelLoader.mainMenu);
    }

    public void OptionsClicked()
    {
        SceneManager.LoadScene(LevelLoader.options);
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

        if (playerNameInput != null)
            playerNameInput.text = newName;

        if (nameText != null)
            nameText.text = newName;

        if (submit)
            SubmitNameClicked();
    }

    public void SetMusicVolume(float vol)
    {
        GameObject.Find("MusicSource").GetComponent<AudioSource>().volume = vol * maxMusicVolume;
    }

    public void SetEffectsVolume(float vol)
    {
        DataManager.Instance.SetEffectsVolume(vol);
    }
}
