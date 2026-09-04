using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PourDetector : MonoBehaviour
{
    public int pourThreshold = 45;
    public Transform origin = null;
    public GameObject streamPrefab = null;

    private bool isPouring = false;
    private Stream currentStream = null;

    private void Update()
    {
        bool pourCheck = CalculatePourAngle() < pourThreshold;
        if (isPouring != pourCheck)
        {
            isPouring = pourCheck;

            if (isPouring)
            {
                StartPour();
            }
            else
            {
                EndPour();
            }
        }
    }
    
    private void StartPour()
    {
        Debug.Log("Pouring started!");
        if (streamPrefab == null || origin == null)
        {
            Debug.LogError("Error: streamPrefab or origin not assigned!");
            return;
        }

        // Create the stream and set the color
        currentStream = CreateStream();
        if (currentStream != null)
        {
            Color liquidColor = LiquidManager.Instance != null ? LiquidManager.Instance.GetSelectedLiquidColor() : Color.white;
            currentStream.SetStreamColor(liquidColor);
            currentStream.Begin();
        }
        else
        {
            Debug.LogError("Error: pouring stream was not created!");
        }
    }
    
    private void EndPour()
    {
        Debug.Log("Pouring ended!");
        if (currentStream != null)
        {
            currentStream.End();
            currentStream = null;
        }
    }

    private float CalculatePourAngle()
    {
        float angle = transform.forward.y * Mathf.Rad2Deg;
        Debug.Log($"Correct bottle angle: {angle}°");
        return angle;
    }

    private Stream CreateStream()
    {
        GameObject streamObject = Instantiate(streamPrefab, origin.position, Quaternion.identity, transform);
        return streamObject.GetComponent<Stream>();
    }
}