using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUXManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI deathText;
    [SerializeField] TextMeshProUGUI roomsText;
    private readonly Color32 redCountdown = new Color32(255, 25, 25, 255);
    private readonly Color32 standardColor = new Color32(255, 203, 13, 255);

    // Update is called once per frame
    void Update()
    {
        UpdateText();
        if (DataManager.timer < 10)
        {
            timerText.color = redCountdown;
        }
    }

    void UpdateText()
    {
        timerText.text = "Countdown: " + DataManager.timer.ToString("0");
        deathText.text = "Deaths: " + DataManager.deathCount;
        roomsText.text = "Rooms: " + DataManager.roomCount;
    }
}
