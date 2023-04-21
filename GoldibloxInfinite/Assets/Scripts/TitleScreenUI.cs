using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenUI : MonoBehaviour
{
    [SerializeField] TMP_InputField playerNameInput;
    // Start is called before the first frame update
    void Start()
    {
        DataManager.userDataRetrieved.AddListener(SetCurrentName);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void SubmitName()
    {
        DataManager.Instance.SetUserName(playerNameInput.text);
    }

    private void SetCurrentName()
    {
        playerNameInput.text = DataManager.playerData.UserName;
    }
}
