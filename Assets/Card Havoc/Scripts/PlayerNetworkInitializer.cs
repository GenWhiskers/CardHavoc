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
            //TODO: Having trouble not switching camreas... Maybe flip logic idk.
            // I think it has to do with the Cinemachine brain and what not look into that more
            //      Find a way to diable everything but the FPSgraphics of the other player

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

            //gameObject.GetComponent<PlayerController>().enabled = false;



            // Remote player setup
            if (virtualCamera != null)
                virtualCamera.SetActive(false);
                Debug.Log("SET cirtual FALSE");

            
            if (mainCamera != null)
                mainCamera.SetActive(false);
                Debug.Log("SET main camrea FALSE");

            if (fpsGraphics != null)
                fpsGraphics.SetActive(false);
                Debug.Log("SET fps FALSE");

            if (thirdPersonGraphics != null)
                thirdPersonGraphics.SetActive(true);
                Debug.Log("SET Thirdperson true");

            foreach (var script in localOnlyScripts)
            {
                Debug.Log("Deactive script: " + script);
                if (script != null)
                    script.enabled = false;
            }
        }
    }
}