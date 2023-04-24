using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TopTenNames;
    [SerializeField] TextMeshProUGUI TopTwentyNames;
    [SerializeField] TextMeshProUGUI TopTenScores;
    [SerializeField] TextMeshProUGUI TopTwentyScores;

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
        TopTenNames.text = string.Empty;
        TopTwentyNames.text = string.Empty;
        TopTenScores.text = string.Empty;
        TopTwentyScores.text = string.Empty;

        DataManager.Instance.GetHighScores(20, (items) =>
        {
            for (int i = 0; i < 10 && i < items.Length; i++)
            {
                AddItemToScoreList(items[i], TopTenNames, TopTenScores);
            }

            for (int i = 10; i < 20 && i < items.Length; i++)
            {
                AddItemToScoreList(items[i], TopTwentyNames, TopTwentyScores);
            }
        });
    }

    private void AddItemToScoreList(LootLockerLeaderboardMember item, TextMeshProUGUI nameList, TextMeshProUGUI scoreList)
    {
        nameList.text += item.player.name + "\r\n";
        scoreList.text += item.score + "\r\n";
    }
}
