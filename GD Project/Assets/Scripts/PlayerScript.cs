using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private float horizontalInput = 0f;
    private float verticalInput = 0f;
    private float movementSpeed = 10f;
    private GameObject camera;
    public GameObject bullet;
    private GameObject firePoint;
    public int clickForce = 100;

    // Start is called before the first frame update
    void Start()
    {
        // Get bullet child
        firePoint = gameObject.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //Player facing the mouse
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

        float hitDist = 0.0f;

        if (playerPlane.Raycast(ray, out hitDist))
        {
            Vector3 targetPoint = ray.GetPoint(hitDist);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7f * Time.deltaTime);
        }

        //movement
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        //shooting
        if (Input.GetMouseButtonDown(0))
        {
            float enter;
            if (playerPlane.Raycast(ray, out enter))
            {
                GameObject bulletCopy = Instantiate(bullet, firePoint.transform.position, Quaternion.identity);
                var hitPoint = ray.GetPoint(enter);
                var mouseDir = hitPoint - gameObject.transform.position;
                mouseDir = mouseDir.normalized;
                bulletCopy.GetComponent<Rigidbody>().AddForce(mouseDir * clickForce);
            }
        }
    }

    void FixedUpdate()
    {
        transform.position = transform.position + new Vector3(horizontalInput * movementSpeed * Time.deltaTime, 0f, verticalInput * movementSpeed * Time.deltaTime);
    }
}
