using TMPro;
using UnityEngine;


public class SliderExample : MonoBehaviour
{
    public UnityEngine.UI.Slider slider;
    public TextMeshProUGUI valueText;
    private GameObject currentLiquid; // Represents only the preview
    private bool isConfirmed = false;
    private Vector3 originalLiquidScale; // Saves the original scale
    private float remainingValue = 100f; // Available value after the first liquid

    void Start()
    {
        slider.value = 100;
        slider.maxValue = 100; // Sets the initial maximum value
        slider.onValueChanged.AddListener(UpdateSliderValue);
    }

    public void SetLiquid(GameObject liquid)
    {
        currentLiquid = liquid;
        isConfirmed = false;
        slider.interactable = true;

        // Save the original scale
        originalLiquidScale = liquid.transform.localScale;

        // Update the slider's maximum value for the new liquid
        slider.maxValue = remainingValue;
        slider.value = remainingValue; // Set the slider value to the maximum available
    }

    public void DisableSlider()
    {
        isConfirmed = true;
        slider.interactable = false;

        // After confirmation, update the remaining value for the next liquid
        remainingValue -= slider.value; 
        if (remainingValue < 0) remainingValue = 0; // Ensure the value does not go below 0
    }

    public void UpdateSliderValue(float value)
    {
        if (isConfirmed || currentLiquid == null) return;

        valueText.text = "Value: " + value.ToString("0");

        // Calculate the new Z size based on the slider value
        float newDepth = originalLiquidScale.z * (value / 100f);

        // Keep X and Y unchanged, scale only Z
        Vector3 newScale = new Vector3(originalLiquidScale.x, originalLiquidScale.y, newDepth);
        currentLiquid.transform.localScale = newScale;

        Debug.Log($"Liquid scale updated: {newScale}");
    }
}


