using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class PlayerNetworkInitializer : NetworkBehaviour
{
    [Header("Local Player Only")]
    [SerializeField] private GameObject mainCamera;         // Your camera with CinemachineBrain
    [SerializeField] private GameObject fpsGraphics;        // Arms, weapon, etc.
    
    [Header("Remote Players Only")]
    [SerializeField] private GameObject thirdPersonGraphics; // Full-body mesh for other players

    [Header("Scripts")]
    [SerializeField] private MonoBehaviour[] localOnlyScripts; // e.g., PlayerController, CameraController


    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            GameObject worldCam = GameObject.FindGameObjectWithTag("WorldCamera");

            // Local player setup
            if (mainCamera != null)
                mainCamera.SetActive(true);

            if (fpsGraphics != null)
                fpsGraphics.SetActive(true);

            if (thirdPersonGraphics != null)
                thirdPersonGraphics.SetActive(false);

            foreach (var script in localOnlyScripts)
            {
                Debug.Log("script active " + script);
                if (script != null)
                    script.enabled = true;
            }

            if (worldCam != null)
                worldCam.SetActive(false);
        }
        else
        {
            // Remote player setup
            if (mainCamera != null)
                mainCamera.SetActive(false);

            if (fpsGraphics != null)
                fpsGraphics.SetActive(false);

            if (thirdPersonGraphics != null)
                thirdPersonGraphics.SetActive(true);

            foreach (var script in localOnlyScripts)
            {
                if (script != null)
                    script.enabled = false;
            }
        }
    }
}