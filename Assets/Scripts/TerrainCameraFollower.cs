using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCameraFollower : MonoBehaviour
{
    Camera mainCam;
    Vector3 Offset;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        Offset = mainCam.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        transform.position = mainCam.transform.position - Offset;
    }
}
