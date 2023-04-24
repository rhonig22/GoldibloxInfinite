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

    public void CreditsClicked()
    {
        SceneManager.LoadScene(LevelLoader.credits);
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
