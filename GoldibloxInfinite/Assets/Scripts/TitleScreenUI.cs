using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenUI : MonoBehaviour
{
    public void LoadMenu()
    {
        SceneManager.LoadScene(LevelLoader.mainMenu);
    }
}
