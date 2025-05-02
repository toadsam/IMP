using UnityEngine;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera subCamera;
    public float subCameraDuration = 3f;

    public void SwitchToSubCamera()
    {
        StartCoroutine(SwitchCameraCoroutine());
    }

    private IEnumerator SwitchCameraCoroutine()
    {
       // mainCamera.gameObject.SetActive(false);
        subCamera.gameObject.SetActive(true);

        yield return new WaitForSeconds(subCameraDuration);

        subCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }
}
