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

    public bool isDestroyed;
    public bool hitting;

    public string type;

    // Start is called before the first frame update
    void Start()
    {
        float randType = Random.Range(0.0f, 2.0f);

        if(randType < 1.0f)
        {
            type = "walker";
            
            float randChosen = Random.Range(-0.5f, 0.8f);
            health = 100 + randChosen * 190;
            maxHealth = 100 + randChosen * 190;
            damage = 25 + randChosen * 45;
            speed = 2600f - 1000 * randChosen;
            gameObject.transform.localScale += new Vector3(randChosen, randChosen, randChosen);
        }
        else if(randType >= 1.0f && randType < 1.8f)
        {
            type = "shooter";

            health = 100;
            maxHealth = 100;
            damage = 0;
            speed = 2500f;
            gameObject.transform.localScale += new Vector3(0f, 0f, 0f);

            gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f));
        }
        else
        {
            type = "eater";

            health = 300;
            maxHealth = 300;
            damage = 80;
            speed = 2000f;
            gameObject.transform.localScale += new Vector3(1f, 1f, 1f);

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
        slider.value = health / maxHealth;
        healthBar.transform.position = transform.position;
        Vector3 pos = transform.position;
        pos.z = pos.z + gameObject.transform.localScale.z;
        healthBar.transform.position = pos;
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.name == "Player")
        {
            startHitting();
        }
        else if (other.gameObject.name.Contains("Bullet"))
        {
            CalculateHealth();
            if (health <= 0)
            {
                GameObject.Find("Player").GetComponent<PlayerScript>().enemiesKilled++;
                Destroy(healthBar);
                Destroy(gameObject);
            }

            Vector3 moveDirection = gameObject.GetComponent<Rigidbody>().transform.position - playerTransform.position;
            gameObject.GetComponent<Rigidbody>().AddForce(moveDirection.normalized * 200f);

            ContactPoint contact = other.contacts[0];
            Instantiate(impactParticle, contact.point + (gameObject.transform.position - contact.point).normalized * (gameObject.transform.localScale.x + 0.5f), Quaternion.FromToRotation(Vector3.up, contact.normal * -1));
        }
        else if(other.gameObject.name == "Enemy(Clone)" && !other.gameObject.GetComponent<EnemyScript>().isDestroyed && other.gameObject.GetComponent<EnemyScript>().type == "eater" && type == "eater")
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

    void OnCollisionExit(Collision other)
    {
        hitting = false;
    }

    void CalculateHealth()
    {
        health = health - GameObject.Find("Player").GetComponent<PlayerScript>().amplifiedDamage;
        slider.value = health / maxHealth;
    }

    IEnumerator hitPlayer()
    {
        hitting = true;
        while (hitting)
        {
            GameObject.Find("Player").GetComponent<PlayerScript>().takeHit(damage);
            yield return new WaitForSeconds(0.7f);
        }
    }

    void startHitting()
    {
        StartCoroutine(hitPlayer());
    }
}
