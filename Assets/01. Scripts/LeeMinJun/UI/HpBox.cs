using UnityEngine;

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
