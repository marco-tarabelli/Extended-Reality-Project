using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidManager : MonoBehaviour
{
    public static LiquidManager Instance { get; private set; }
    private GameObject previewLiquid = null; // Temporary liquid (preview)
    private GameObject confirmedLiquid = null; // Final liquid
    public GameObject bottlePrefab; // Bottle prefab
    private GameObject currentBottle; // Instantiated bottle
    private float totalLiquidHeight = 0f;
    public GameObject mergedLiquidPrefab; // Merged liquid prefab
    private List<GameObject> confirmedLiquids = new List<GameObject>();
    private List<LiquidData> confirmedLiquidData = new List<LiquidData>();
    private Color selectedLiquidColor; // Color of selected liquid

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject); // Keep the object across scenes
        Debug.Log("LiquidManager it's reloaded!");
    }

    public void SelectLiquid(GameObject liquidPrefab)
    {
        Debug.Log("SelectLiquid() called with " + liquidPrefab.name);

        GameObject container = ContainerManager.Instance.GetSpawnedContainer();
        if (container == null)
        {
            Debug.LogError("ERROR: No container found when selecting liquid!");
            return;
        }

        Debug.Log($"Liquid will be placed in: {container.name}");

        Renderer liquidRenderer = liquidPrefab.GetComponent<Renderer>();
        if (liquidRenderer != null)
        {
            selectedLiquidColor = liquidRenderer.material.color;
        }
        else
        {
            Debug.LogWarning("No Renderer found on the selected liquid prefab!");
        }

        // Calculate height based on confermated liquids
        float confirmedHeight = 0f;
        foreach (GameObject liquid in confirmedLiquids)
        {
            confirmedHeight += liquid.transform.localScale.z;
        }
        confirmedHeight = confirmedHeight * 2;
        Debug.Log("Confirmed Height: " + confirmedHeight);

        // Delete the old preview
        if (previewLiquid != null)
        {
            Destroy(previewLiquid);
        }

        // Create the preview
        previewLiquid = Instantiate(liquidPrefab, container.transform);
        previewLiquid.transform.localPosition = new Vector3(0, 0, confirmedHeight);

        Debug.Log($"New preview liquid positioned at {previewLiquid.transform.localPosition}");

        // Match the preview with slider
        SliderExample sliderExample = FindObjectOfType<SliderExample>();
        if (sliderExample != null)
        {
            sliderExample.SetLiquid(previewLiquid);
        }
    }

    public Color GetSelectedLiquidColor()
    {
        return selectedLiquidColor;
    }
    public void ConfirmLiquid()
    {
        if (previewLiquid == null)
        {
            Debug.LogWarning("No liquid to confirm!");
            return;
        }

        GameObject container = ContainerManager.Instance.GetSpawnedContainer();
        if (container == null)
        {
            Debug.LogWarning("No container found!");
            return;
        }

        // Create the liquid
        GameObject newLiquid = Instantiate(previewLiquid, container.transform);
        newLiquid.transform.localPosition = previewLiquid.transform.localPosition;
        newLiquid.transform.localScale = previewLiquid.transform.localScale;
        newLiquid.GetComponent<Renderer>().material.color = previewLiquid.GetComponent<Renderer>().material.color;

        // Add the liquids to the confirmeted ones
        confirmedLiquids.Add(newLiquid);

        // Save data
        LiquidData liquidData = new LiquidData
        {
            name = newLiquid.name.Replace("(Clone)", "").Trim(),
            amount = FindObjectOfType<SliderExample>().slider.value
        };
        confirmedLiquidData.Add(liquidData);

        // Update the total height
        totalLiquidHeight += newLiquid.transform.localScale.z;

        Debug.Log($"New total height: {totalLiquidHeight}");

        // Delete previww
        Destroy(previewLiquid);
        previewLiquid = null;

        // Slider disabled until next liquid
        SliderExample sliderExample = FindObjectOfType<SliderExample>();
        if (sliderExample != null)
        {
            sliderExample.DisableSlider();
        }
        SpawnBottle();
        // If more than one liquid merge all
        if (confirmedLiquids.Count > 1)
        {
            MergeLiquids();
        }
    }


    public void MergeLiquids()
    {
        if (confirmedLiquids.Count < 2)
        {
            Debug.LogWarning("You need at least two liquids to merge!");
            return;
        }

        GameObject container = ContainerManager.Instance.GetSpawnedContainer();
        if (container == null)
        {
            Debug.LogWarning("No container found!");
            return;
        }

        Color finalColor = Color.black;
        float totalAmount = 0f;

        foreach (GameObject liquid in confirmedLiquids)
        {
            Material mat = liquid.GetComponent<Renderer>().material;
            Color liquidColor = mat.color;
            float amount = liquid.transform.localScale.z;

            finalColor += liquidColor * amount;
            totalAmount += amount;
        }

        finalColor /= totalAmount;

        // Get the Y position of the first liquid
        float firstLiquidZ = confirmedLiquids[0].transform.localPosition.z;

        GameObject mergedLiquid = Instantiate(mergedLiquidPrefab, container.transform);
        mergedLiquid.transform.localPosition = new Vector3(0, 0, firstLiquidZ);

        // Set the size of the merged liquid based on total liquid height
        mergedLiquid.transform.localScale = new Vector3(
            confirmedLiquids[0].transform.localScale.x,
            confirmedLiquids[0].transform.localScale.y,
            totalLiquidHeight
        );

        mergedLiquid.GetComponent<Renderer>().material.color = finalColor;

        foreach (GameObject liquid in confirmedLiquids)
        {
            Destroy(liquid);
        }

        confirmedLiquids.Clear();
        confirmedLiquids.Add(mergedLiquid);

        Debug.Log("Liquids successfully merged!");
    }

    private void SpawnBottle()
    {
        if (bottlePrefab == null)
        {
            Debug.LogWarning("No bottle prefab assigned!");
            return;
        }

        GameObject container = ContainerManager.Instance.GetSpawnedContainer();
        if (container == null)
        {
            Debug.LogWarning("No container found to place the bottle!");
            return;
        }

        if (currentBottle != null)
        {
            Destroy(currentBottle);
        }
        
        
        // Ccalculate a slightly higher position and moved to the side with respect to the container
        Vector3 bottleSpawnPosition =
            container.transform.position + new Vector3(0.4f, 0.8f, 0);

        Vector3 directionToContainer = (container.transform.position - bottleSpawnPosition).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToContainer);
        // Create the bottle with the calculated and tilted rotation
        Quaternion bottleRotation = lookRotation * Quaternion.Euler(-40, 0, 0);

        currentBottle = Instantiate(bottlePrefab, bottleSpawnPosition, bottleRotation);

        if (currentBottle != null)
        {
            Debug.Log(
                $"Bottle successfully instantiated at {bottleSpawnPosition} with rotation {bottleRotation.eulerAngles}!");
            Destroy(currentBottle, 2f);
        }
        else
        {
            Debug.LogError("Error instantiating the bottle!");
        }
    }

    public void EndExperiment()
    {
        Debug.Log("End experiment button pressed");

        if (confirmedLiquidData.Count == 0)
        {
            Debug.LogWarning("No liquid to save!");
            return;
        }

        FindObjectOfType<ExperimentManager>().SaveExperiment(confirmedLiquidData);
        Debug.Log("Experiment successfully recorded!");
        //Clean out the container and the liquids
        GameObject container = ContainerManager.Instance.GetSpawnedContainer();
        if (container != null)
        {
            foreach (Transform child in container.transform)
            {
                Destroy(child.gameObject);
            }
            Destroy(container);
        }

        ResetLiquidManager();
        ContainerManager.Instance.ResetContainerState();
        StartCoroutine(ChangeSceneAfterDelay("222", 0.5f));
    }
    public void SetCurrentContainer(GameObject newContainer)
    {
        if (newContainer == null)
        {
            Debug.LogError("Trying to set a null container in LiquidManager!");
            return;
        }

        Debug.Log($"LiquidManager is now using the new container: {newContainer.name}");
    }

    // Method to reset liquids
    private void ResetLiquidManager()
    {
        confirmedLiquids.Clear();
        confirmedLiquidData.Clear();
        totalLiquidHeight = 0f;
        previewLiquid = null;
        Debug.Log("Liquid Manager resetted.");
    }
    private IEnumerator ChangeSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        ResetLiquidManager();
        
    }
}
