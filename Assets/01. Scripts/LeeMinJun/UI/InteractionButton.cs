using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractionButton : MonoBehaviour
{
    private Button button;
    public Button.ButtonClickedEvent onAction;
    public Button.ButtonClickedEvent offAction;

    private bool isOn = false;
    
    [SerializeField] BatteryCollector batteryCollector;
    [SerializeField] GameObject offObject;
    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void Update()
    {
        isOn = batteryCollector.GetCanCollect();
        if(!isOn)
            offObject.SetActive(true);
        else
            offObject.SetActive(false);
    }

    void OnClick()
    {
        if (isOn)
            onAction.Invoke();
        else
            offAction.Invoke();
    }
}
