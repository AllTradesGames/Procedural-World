using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawnControl : MonoBehaviour
{
    public GameObject targetPrefab;
    public float spawnDelay;
    public float[] xRange;
    public float[] yRange;
    public float zDistance;

    private int lastTargetId = 0;
    private float lastSpawnTime;
    private Transform target;
    private TargetControl targetScript;

    // Start is called before the first frame update
    private void Awake()
    {
        SpawnTarget();
    }

    // Update is called once per frame
    void Update()
    {
            if ((Time.time - lastSpawnTime) > spawnDelay)
            {
                SpawnTarget();
            }        
    }

    void SpawnTarget()
    {
        target = Instantiate(targetPrefab, Vector3.zero, Quaternion.identity, transform).transform;
        target.localPosition = new Vector3(Random.Range(xRange[0], xRange[1]), Random.Range(yRange[0], yRange[1]), zDistance);
        targetScript = target.GetComponent<TargetControl>();
        targetScript.id = ++lastTargetId;
        targetScript.spawnScript = this;
        lastSpawnTime = Time.time;
    }

    public void Sliced(int id)
    {
        Debug.Log("Target " + id + " was sliced!");
    }

    public void Missed(int id)
    {
        Debug.Log("Target " + id + " was missed!");
    }
}
