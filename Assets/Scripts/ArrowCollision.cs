using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollision : MonoBehaviour
{

    // Holds destruction timer
    private float timer;

    // Start is called before the first frame update
    void Start()
    {

        timer = 5;
    }

    // Method for collision trigger on enter
    void OnTriggerEnter2D(Collider2D collide)
    {

        // Continue for player object
        if (collide.transform.tag == "Player")
        {

            // Deal damage
            collide.GetComponent<PlayerController>().TakeDamage(20);

            // Destroy arrow
            Destroy(this.gameObject, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {

        // Count down timer
        timer -= Time.deltaTime;

        // Continue if timer is up
        if (timer <= 0)
        {

            // Destroy arrow
            DestroyImmediate(this.gameObject, true);
        }
    }
}
