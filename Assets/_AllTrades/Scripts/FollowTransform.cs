using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{

    public Transform target;
    public bool translate;
    public float lerpPosSpeed;
    public Vector3 offset;
    public bool rotate;
    public float lerpRotSpeed;

    void Awake()
    {
        if (target != null)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
            ATVRPlayerController playerController = target.root.GetComponent<ATVRPlayerController>();
            if (playerController != null)
            {
                playerController.onRotate += this.Rotate;
            }
        }
    }

    void LateUpdate()
    {
        if (translate)
            if (lerpPosSpeed < 0)
            {
                transform.position = target.position + offset;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, target.position + offset, lerpPosSpeed * Time.deltaTime);
            }
        if (rotate)
            if (lerpRotSpeed < 0)
            {
                transform.rotation = target.rotation;
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, lerpRotSpeed * Time.deltaTime);
            }
    }

    public void Rotate(float amount)
    {
        transform.Rotate(new Vector3(0f, amount, 0f), Space.World);
    }
}
