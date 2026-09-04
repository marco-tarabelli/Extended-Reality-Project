using UnityEngine;
public class ContainerManager : MonoBehaviour
{
    public static ContainerManager Instance { get; private set; }

    private GameObject selectedContainerPrefab; // Selected container prefab
    private GameObject spawnedContainer;       // Instantiated container
    private bool isContainerConfirmed = false; // Flag to check if the container is confirmed

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Make ContainerManager persistent
    }

    // Method to select a container prefab
    public void SelectContainer(GameObject containerPrefab)
    {
        if (!isContainerConfirmed) // Allows selection only if not confirmed
        {
            selectedContainerPrefab = containerPrefab;
            Debug.Log($"Selected container: {selectedContainerPrefab.name}");
        }
        else
        {
            Debug.LogWarning("The container has already been confirmed and cannot be changed!");
        }
    }

    // Method to instantiate the selected container
    public void SpawnSelectedContainer(Vector3 position, Quaternion rotation)
    {
        if (selectedContainerPrefab != null && !isContainerConfirmed)
        {
            if (spawnedContainer != null)
            {
                Destroy(spawnedContainer); // Removes the previous container if it exists
            }

            spawnedContainer = Instantiate(selectedContainerPrefab, position, rotation);
            Debug.Log($"Container instantiated: {spawnedContainer.name}");
            Debug.Log($"Container instantiated - Position Y: {spawnedContainer.transform.position.y}");
            LiquidManager.Instance.SetCurrentContainer(spawnedContainer);
        }
        else if (isContainerConfirmed)
        {
            Debug.LogWarning("The container has already been confirmed and cannot be repositioned!");
        }
        else
        {
            Debug.LogWarning("No container selected for instantiation!");
        }
    }

    // Method to confirm the container
    public void ConfirmContainer()
    {
        if (spawnedContainer != null)
        {
            isContainerConfirmed = true;
            DontDestroyOnLoad(spawnedContainer); // Make the container persistent
            Debug.Log("Container confirmed and made persistent!");
        }
        else
        {
            Debug.LogWarning("No container to confirm!");
        }
    }

    // Method to get the instantiated container
    public GameObject GetSpawnedContainer()
    {
        return spawnedContainer;
    }

    // Method to check if the container is confirmed
    public bool IsContainerConfirmed()
    {
        return isContainerConfirmed;
    }
    
    // Method to reset the container state
    public void ResetContainerState()
    {
        isContainerConfirmed = false;
        selectedContainerPrefab = null;
        spawnedContainer = null;
        Debug.Log("ContainerManager reset: the container can be selected again.");
    }
}
