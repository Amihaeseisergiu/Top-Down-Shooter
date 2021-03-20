using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{

    public GameObject enemy;
    private GameObject player;
    private Transform playerPos;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerPos = player.transform;
        StartCoroutine(spawn());
    }

    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator spawn()
    {
        while (player != null)
        {
            yield return new WaitForSeconds(2);
            var spawned = false;
            while (!spawned && playerPos != null)
            {
                var theNewPos = new Vector3(Random.Range(-50f, 50f), 0.5f, Random.Range(-50f, 50f));
                float dist = Vector3.Distance(theNewPos, playerPos.position);
                if (dist >= 15f)
                {
                    GameObject enemyCopy = Instantiate(enemy);
                    enemyCopy.transform.position = theNewPos;
                    spawned = true;
                }
            }
        }
    }
}
