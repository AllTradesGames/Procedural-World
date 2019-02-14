using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetControl : MonoBehaviour
{
    public TargetSpawnControl spawnScript;
    public float prepTime;
    public float activeTime;
    public int id;
    public float maxScale = 0.3f;

    private float markTime;
    private bool active = false;
    private float scale;

    // Start is called before the first frame update
    void Awake()
    {
        markTime = Time.time;
        GetComponent<Collider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
        {
            if ((Time.time - markTime) > prepTime)
            {
                OnActivate();
            }
            else
            {
                //Debug.Log(new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, ((Time.time - markTime) / prepTime))));
                scale = Mathf.Lerp(0f, maxScale, ((Time.time - markTime) / prepTime));
                transform.localScale = new Vector3(scale, scale, scale);
                GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, ((Time.time - markTime) / prepTime)));
            }
        }
        else if ((Time.time - markTime) > activeTime)
        {
            Miss();
        }
    }

    private void OnActivate()
    {
        active = true;
        markTime = Time.time;
        GetComponent<Collider>().enabled = true;
        GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f, 1f);
    }

    private void Slice()
    {
        spawnScript.Sliced(id);
        Destroy(this.gameObject);
    }

    private void Miss()
    {
        spawnScript.Missed(id);
        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon")
        {
            Slice();
        }
    }
}
