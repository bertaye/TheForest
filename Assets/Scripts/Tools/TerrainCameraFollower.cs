using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCameraFollower : MonoBehaviour
{
    Camera mainCam;
    Vector3 Offset;
    Vector3 upToDatePos;
    float initialY;
    Quaternion rotationOffset;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        Offset = mainCam.transform.position - transform.position;
        rotationOffset = gameObject.transform.rotation;
        initialY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        upToDatePos = mainCam.transform.position - Offset;
        upToDatePos.y = initialY;
        transform.position = upToDatePos;
        //transform.rotation = rotationOffset * new Quaternion(0, mainCam.transform.rotation.y, 0, mainCam.transform.rotation.w);
    }
}
