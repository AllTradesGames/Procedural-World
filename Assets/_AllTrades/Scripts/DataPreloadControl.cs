using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Platform;

public class DataPreloadControl : MonoBehaviour
{
    public AT_proto_User user;

    Oculus.Platform.Models.User OVRuser;

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
                OVRuser = msg.GetUser();
                Users.GetOrgScopedID(OVRuser.ID).OnComplete((Message ms) =>
                {
                    if (ms.IsError)
                    {
                        Debug.LogError("Error getting org scoped ID.");
                    }
                    else
                    {
                        user = new AT_proto_User(ms.GetOrgScopedID().ID, OVRuser.OculusID);

                        Debug.Log("Logged in Oculus user: " + OVRuser.OculusID + ", App Relative ID: " + OVRuser.ID);
                        Debug.Log("Org Scoped ID: " + user.orgScopedID);

                        Debug.Log("Getting User Data...");
                        StartCoroutine(GetUserData(user));
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
            JsonUtility.FromJsonOverwrite(www.downloadHandler.text, user);

            if(www.responseCode == 201)
            {
                // New User was created
            }
        }
    }


}
