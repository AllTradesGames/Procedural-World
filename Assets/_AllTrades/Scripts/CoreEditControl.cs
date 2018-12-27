using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreEditControl : MonoBehaviour
{

    public bool continuousOffset = false;
    public Vector3 offsetFromPlayer = new Vector3(0f, -0.2f, 0.45f);
    public float rotateSpeed = 1f;
    public GameObject optionPrefab;
    public GameObject placementPrefab;
    public int coreLimit = 7;

    public AT_proto_Core coreData;

    private Transform mainCamera;

    void Awake()
    {
        mainCamera = Camera.main.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = mainCamera.position + offsetFromPlayer;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (continuousOffset)
        {
            transform.position = mainCamera.position + offsetFromPlayer;
        }
        transform.Rotate(new Vector3(Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical"), -Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal"), 0f) * rotateSpeed * Time.deltaTime, Space.World);
    }

    public void CreatePlacements()
    {
        Vector3 posLimits = Vector3.zero;
        Vector3 negLimits = Vector3.zero;

        Transform pTransform;
        foreach (AT_proto_V3 placement in coreData.placements)
        {
            pTransform = Instantiate(placementPrefab, Vector3.zero, Quaternion.identity, transform).transform;
            pTransform.localPosition = new Vector3(placement.x, placement.y, placement.z);

            if (placement.x > posLimits.x)
            {
                posLimits.x = placement.x;
            }
            else if (placement.x < negLimits.x)
            {
                negLimits.x = placement.x;
            }

            if (placement.y > posLimits.y)
            {
                posLimits.y = placement.y;
            }
            else if (placement.y < negLimits.y)
            {
                negLimits.y = placement.y;
            }

            if (placement.z > posLimits.z)
            {
                posLimits.z = placement.z;
            }
            else if (placement.z < negLimits.z)
            {
                negLimits.z = placement.z;
            }
        }

        if (posLimits.x < coreLimit)
        {
            pTransform = Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, transform).transform;
            pTransform.localPosition = new Vector3(posLimits.x + 1, 0, 0);
        }
        if (posLimits.y < coreLimit)
        {
            pTransform = Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, transform).transform;
            pTransform.localPosition = new Vector3(0, posLimits.y + 1, 0);
        }
        if (posLimits.z < coreLimit)
        {
            pTransform = Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, transform).transform;
            pTransform.localPosition = new Vector3(0, 0, posLimits.z + 1);
        }
        if (Mathf.Abs(negLimits.x) < coreLimit)
        {
            pTransform = Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, transform).transform;
            pTransform.localPosition = new Vector3(negLimits.x - 1, 0, 0);
        }
        if (Mathf.Abs(negLimits.y) < coreLimit)
        {
            pTransform = Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, transform).transform;
            pTransform.localPosition = new Vector3(0, negLimits.y - 1, 0);
        }
        if (Mathf.Abs(negLimits.z) < coreLimit)
        {
            pTransform = Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, transform).transform;
            pTransform.localPosition = new Vector3(0, 0, negLimits.z - 1);
        }


        // TODO: Modify these loops to cover all areas of the core cube. Right now, only covers +x+y+z and -x-y-z but not +x-y+z, +x+y-z, etc.
        bool skip;
        for (int ii = 0; ii <= posLimits.x; ii++)
        {
            skip = false;
            for (int jj = 0; jj <= posLimits.y; jj++)
            {
                for (int kk = 0; kk <= posLimits.z; kk++)
                {
                    if (coreData.pList.Find(place => { return place.x == (UInt16)ii && place.y == (UInt16)jj && place.z == (UInt16)kk; }) == null)
                    {
                        if (coreData.pList.Find(place => { return place.x == (UInt16)ii - 1 && place.y == (UInt16)jj && place.z == (UInt16)kk; }) != null)
                        {
                            pTransform = Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, transform).transform;
                            pTransform.localPosition = new Vector3(ii, jj, kk);
                        }
                        skip = true;
                        break;
                    }
                }
                if (skip)
                    break;
            }
            if (skip)
                continue;
        }
        
        for (int ii = 0; ii <= Mathf.Abs(negLimits.x); ii++)
        {
            skip = false;
            for (int jj = 0; jj <= Mathf.Abs(negLimits.y); jj++)
            {
                for (int kk = 0; kk <= Mathf.Abs(negLimits.z); kk++)
                {
                    if (coreData.pList.Find(place => { return place.x == -(UInt16)ii && place.y == -(UInt16)jj && place.z == -(UInt16)kk; }) == null)
                    {
                        if (coreData.pList.Find(place => { return place.x == -(UInt16)ii - 1 && place.y == -(UInt16)jj && place.z == -(UInt16)kk; }) != null)
                        {
                            pTransform = Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, transform).transform;
                            pTransform.localPosition = new Vector3(-ii, -jj, -kk);
                        }
                        skip = true;
                        break;
                    }
                }
                if (skip)
                    break;
            }
            if (skip)
                continue;
        }

    }


}
