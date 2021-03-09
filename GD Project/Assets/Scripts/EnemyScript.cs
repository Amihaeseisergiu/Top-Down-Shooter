using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private float movementSpeed = 0.15f;
    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, movementSpeed);
    }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log(col.gameObject.name);
        if (col.gameObject.name == "Bullet(Clone)")
        {
            Destroy(gameObject);
        } else if (col.gameObject.name == "Player")
        {
            Destroy(col.gameObject);
        }
    }
}
