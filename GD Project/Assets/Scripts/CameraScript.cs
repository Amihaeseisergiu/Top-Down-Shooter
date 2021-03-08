using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //horizontalInput = Input.GetAxis("Horizontal");
        //verticalInput = Input.GetAxis("Vertical");
        //transform.position = transform.position + new Vector3(horizontalInput * movementSpeed * Time.deltaTime, 0f, verticalInput * movementSpeed * Time.deltaTime);
        transform.position = playerTransform.position;
        Vector3 pos = transform.position;
        pos.y = 15;
        transform.position = pos;
    }
}
