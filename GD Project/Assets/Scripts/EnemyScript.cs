using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    private float movementSpeed = 0.15f;
    private Transform playerTransform;
    public GameObject healthBarPrefab;
    private GameObject healthBar;
    private float health;
    private float maxHealth;
    private Slider slider;
    public GameObject impactParticle;

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        maxHealth = 100;
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        impactParticle = GameObject.Find("Blood");
        healthBar = Instantiate(healthBarPrefab);
        slider = (Slider)GameObject.FindObjectsOfType(typeof(Slider))[0];
        FollowHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (playerTransform != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, movementSpeed);
        }
        FollowHealthBar();
    }

    void FollowHealthBar()
    {
        healthBar.transform.position = transform.position;
        Vector3 pos = transform.position;
        pos.z = pos.z + 1.5f;
        healthBar.transform.position = pos;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Bullet(Clone)")
        {
            CalculateHealth();
            if (health <= 0)
            {
                Destroy(healthBar);
                Destroy(gameObject);
            }

            Vector3 moveDirection = gameObject.GetComponent<Rigidbody>().transform.position - playerTransform.position;
            gameObject.GetComponent<Rigidbody>().AddForce(moveDirection.normalized * 200f);

            ContactPoint contact = other.contacts[0];
            Instantiate(impactParticle, contact.point, Quaternion.FromToRotation(Vector3.up, contact.normal));
        }
    }

    void CalculateHealth()
    {
        health = health - 25;
        slider.value = health / maxHealth;
    }
}
