using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Platform;

public class DataPreloadControl : MonoBehaviour
{

    Oculus.Platform.Models.User user;
    UInt64 orgScopedID;

    void Awake()
    {
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
                user = msg.GetUser();
                Users.GetOrgScopedID(user.ID).OnComplete((Message ms) =>
                {
                    if (ms.IsError)
                    {
                        Debug.LogError("Error getting org scoped ID.");
                    }
                    else
                    {
                        orgScopedID = ms.GetOrgScopedID().ID;

                        Debug.Log("Logged in user: " + user.OculusID + ", App Relative ID: " + user.ID);
                        Debug.Log("Org Scoped ID: " + orgScopedID);

                        Debug.Log("Getting User Data...");
                        StartCoroutine(GetUserData(orgScopedID));
                    }
                });
            }
        });
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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

    IEnumerator GetUserData(UInt64 orgScopedID)
    {
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get("https://us-central1-prototype-backend.cloudfunctions.net/get-user-data?orgscopedid=" + orgScopedID);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            // TODO: Store retrieved data as User class
            // TODO: Check if screenName is "". If so, prompt user for screenName
        }
    }


}
