using UnityEngine;
using System.Collections;

/* 
 * For now, have the camera follow the player object's movement without being a child
 */
public class CameraInput : MonoBehaviour {

    //Store the camera's focus target as a public variable
    public GameObject player;

    //Store the distance that is between the camera and it's focus target
    private Vector3 offset;

	// Use this for initialization
	void Start(){

        //Set the camera's offset of the GameObject that it is focusing on
        offset = transform.position - player.transform.position;
    }

    // LateUpdate is called every frame AFTER the Update functions are finished
    void LateUpdate(){

        //Set the camera's new position relative to it's target's position and the camera's offset
        transform.position = player.transform.position + offset;
	}
}
