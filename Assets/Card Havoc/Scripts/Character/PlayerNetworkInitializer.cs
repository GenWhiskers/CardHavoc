using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Example.Scened;
using System.Collections.Generic;
using Unity.Cinemachine;

public class PlayerNetworkInitializer : NetworkBehaviour
{
    [Header("Local Player Only")]
    [SerializeField] private GameObject mainCamera;         // Your camera with CinemachineBrain
    [SerializeField] private GameObject virtualCamera;         // Your camera with CinemachineBrain

    [Header("Scripts")]
    [SerializeField] private MonoBehaviour[] localOnlyScripts; // e.g., PlayerController, CameraController


    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            GameObject worldCam = GameObject.FindGameObjectWithTag("WorldCamera");
            if (worldCam != null)
                worldCam.SetActive(false);

            // Local player setup
            // if (thirdPersonGraphics != null)
            //     thirdPersonGraphics.SetActive(false);
            //     Debug.Log("SET FALSE");

        }
        else
        {

            // Remote player setup
            if (virtualCamera != null)
                virtualCamera.SetActive(false);
                Debug.Log("SET cirtual FALSE");

            if (mainCamera != null)
                mainCamera.SetActive(false);
                Debug.Log("SET main camrea FALSE");

            foreach (var script in localOnlyScripts)
            {
                Debug.Log("Deactive script: " + script);
                if (script != null)
                    script.enabled = false;
            }
        }
    }
}