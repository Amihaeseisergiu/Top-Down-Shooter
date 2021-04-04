using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    private Transform playerTransform;
    public GameObject healthBarPrefab;
    private GameObject healthBar;
    private Slider slider;
    public GameObject impactParticle;

    public float health;
    public float maxHealth;
    public float damage;
    public float speed;
    public float randChosen;

    public bool isDestroyed;

    // Start is called before the first frame update
    void Start()
    {
        randChosen = Random.Range(-0.5f, 1.0f);

        health = 100 + randChosen * 190;
        maxHealth = 100 + randChosen * 190;
        damage = 25 + randChosen * 45;
        speed = 2500f - randChosen * 2000;
        gameObject.transform.localScale += new Vector3(randChosen, randChosen, randChosen);

        if(randChosen > 0.8f)
        {
            var color = gameObject.GetComponent<Renderer>().material.GetColor("_Color");
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f));
        }

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
        FollowHealthBar();
    }

    void FollowHealthBar()
    {
        healthBar.transform.position = transform.position;
        Vector3 pos = transform.position;
        pos.z = pos.z + gameObject.transform.localScale.z;
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
            Instantiate(impactParticle, contact.point + gameObject.transform.localScale / 4, Quaternion.FromToRotation(Vector3.up, contact.normal));
        }
        else if(other.gameObject.name == "Enemy(Clone)" && !other.gameObject.GetComponent<EnemyScript>().isDestroyed && other.gameObject.GetComponent<EnemyScript>().randChosen > 0.8f)
        {
            Vector3 newSize = other.gameObject.transform.localScale + gameObject.transform.localScale;
            if (newSize.x < 5.0f && newSize.y < 5.0f && newSize.z < 5.0f)
            {
                other.gameObject.transform.localScale += gameObject.transform.localScale;
            }

            other.gameObject.GetComponent<EnemyScript>().maxHealth += maxHealth;
            other.gameObject.GetComponent<EnemyScript>().health += health;
            other.gameObject.GetComponent<EnemyScript>().damage += damage;
            other.gameObject.GetComponent<EnemyScript>().speed += speed / 2;

            var color = other.gameObject.GetComponent<Renderer>().material.GetColor("_Color");
            other.gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(color.r * 0.9f, color.g * 0.9f, color.b * 0.9f));

            isDestroyed = true;
            Destroy(gameObject);
            Destroy(healthBar);
        }
    }

    void CalculateHealth()
    {
        health = health - GameObject.Find("Player").GetComponent<PlayerScript>().damage;
        slider.value = health / maxHealth;
    }
}
