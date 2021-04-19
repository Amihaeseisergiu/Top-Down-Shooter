using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{

    private Transform player;
    public float nextWaypointDistance = 3f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody>();

        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void UpdatePath()
    {
        float dist = Vector3.Distance(rb.position, player.position);

        if(gameObject.GetComponent<EnemyScript>().type != "shooter")
        {
            if (dist <= 7.5f)
            {
                if (seeker.IsDone())
                    seeker.StartPath(rb.position, player.position, OnPathComplete);
            }
            else
            {
                Vector2 vec = Random.insideUnitCircle * dist * 1.25f;
                if (seeker.IsDone())
                    seeker.StartPath(rb.position, player.position + new Vector3(vec.x, 0f, vec.y), OnPathComplete);
            }
        }
        else
        {
            if (dist <= 10.0f)
            {
                gameObject.transform.LookAt(player.position);
                GameObject bulletCopy = Instantiate((GameObject)Resources.Load("Bullet"), gameObject.transform.position + gameObject.transform.forward * gameObject.transform.localScale.x, Quaternion.identity);
                bulletCopy.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * 200.0f);
            }
            else
            {
                Vector2 vec = Random.insideUnitCircle * dist * 1.25f;
                if (seeker.IsDone())
                    seeker.StartPath(rb.position, player.position + new Vector3(vec.x, 0f, vec.y), OnPathComplete);
            }
        }
    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null)
            return;

        if(currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector3 direction = ((Vector3) path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector3 force = direction * this.GetComponent<EnemyScript>().speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector3.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if(distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
}
