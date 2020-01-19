using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITexts : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI centralScreenText;
    public TextMeshProUGUI CentralScreenText
    {
        get => centralScreenText;
        set => centralScreenText = value;
    }

    [SerializeField] TextMeshProUGUI scoreText;
    public TextMeshProUGUI ScoreText
    {
        get => scoreText;
        set => scoreText = value;
    }

    [SerializeField] TextMeshProUGUI timeText;
    public TextMeshProUGUI TimeText
    {
        get => timeText;
        set => timeText = value;
    }
}
