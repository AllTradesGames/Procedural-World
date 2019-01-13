using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class at_SelectPointer : MonoBehaviour
{

    private RaycastHit hit;
    private LineRenderer line;
    private at_GameControlClient gameController;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<at_GameControlClient>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            line.SetPosition(1, hit.point);
        }
        else 
        {
            line.SetPosition(1, transform.position + transform.TransformDirection(Vector3.forward) * 100f);
        }
        line.SetPosition(0, transform.position);
    }
}
