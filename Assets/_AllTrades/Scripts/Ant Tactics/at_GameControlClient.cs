using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class at_GameControlClient : MonoBehaviour
{
    public int team;

    private at_ECSBootstrap bootstrapper;

    void Awake()
    {
        bootstrapper = transform.Find("AT ECS Bootstrapper").GetComponent<at_ECSBootstrap>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
