using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicTransform : MonoBehaviour
{

    public Transform target;
    public int bufferSize = 90;

    private struct v3
    {
        public float x;
        public float y;
        public float z;
    }
    private v3[] buffer;
    private v3 targetStartPosition;
    private v3 selfStartPosition;
    private int writeIndex = 0;
    private bool fullBuffer = false;

    // Start is called before the first frame update
    void Start()
    {
        buffer = new v3[bufferSize];
        selfStartPosition.x = transform.position.x;
        selfStartPosition.y = transform.position.y;
        selfStartPosition.z = transform.position.z;
        if (target != null)
        {
            targetStartPosition.x = target.position.x;
            targetStartPosition.y = target.position.y;
            targetStartPosition.z = target.position.z;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        WriteToBuffer();
        ReadFromBuffer();
    }

    void WriteToBuffer()
    {
        if (target != null)
        {
            buffer[writeIndex].x = target.position.x;
            buffer[writeIndex].y = target.position.y;
            buffer[writeIndex].z = target.position.z;
            writeIndex++;
            if (writeIndex >= buffer.Length)
            {
                writeIndex = 0;
                fullBuffer = true;
            }
        }
    }

    void ReadFromBuffer()
    {
        if (fullBuffer)
        {
            transform.position = new Vector3(
                selfStartPosition.x + buffer[writeIndex].x,
                selfStartPosition.y + buffer[writeIndex].y,
                selfStartPosition.z + buffer[writeIndex].z
                );
        }
    }
}
