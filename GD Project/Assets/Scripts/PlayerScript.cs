using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    private float horizontalInput = 0f;
    private float verticalInput = 0f;
    private GameObject camera;
    public GameObject bulletPistol;
    public GameObject bulletAuto;
    public GameObject bulletShotgun;
    private GameObject firePoint;
    private GameObject firePoint2;
    private GameObject firePoint3;
    private float thrust = 200.0f;
    public GameObject healthBarPrefab;
    private GameObject healthBar;
    private Slider slider;
    public Text ammoDisplay;
    bool isReloading = false;
    bool isFiring = false;
    public Slider heartSlider;
    public Slider damageSlider;
    public Text pointsDisplay;
    public int enemiesKilled;
    private int points;

    private float movementSpeed;
    private float health;
    private float maxHealth;
    public float damage;
    public float amplifiedDamage;
    public int ammo;
    public int restAmmo;

    GameObject square;
    int weaponSelected = 1;

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        maxHealth = 100;
        movementSpeed = 10f;
        damage = 25;
        amplifiedDamage = 25;
        ammo = 30;
        restAmmo = 120;

        enemiesKilled = 0;
        square = GameObject.Find("SquareSprite");
        // Get bullet child
        firePoint = gameObject.transform.GetChild(0).gameObject;
        firePoint2 = gameObject.transform.GetChild(2).gameObject;
        firePoint3 = gameObject.transform.GetChild(1).gameObject;
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
            if (playerPlane.Raycast(ray, out enter) && ammo > 0 && !isReloading && !isFiring)
            {
                var hitPoint = ray.GetPoint(enter);
                var mouseDir = hitPoint - gameObject.transform.position;
                mouseDir = mouseDir.normalized;
                if (weaponSelected == 1) 
                { 
                    GameObject bulletCopy = Instantiate(bulletAuto, firePoint.transform.position, Quaternion.identity);
                    bulletCopy.GetComponent<Rigidbody>().AddForce(mouseDir * thrust);
                    isFiring = true;
                    Invoke("ResetFire", 0.1f);
                } else if(weaponSelected == 2)
                {
                    GameObject bulletCopy = Instantiate(bulletPistol, firePoint.transform.position, Quaternion.identity);
                    bulletCopy.GetComponent<Rigidbody>().AddForce(mouseDir * thrust);
                    isFiring = true;
                    Invoke("ResetFire", 0.25f);
                } else if(weaponSelected == 3)
                {
                    GameObject bulletCopy = Instantiate(bulletShotgun, firePoint.transform.position, Quaternion.identity);
                    GameObject bulletCopy2 = Instantiate(bulletShotgun, firePoint2.transform.position, Quaternion.identity);
                    GameObject bulletCopy3 = Instantiate(bulletShotgun, firePoint3.transform.position, Quaternion.identity);
                    var mouseDir2 = Quaternion.AngleAxis(-10, Vector3.up) * mouseDir;
                    var mouseDir3 = Quaternion.AngleAxis(+10, Vector3.up) * mouseDir;
                    bulletCopy.GetComponent<Rigidbody>().AddForce(mouseDir * thrust);
                    bulletCopy2.GetComponent<Rigidbody>().AddForce(mouseDir2 * thrust);
                    bulletCopy3.GetComponent<Rigidbody>().AddForce(mouseDir3 * thrust);

                    isFiring = true;
                    Invoke("ResetFire", 0.5f);
                }
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
        } else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (points > 0 && heartSlider.value < 1f)
            {
                heartSlider.value += 0.1f;
                maxHealth += 20;
                health += 20;
                slider.value = health / maxHealth;
                points--;
                pointsDisplay.text = points.ToString() + " Points";
            }
        } else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (points > 0 && damageSlider.value < 1f)
            {
                damageSlider.value += 0.1f;
                damage += 5;
                points--;
                pointsDisplay.text = points.ToString() + " Points";
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            weaponSelected--;
            Vector3 pos = square.GetComponent<RectTransform>().position;
            if (weaponSelected == 0)
            {
                weaponSelected = 3;
                pos.x = pos.x + 220;
            } else
            {
                pos.x = pos.x - 110;
            }
            square.GetComponent<RectTransform>().position = pos;
            if(weaponSelected == 1)
            {
                amplifiedDamage = damage;
            } else if(weaponSelected == 2)
            {
                amplifiedDamage = 3 * damage;
            } else if(weaponSelected == 3)
            {
                amplifiedDamage = 2 * damage;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            weaponSelected++;
            Vector3 pos = square.GetComponent<RectTransform>().position;
            if (weaponSelected == 4)
            {
                weaponSelected = 1;
                pos.x = pos.x - 220;
            }
            else
            {
                pos.x = pos.x + 110;
            }
            square.GetComponent<RectTransform>().position = pos;
            if (weaponSelected == 1)
            {
                amplifiedDamage = damage;
            }
            else if (weaponSelected == 2)
            {
                amplifiedDamage = 3 * damage;
            }
            else if (weaponSelected == 3)
            {
                amplifiedDamage = 2 * damage;
            }
        }

        if (enemiesKilled == 5)
        {
            points++;
            pointsDisplay.text = points.ToString() + " Points";
            enemiesKilled = 0;
        }
    }

    void ResetReload()
    {
        isReloading = false;
        ammoDisplay.text = ammo.ToString() + "/" + restAmmo.ToString();
    }

    void ResetFire()
    {
        isFiring = false;
    }

    void FixedUpdate()
    {
        transform.position = transform.position + new Vector3(horizontalInput * movementSpeed * Time.deltaTime, 0f, verticalInput * movementSpeed * Time.deltaTime);
        FollowHealthBar();
    }

    void FollowHealthBar()
    {
        slider.value = health / maxHealth;
        healthBar.transform.position = transform.position;
        Vector3 pos = transform.position;
        pos.z = pos.z + 1.5f;
        healthBar.transform.position = pos;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Ammo(Clone)")
        {
            Destroy(other.gameObject);
            restAmmo += 120;
            ammoDisplay.text = ammo.ToString() + "/" + restAmmo.ToString();
            if (ammo == 0)
            {
                isReloading = true;
                Invoke("ResetReload", 2);
                ammo += 60;
                restAmmo -= 60;
            }
        } else if(other.gameObject.name == "Heart(Clone)")
        {
            Destroy(other.gameObject);
            health += maxHealth / 10;
            if(health > maxHealth)
            {
                health = maxHealth;
            }
            slider.value = health / maxHealth;
        }
        else if (other.gameObject.name.Contains("Bullet"))
        {
            takeHit(20);

            ContactPoint contact = other.contacts[0];
            Instantiate(GameObject.Find("Blood"), contact.point + (gameObject.transform.position - contact.point).normalized * 2f, Quaternion.FromToRotation(Vector3.up, contact.normal * -1));
        }
    }

    public void takeHit(float damage)
    {
        health = health - damage;

        if (health <= 0)
        {
            SceneManager.LoadScene("DeadScreen");
        }
    }
}
