using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoSceneController : MonoBehaviour
{
    // This script does all the necessary hookups to run the prototype scene error-free

    ATVRPlayerController pcScript;
    ATVRPlayerData pdScript;
    Transform player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null)
        {
            pcScript = player.GetComponent<ATVRPlayerController>();
            pcScript.enabled = true;
            pdScript = player.GetComponent<ATVRPlayerData>();
            player.Find("Menu Canvas/Text").GetComponent<UnityEngine.UI.Text>().text = "";
        }

        Destroy(Camera.main.GetComponent<Skybox>());
    }


}
