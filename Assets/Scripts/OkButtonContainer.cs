using UnityEngine;



public class ConfirmButton : MonoBehaviour
{
    public void OnOkButtonClicked()
    {
        if (ContainerManager.Instance != null)
        {
            ContainerManager.Instance.ConfirmContainer();
            

        }
        else
        {
            Debug.LogWarning("ContainerManager didnt find!");
        }
    }
}
