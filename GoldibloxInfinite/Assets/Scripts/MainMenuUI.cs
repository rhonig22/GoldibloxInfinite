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
    [SerializeField] GameObject hiddenControlsText;
    [SerializeField] GameObject controlsText;
    private readonly int maxLength = 24;

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

    public void CreditsClicked()
    {
        SceneManager.LoadScene(LevelLoader.credits);
    }

    public void EditNameClicked()
    {
        ViewName.SetActive(false);
        EditName.SetActive(true);
        controlsText.SetActive(false);
        hiddenControlsText.SetActive(true);
    }

    private void ShowName()
    {
        ViewName.SetActive(true);
        EditName.SetActive(false);
        controlsText.SetActive(true);
        hiddenControlsText.SetActive(false);
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

    public void SetMusicVolume(float vol)
    {
        GameObject.Find("MusicSource").GetComponent<AudioSource>().volume = vol;
    }

    public void SetEffectsVolume(float vol)
    {
        DataManager.Instance.SetEffectsVolume(vol);
    }
}
