using UnityEngine;

/// <summary>
/// Turns a child HP indicator on or off and exposes its active state for other systems.
/// </summary>
public class HpBox : MonoBehaviour
{
    [SerializeField] private GameObject onHpBox;
    private bool isOn = true;
    
    public void OffHpBox()
    {
        isOn = false;
        onHpBox.SetActive(false);
    }

    public void OnHpBox()
    {
        isOn = true;
        onHpBox.SetActive(true);
    }

    public bool GetIsOn()
    {
        return isOn;
    }
}
