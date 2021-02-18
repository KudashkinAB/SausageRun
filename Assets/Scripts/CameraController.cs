using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject follow;
    [SerializeField] Vector3 offset;
    [SerializeField] bool setOffsetOnStart = true;

    private void Start()
    {
        if (setOffsetOnStart)
        {
            offset = transform.position - follow.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(follow != null)
        {
            transform.position = follow.transform.position + offset;
        }
    }
}
