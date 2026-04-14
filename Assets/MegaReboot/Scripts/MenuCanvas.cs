using UnityEngine;

public class MenuCanvas : MonoBehaviour
{
    private Transform cameraT;

    private void Start()
    {
        cameraT = Camera.main.transform;    
    }
    private void Update()
    {
        if (cameraT == null)
            return;

        transform.position = cameraT.position + (cameraT.forward.normalized) * 1f;
        
        transform.rotation = Quaternion.LookRotation(cameraT.forward);
    }
}
