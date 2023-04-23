using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TopTen;
    [SerializeField] TextMeshProUGUI TopTwenty;

    // Start is called before the first frame update
    void Start()
    {
        PopulateHighScores();
    }

    public void MenuButtonCLicked()
    {
        SceneManager.LoadScene(LevelLoader.mainMenu);
    }

    private void PopulateHighScores()
    {
        TopTen.text = string.Empty;
        TopTwenty.text = string.Empty;

        DataManager.Instance.GetHighScores(20, (items) =>
        {
            for (int i = 0; i < 10 && i < items.Length; i++)
            {
                AddItemToScoreList(items[i], TopTen);
            }

            for (int i = 10; i < 20 && i < items.Length; i++)
            {
                AddItemToScoreList(items[i], TopTwenty);
            }
        });
    }

    private void AddItemToScoreList(LootLockerLeaderboardMember item, TextMeshProUGUI scoreList)
    {
        scoreList.text += item.rank + "  " + item.player.name + "\t" + item.score + "\r\n";
    }
}
