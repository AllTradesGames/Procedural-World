using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{

    public Transform target;
    public float lerpPosSpeed;
    public float lerpRotSpeed;

    void Awake()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, lerpPosSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, lerpRotSpeed * Time.deltaTime);
    }
}
