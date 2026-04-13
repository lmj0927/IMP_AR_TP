using System.Collections;
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

    Coroutine filterCoroutine;

    void Start()
    {
        filterButton.onClick.AddListener(OnClickFilterButton);
        leftFilterTime = filterTime;
        filterGaugeSlider.maxValue = filterTime;
        filterGaugeSlider.value = filterTime;
        mainCam = Camera.main;
    }

    void Update()
    {
        if (isOnFilter)
        {
            if (leftFilterTime <= 0)
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
        if (filterCoroutine != null)
            StopCoroutine(filterCoroutine);
        filterCoroutine = StartCoroutine(FilterEffectCoroutine());
    }

    IEnumerator FilterEffectCoroutine()
    {
        if (isOnFilter)
        {
            mainCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Enemy"));
            
            while (filterVolume.weight > 0)
            {
                filterVolume.weight -= Time.deltaTime * 2;
                yield return null;
            }
        }
        else
        {
            mainCam.cullingMask |= 1 << LayerMask.NameToLayer("Enemy");
            while (filterVolume.weight < 1)
            {
                filterVolume.weight += Time.deltaTime * 2;
                yield return null;
            }
        }
        isOnFilter = !isOnFilter;
    }

    public void IncreaseLeftFilterTime(float increaseAmount)
    {
        leftFilterTime += increaseAmount;
        if (leftFilterTime > filterTime)
            leftFilterTime = filterTime;
    }
}
