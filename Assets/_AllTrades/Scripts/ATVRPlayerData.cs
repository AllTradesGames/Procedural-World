using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATVRPlayerData : MonoBehaviour
{
    static bool isDuplicate;

    public AT_proto_User user;
    public AT_proto_Core core;
    public Oculus.Platform.Models.User OVRuser;

    void Awake()
    {
        if (isDuplicate)
        {
            DestroyImmediate(this.gameObject);
        }
        else
        {
            isDuplicate = true;
        }
    }


}
