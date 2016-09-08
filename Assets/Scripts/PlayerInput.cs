using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

    //Create a public variable "speed" to multiply the amount of force applied each physics calculations
    public float speed;

    //Create a public variable which determines the object's speed. Should have a sepperate max speed for the Y axis
    private float maxSpeed = 10;

    //The laser object that the user will fire
    public GameObject laserShot;

    //How fast the player's laser shots will move
    public float shotSpeed;

    //Create a public variable to hold a reference to the rigidbody of the player
    private Rigidbody playerBody;


    /* 
     * Start is called on the first frame of the script becoming active
     */
    void Start(){
        playerBody = GetComponent<Rigidbody>();
    }

    /*
     * Update is called before the scene is rendered
     */
    void Update(){

        /* Use the WASD keys to aim the shootingDirection for firing a laser */
        Vector3 shootingDirection = new Vector3(0.0f, 0.0f, 0.0f);
        if(Input.GetKey("w")) {
            shootingDirection.z++;
        }
        if(Input.GetKey("a")) {
            shootingDirection.x--;
        }
        if(Input.GetKey("s")) {
            shootingDirection.z--;
        }
        if(Input.GetKey("d")) {
            shootingDirection.x++;
        }

        /* Check the player's input for a request to fire a laser shot by pressing spacebar. Make sure they have given an aiming direction */
        if (Input.GetKeyDown("space") && !(shootingDirection.x == 0.0f && shootingDirection.y == 0.0f && shootingDirection.z == 0.0f)){
            /* Get the player's position and direction which will be used to place the laser shot.
             * Note: shooting direction must be normalized and keep the Y at 0 */
            shootingDirection.Normalize();
            FireLaser(shootingDirection, playerBody.position);
        }
    }

    /*
     * Fixed update is called before any physics are calculated and applied
     */
    void FixedUpdate(){
    
        /* Set the X and Z movement for keyboard presses. The movement is done by setting the player object's
         * speed in lieu of applying force or acceleration just to keep the movement simple. */
        float moveHorizontal = 0;
        float moveVertical = 0;
        if (Input.GetKey("up")) { moveVertical++; }
        if (Input.GetKey("down")) { moveVertical--; }
        if (Input.GetKey("right")) { moveHorizontal++; }
        if (Input.GetKey("left")) { moveHorizontal--; }

        /* Update the player's velocity in the X and Y axis and keeping their Y speed the same */
        playerBody.velocity = new Vector3(moveHorizontal*speed, playerBody.velocity.y, moveVertical*speed);

        /* Prevent the player from exceeding a limited velocity by clamping the X and Z magnitude
         * together and clamping the Y on it's own. We clamp the Y on it's own to prevent the player's Y
         * movement from effecting the X and Z movement, giving them full control on the primary axis. 
         * 
         * The speed is clamped by removing the Y from the velocity vector, clamping the vector and the Y 
         * velocity sepperatly, then adding the Y velocity back to the vector.
         * 
         * Is clamping better than finding the vector's normal then multiplying by maxSpeed?
         */
        float yVelocity = playerBody.velocity.y;
        yVelocity =  Mathf.Clamp(yVelocity, -maxSpeed, maxSpeed);
        Vector3 clampedVelocity = new Vector3(playerBody.velocity.x, 0.0f, playerBody.velocity.z);
        clampedVelocity = Vector3.ClampMagnitude(clampedVelocity, maxSpeed);
        clampedVelocity.Set(clampedVelocity.x, yVelocity, clampedVelocity.z);
        playerBody.velocity = clampedVelocity;
    }




    /*
     * Create a new laser shot in the scene using the given parameters to place the laser relative to the player
     */
    void FireLaser(Vector3 playerDirection, Vector3 playerPosition){

		/* Set the desired positionnal properties of the laser shot fired from the player */
        laserShot.GetComponent<Laser>().laserPosition = playerPosition;
        laserShot.GetComponent<Laser>().laserDirection = playerDirection;
        laserShot.transform.position = playerPosition;

        /* Set the desired laser-specific properties such as it's velocity */
        //laserShot.GetComponent<Laser>().lifetime = 20.0f;
        laserShot.GetComponent<Laser>().velocity = shotSpeed;
        
        /* Create the new laser object */
        Instantiate(laserShot);
    }
}


