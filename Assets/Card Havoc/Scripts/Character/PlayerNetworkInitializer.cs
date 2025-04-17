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

    void Update()
    {
        if (!IsOwner || !IsSpawned || animationController == null || movementController == null)
            return;

        // Step 1: Read player movement state
        float moveX = movementController.InputMove.x;
        float moveZ = movementController.InputMove.y;
        bool isRunning = movementController.IsSprinting;
        bool isJumping = !movementController.Grounded && movementController.VerticalVelocity > 0.1f;
        bool isGrounded = movementController.Grounded;

        // Step 2: Apply locally
        animationController.UpdateLocalAnimations(moveX, moveZ, isRunning, isJumping, isGrounded);

        // Step 3: Sync to others
        SendAnimationToServerRpc(moveX, moveZ, isRunning, isJumping, isGrounded);
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void UpdateRemoteAnimations(float moveX, float moveZ, bool isRunning, bool isJumping, bool isGrounded)
    {
        // Step 4: Let remote players update their view of this player
        if (animationController != null) {
            animationController.UpdateRemoteAnimations(moveX, moveZ, isRunning, isJumping, isGrounded);
        }

    }

    [ServerRpc]
    private void SendAnimationToServerRpc(float x, float z, bool isRunning, bool isJumping, bool isGrounded)
    {
        UpdateRemoteAnimations(x, z, isRunning, isJumping, isGrounded);
    }
}