using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI CurrentDate;
    public TextMeshProUGUI TimerText;
    private float TimeCount;
  


    void Start()
    {
        CurrentDate.text = DateTime.Now.ToString("yyyy.MM.dd");
        
    }

    void Update()
    {
        TimeCount += Time.deltaTime;
        int minutes = Mathf.FloorToInt(TimeCount / 60);
        int secs = Mathf.FloorToInt(TimeCount % 60);

        TimerText.text = string.Format("{0:00}:{1:00}", minutes, TimeCount);
    }

    
}
