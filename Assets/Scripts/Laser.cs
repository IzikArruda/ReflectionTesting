using UnityEngine;
using System.Collections;

/*
 * This script controls all the laser's movements. The position and direction vectors
 * must be set by the script when it creates a laser object. 
 * 
 * Currently, lasers have two different reactions upon collisions : reflect and deactivate. 
 * Reflection simply reflects the lasers trajectory using the hit vertex's normal.
 * Deactivate will stop the laser at where it hit.
 * 
 * There are two different states a laser can be in, noted by the variable "action":
 * 0 will move the laser along its trajectory and will be destroyed upon hitting a wall.
 * 1 will show a predicted path of how the laser will move if we were to step a set amount of frames ahead.
 */
public class Laser : MonoBehaviour {

	//save the laser's current position in the world
	public Vector3 laserPosition;
	
	//save the laser's current directionnal vector that points towards where it's headed
	public Vector3 laserDirection;
	
	//Track the amount of ticks in which the laser will be active for
	//public Int lifetime;
	
	//The amount of distance the laser will travel after each tick
	public float velocity;

    /* The kind of action that will be done for each laser : 
     * 0 = The laser will move it's velocity after each frame.
     * 1 = keep the laser static while it predicts the path it will take after 100 frames. */
    public int action;
	

	// Use this for initialization
	void Start(){
	}
	
	/* 
	 * LateUpdate is called once per frame before the scene is rendered. This will
	 * be where the laser movement is calculated, which is when all objects have
	 * already finished moving around the scene from the physics. I think you want to move
     * the lasers once all other objects have moved, its a case-by-case basis for this I guess
	 */
	void Update(){
        /* Laser movement is done through raytracing along its direction vector for its set "velocity" each frame.
		 * There are a two events that can happen when calculating its path:
		 * The ray does not collide with anything or it hits an object along its path.
         * 
		 * If the ray hits nothing, then its movement for this frame is simply reaching the end of that ray.
         * 
		 * If it does collide, then the reaction will be dependent on the object it hit. 
		 * Walls will stop the ray and deactivate it, mirrors will reflect the ray and continue
		 * with the tracing and using the remaining distance to travel (self-referencing function?).
		 */

        /* Set a variable to track how far the laser needs to travel this frame. 
         * This distance increases if we are trying to predict the laser trajectory. */
        float distance = velocity;
        if(action == 1) {
            distance *= 100;
        }
        
        /* If we want to recover from a prediction of the path of the laser, we will need to save its current direction */
        Vector3 savedDirection = laserDirection;

        /* Call ContinuousTrace to ray trace until the laser travels its set distance */
        while(distance > 0.0f){
			ContinuousTrace(ref distance);
        }

        /* If we have predicted the path of the laser, we will now recover the laser's original position and direction */
        if (action == 1){
            laserPosition = transform.position;
            laserDirection = savedDirection;
        }
        /* Else move the laser to the newly calculated laserPosition */
        else if (action == 0){
            transform.position = laserPosition;
        }
    }
	
	
	/* 
     * Using the given distance and the laser's trajectory, Set and fire a ray along its path.
	 * Depending on whether the ray collides with something or not, decrement the distance
	 * and move the laser's body. It will need to change its direction vector at times.
	 */
	void ContinuousTrace(ref float distance){
	
		/* Initilize ray variables to use when raytracing */
		Ray trajectoryRay = new Ray(laserPosition, laserDirection);
        RaycastHit rayHitInfo;
        
        /* Fire the ray and check for a collision */
        if(Physics.Raycast(trajectoryRay, out rayHitInfo, distance)){
        	/* Hitting an object will cause the laser to move to where the collision takes place and 
        	 * decrement the distance by however much it travelled. To prevent the loop from never ending,
             * always remove a bit of distance upon a reflection */
            Debug.DrawLine(laserPosition, rayHitInfo.point, Color.green);
            distance = distance - (rayHitInfo.distance) - 0.1f;
            laserPosition += laserDirection * rayHitInfo.distance;
            /* Reflect the laser's direction vector off the hit object using its normal */
            laserDirection = ReflectingVector(laserDirection, rayHitInfo.normal);

            /* Hitting an object has sepperate reactions for the different laser "action" values */
            if(rayHitInfo.transform.tag == "Mirror") {
                //The ray hit a mirror - continue the reflection process
            }else {
                if(action == 0) {
                    /* Destroy the active laser once it hits something */
                }else if(action == 1) {
                    /* Stop the prediction at the collision spot */
                    distance = 0.0f;
                }
            }
        }
        else{
        	/* Nothing was blocking the ray's path, so move the body and set the distance pointer to 0 */
            Debug.DrawLine(laserPosition, laserPosition + laserDirection * distance, Color.green);
            laserPosition = laserPosition + laserDirection * distance;
            distance = 0.0f;
        }
	}

    /* 
     * Calculate the reflected vector if the vector "ray" hits a reflective surface with a normal "normal"
     */
    Vector3 ReflectingVector(Vector3 ray, Vector3 normal){
        Vector3 reflectionVector = new Vector3(0, 0, 0);
        reflectionVector = (-2*(DotProduct(ray, normal))*normal) + ray;
        return reflectionVector;
    }

    /* 
     * Calculate the dot product of two Vector3 
     */
    float DotProduct(Vector3 a, Vector3 b){
        float product = 0.0f;
        product = (a.x*b.x) + (a.y*b.y) + (a.z*b.z);
        return product;
    }
}
