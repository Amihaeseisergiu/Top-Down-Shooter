using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private float horizontalInput = 0f;
    private float verticalInput = 0f;
    private GameObject camera;
    public GameObject bullet;
    private GameObject firePoint;
    private float thrust = 200.0f;
    public GameObject healthBarPrefab;
    private GameObject healthBar;
    private Slider slider;
    private float nextHit;
    private bool collided;
    public Text ammoDisplay;
    bool isReloading = false;

    private float movementSpeed;
    private float health;
    private float maxHealth;
    public float damage;
    private float enemyDamage;
    public int ammo;
    public int restAmmo;

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        maxHealth = 100;
        movementSpeed = 10f;
        damage = 25;
        ammo = 30;
        restAmmo = 120;

        // Get bullet child
        firePoint = gameObject.transform.GetChild(0).gameObject;
        nextHit = Time.time;
        healthBar = Instantiate(healthBarPrefab);
        slider = (Slider)GameObject.FindObjectsOfType(typeof(Slider))[0];
        FollowHealthBar();
        ammoDisplay.text = ammo.ToString() + "/" + restAmmo.ToString();
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
            if (playerPlane.Raycast(ray, out enter) && ammo > 0 && !isReloading)
            {
                GameObject bulletCopy = Instantiate(bullet, firePoint.transform.position, Quaternion.identity);
                var hitPoint = ray.GetPoint(enter);
                var mouseDir = hitPoint - gameObject.transform.position;
                mouseDir = mouseDir.normalized;
                bulletCopy.GetComponent<Rigidbody>().AddForce(mouseDir * thrust);
                ammo--;
                ammoDisplay.text = ammo.ToString() + "/" + restAmmo.ToString();
                if (ammo == 0 && restAmmo > 0)
                {
                    isReloading = true;
                    Invoke("ResetReload", 2);
                    ammo += 30;
                    restAmmo -= 30;
                }
            }
        }

        //Continuous enemy hits
        if (collided && nextHit <= Time.time)
        {
            CalculateHealth();
            nextHit = Time.time + 1.0f;
            if (health <= 0)
            {
                Destroy(healthBar);
                Destroy(gameObject);
            }
        }
    }

    void ResetReload()
    {
        isReloading = false;
        ammoDisplay.text = ammo.ToString() + "/" + restAmmo.ToString();
    }

    void FixedUpdate()
    {
        transform.position = transform.position + new Vector3(horizontalInput * movementSpeed * Time.deltaTime, 0f, verticalInput * movementSpeed * Time.deltaTime);
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
        if (other.gameObject.name == "Enemy(Clone)")
        {
            enemyDamage = other.gameObject.GetComponent<EnemyScript>().damage;
            collided = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.name == "Enemy(Clone)")
        {
            collided = false;
        }
    }

    void CalculateHealth()
    {
        health = health - enemyDamage;
        slider.value = health / maxHealth;
    }
}
