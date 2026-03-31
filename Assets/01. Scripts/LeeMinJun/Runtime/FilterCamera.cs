using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FilterCamera : MonoBehaviour
{
    [SerializeField] private Button filterButton;
    [SerializeField] private Slider filterGaugeSlider;
    [SerializeField] private float filterTime;
    [SerializeField] private Volume filterVolume;
    
    private float leftFilterTime;
    bool isOnFilter = false;
    Camera mainCam;
    
    void Start()
    {
        filterButton.onClick.AddListener(OnClickFilterButton);
        leftFilterTime = filterTime;
        filterGaugeSlider.maxValue = filterTime;
        mainCam = Camera.main;
    }

    void Update()
    {
        if (isOnFilter)
        {
            if(leftFilterTime <= 0)
                ChangeFilter();
            
            leftFilterTime -= Time.deltaTime;
            filterGaugeSlider.value = leftFilterTime;
        }
    }

    private void OnClickFilterButton()
    {
        if (leftFilterTime <= 0)
            return;
        ChangeFilter();
    }

    private void ChangeFilter()
    {
        if (isOnFilter)
        {
            mainCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Enemy"));
            filterVolume.weight = 0;
            filterGaugeSlider.gameObject.SetActive(false);
        }
        else
        {
            mainCam.cullingMask |= 1 << LayerMask.NameToLayer("Enemy");
            filterVolume.weight = 1;
            filterGaugeSlider.gameObject.SetActive(true);
        }
        isOnFilter = !isOnFilter;
    }
}
