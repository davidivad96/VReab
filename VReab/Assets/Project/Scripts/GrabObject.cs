﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
	private bool active;
	private bool grabbed;
    private bool affect_gravity;
	private Rigidbody rigidbody_component;
    private GameObject player;
    
    public float distance_forward;
    public float distance_up;
    public float distance_right;
    public TurnOnCoffeeMachine turn_on_coffee_script;
    public TurnOnToaster turn_on_toaster_script;
    public PutObject put_object_script;

    // Start is called before the first frame update
    void Start () {
		active = false;
		grabbed = false;
        affect_gravity = true;
		rigidbody_component = gameObject.GetComponent<Rigidbody> ();
        player = GameObject.FindGameObjectWithTag("Player");
    }

	// Update is called once per frame
	void Update () {
		// If the object is grabbed
		if (grabbed) {
			// If detect a "PointerClick" event with the right button of the mouse or the secondary trigger of the bluetooth controller, put down the object
			if (Input.GetButtonDown("Fire2") || Input.GetMouseButtonDown (1)) {
                releaseObject(true);
            } else {
				// Disable gravity and place object in front of the player
				rigidbody_component.useGravity = false;
				transform.position = Camera.main.transform.position + Camera.main.transform.forward * distance_forward + Camera.main.transform.up * distance_up + Camera.main.transform.right * distance_right;
				transform.rotation = Camera.main.transform.rotation;
			}
        } else {
			// If detect a "PointerClick" event, grab the object
			if ((Input.GetButtonDown ("Jump") || Input.GetMouseButtonDown(0)) && active) {
                // Calculate the distance between the player and the object
                float distance = Vector3.Distance (transform.position, Camera.main.transform.position);
				// If the object is close enough to the player, then it can be grabbed
				if (distance <= 5.0f) {
                    // If the player isn't already grabbing anything
                    if (!player.GetComponent<Movement>().getGrabbingSomething()) {
                        grabbed = true;
                        // Set the grabbing_something attribute for the player
                        player.GetComponent<Movement>().setGrabbingSomething(true);
                        // Tell the "TurnOnCoffeeMachine" or the "TurnOnToaster" script that an object has been grabbed
                        if (turn_on_coffee_script != null) {
                            if (gameObject.name == "CoffeeFilter") {
                                // Coffee Filter
                                turn_on_coffee_script.putCoffeeFilter(false);
                            } else if (gameObject.name == "WaterContainer") {
                                // Water Container
                                turn_on_coffee_script.putWaterContainer(false);
                            } else if (gameObject.name == "cup") {
                                // Cup
                                turn_on_coffee_script.putCup(false);
                            }
                        } else if (turn_on_toaster_script != null) {
                            if (gameObject.name == "toast1") {
                                // Toast 1
                                turn_on_toaster_script.putToast1(false);
                            } else if (gameObject.name == "toast2") {
                                // Toast 2
                                turn_on_toaster_script.putToast2(false);
                            }
                        }
                        // Tell the "PutObject" script that an object has been grabbed
                        if (put_object_script != null) {
                            put_object_script.grabObject(gameObject.name);
                        }
                    }
                }
			} else {
				// Enable gravity if this object is affected by gravity
                if (affect_gravity) {
				    rigidbody_component.useGravity = true;
                }
			}
		}
	}

	// Called when there's a "PointerEnter" event
	public void Activate(){
		active = true;
	}

	// Called when there's a "PointerExit" event
	public void Deactivate(){
		active = false;
	}

    // Return the grabbed attribute
    public bool isGrabbed() {
        return grabbed;
    }

    // Release object
    public void releaseObject(bool gravity) {
        grabbed = false;
        // Set the grabbing_something attribute for the player
        player.GetComponent<Movement>().setGrabbingSomething(false);
        // Disable isKinematic so that the object falls when it's dropped
        rigidbody_component.isKinematic = false;
        // The object is affected by gravity or not
        affect_gravity = gravity;
    }

    // Detect collision
    void OnCollisionEnter(Collision col) {
        // If the object is falling and a collision is detected, isKinematic=true
        if (!rigidbody_component.isKinematic && gameObject.transform.position.y < 1.0f) {
            rigidbody_component.isKinematic = true;
        }
    }
}