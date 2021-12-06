using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    // Camera target & vector track
    public Transform player;
    private Vector3 track;

    // Start is called before the first frame update
    void Start()
    {

        // Set the camera track on camera's position
        track = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        // Set the camera to track player's horizontal movement
        track.x = player.position.x;
        this.transform.position = track;
    }
}
