using UnityEngine;

public class ButtonSelectLiquid : MonoBehaviour
{
    public GameObject liquidPrefab;

    public void OnLiquidButtonClicked()
    {
        if (LiquidManager.Instance != null)
        {
            LiquidManager.Instance.SelectLiquid(liquidPrefab);
        }
        else
        {
            Debug.LogWarning("LiquidManager didnt find!");
        }
    }
}