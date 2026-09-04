using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;


public class Raycast_script : MonoBehaviour
{
    public List<GameObject> beakerPrefabs; // Lista dei prefab disponibili
    private ARRaycastManager arrayman;     // Raycast manager
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        arrayman = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        // Posizionamento con il raycast
        if (Input.touchCount > 0 && !ContainerManager.Instance.IsContainerConfirmed())
        {
            if (arrayman.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                ContainerManager.Instance.SpawnSelectedContainer(hitPose.position, Quaternion.Euler(270f, 0f, 0f));
            }
        }
    }

    // Metodo per selezionare un prefab
    public void SelectPrefab(int prefabIndex)
    {
        if (!ContainerManager.Instance.IsContainerConfirmed())
        {
            if (prefabIndex >= 0 && prefabIndex < beakerPrefabs.Count)
            {
                ContainerManager.Instance.SelectContainer(beakerPrefabs[prefabIndex]);
            }
            else
            {
                Debug.LogWarning("Indice prefab non valido!");
            }
        }
        else
        {
            Debug.LogWarning("Il contenitore è già confermato, non può essere cambiato!");
        }
    }
}
