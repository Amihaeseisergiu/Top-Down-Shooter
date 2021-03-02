using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Variables
    public float movementSpeed;
    public GameObject camera;

    //Methods
    void Update()
    {
        //Player facing the mouse

        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

        float hitDist = 0.0f;

        if(playerPlane.Raycast(ray, out hitDist))
        {
            Vector3 targetPoint = ray.GetPoint(hitDist);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7f * Time.deltaTime);
        }


        //Player movement
        if(Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.forward * movementSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * movementSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * movementSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.back * movementSpeed;
        }
    }
}
