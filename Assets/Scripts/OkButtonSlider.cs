using UnityEngine;

public class OkButtonSlider : MonoBehaviour
{
    public void OnConfirmButtonClicked()
    {
        if (LiquidManager.Instance != null)
        {
            LiquidManager.Instance.ConfirmLiquid();
        }
    }

   
}