using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Oculus.Platform;

public class DataPreloadControl : MonoBehaviour
{
    ATVRPlayerData playerData;
    bool userDone = false;
    bool coreDone = false;
    bool sceneDone = false;

    void Awake()
    {
        OVRManager.tiledMultiResLevel = OVRManager.TiledMultiResLevel.LMSHigh;
        /*GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerData = player.GetComponent<ATVRPlayerData>();

        try
        {
            Core.AsyncInitialize();
            Entitlements.IsUserEntitledToApplication().OnComplete(CheckEntitlement);
        }
        catch (UnityException err)
        {
            Debug.LogError("Platform failed to initialize due to exception.");
            Debug.LogException(err);
            CheckEntitlement(null);
        }

        Users.GetLoggedInUser().OnComplete((Message msg) =>
        {
            if (msg.IsError)
            {
                Debug.LogError("Error getting logged in user.");
                UnityEngine.Application.Quit();
            }
            else
            {
                playerData.OVRuser = msg.GetUser();
                Users.GetOrgScopedID(playerData.OVRuser.ID).OnComplete((Message ms) =>
                {
                    if (ms.IsError)
                    {
                        Debug.LogError("Error getting org scoped ID.");
                    }
                    else
                    {
                        playerData.user = new AT_proto_User(ms.GetOrgScopedID().ID, playerData.OVRuser.OculusID);

                        Debug.Log("Logged in Oculus user: " + playerData.OVRuser.OculusID + ", App Relative ID: " + playerData.OVRuser.ID);
                        Debug.Log("Org Scoped ID: " + playerData.user.orgScopedID);

                        Debug.Log("Getting User Data...");
                        StartCoroutine(GetUserData(playerData.user));

                        Debug.Log("Getting Core Data...");
                        StartCoroutine(GetCoreData(playerData.user));

                        Debug.Log("Loading next scene...");
                        StartCoroutine(LoadSceneAsync());
                    }
                });
            }
        });*/
    }

    void Start()
    {
        // Just load next scene
        Debug.Log("Loading next scene...");
        StartCoroutine(LoadSceneAsync());
    }

    void CheckEntitlement(Message msg)
    {
        if (msg == null || msg.IsError)
        {
            // Entitlement check failed. Quit app
            Debug.LogError("Oculus User Entitlement check failed.");
            UnityEngine.Application.Quit();
        }
        else
        {
            // Entitlement check passed
            Debug.Log("Oculus User Entitlement check passed.");
        }
    }

    IEnumerator GetUserData(AT_proto_User in_user)
    {
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get("https://us-central1-prototype-backend.cloudfunctions.net/get-user-data?orgscopedid=" + in_user.orgScopedID + "&screenname=" + in_user.screenName);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Store results as User class
            JsonUtility.FromJsonOverwrite(www.downloadHandler.text, playerData.user);

            if (www.responseCode == 201)
            {
                // New User was created
            }

            userDone = true;
            CheckIfAllDone();
        }
    }

    IEnumerator GetCoreData(AT_proto_User in_user)
    {
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get("https://us-central1-prototype-backend.cloudfunctions.net/get-core-data?orgscopedid=" + in_user.orgScopedID);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Store results as Core class
            JsonUtility.FromJsonOverwrite(www.downloadHandler.text, playerData.core);
            playerData.core.pList = new List<AT_proto_V3>(playerData.core.placements);
            Debug.Log("points: " + playerData.core.availablePoints + " placements[0]: " + playerData.core.placements[0].x + ", " + playerData.core.placements[0].y + ", " + playerData.core.placements[0].z);

            if (www.responseCode == 201)
            {
                // New Core was created
            }

            coreDone = true;
            CheckIfAllDone();
        }
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        sceneDone = true;
        CheckIfAllDone();
    }

    void CheckIfAllDone()
    {
        if (userDone && coreDone && sceneDone)
        {
            Debug.Log("All Loading Finished. Destroying DataPreloadControl.");
            DestroyImmediate(this.gameObject);
        }
    }


}
