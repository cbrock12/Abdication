using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{

    // Method for collision trigger on enter
    void OnTriggerEnter2D(Collider2D collide)
    {

        // Continue for respawn object
        if (collide.transform.tag == "Respawn")
        {

            // Activate respawn child and deactivate respawn
            collide.transform.GetChild(0).gameObject.SetActive(true);
            collide.transform.GetChild(0).transform.position = collide.transform.position;
            collide.GetComponent<Collider2D>().enabled = false;
        }
    }

    // Method for collision trigger on exit
    void OnTriggerExit2D(Collider2D collide)
    {

        // Continue for enemy object
        if (collide.transform.tag == "Enemy")
        {

            // Activate respawn and deactivate enemy
            collide.transform.parent.GetComponent<Collider2D>().enabled = true;
            collide.gameObject.SetActive(false);
        }
    }
}
