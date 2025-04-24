using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Example.Scened;
using System.Collections.Generic;
using Unity.Cinemachine;
using PlayerAssets;

public class PlayerNetworkInitializer : NetworkBehaviour
{
    [Header("Local Player Only")]
    [SerializeField] private GameObject mainCamera;         // Your camera with CinemachineBrain
    [SerializeField] private GameObject virtualCamera;         // Your camera with CinemachineBrain

    [Header("Scripts")]
    [SerializeField] private MonoBehaviour[] localOnlyScripts; // e.g., PlayerController, CameraController
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private ThirdPersonController movementController;


    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            GameObject worldCam = GameObject.FindGameObjectWithTag("WorldCamera");
            if (worldCam != null)
                worldCam.SetActive(false);
            
            UIManager.Instance?.SpawnHUD();

        }
        else
        {

            // Remote player setup
            if (virtualCamera != null)
                virtualCamera.SetActive(false);

            if (mainCamera != null)
                mainCamera.SetActive(false);

            foreach (var script in localOnlyScripts)
            {
                if (script != null)
                    script.enabled = false;
            }
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (IsOwner)
        {
            UIManager.Instance?.DestroyHUD();
        }
    }
}