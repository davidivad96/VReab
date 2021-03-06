﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PutObject : MonoBehaviour {
    private bool active;
    private List<GrabObject> grab_object_scripts = new List<GrabObject>();
    private List<Vector3> initial_positions = new List<Vector3>();
    private List<Quaternion> initial_rotations = new List<Quaternion>();
    private List<bool> objects_putted = new List<bool>();
    private bool all_objects_putted;

    public List<GameObject> objects_to_put;
    public AudioSource success_audio;

    // Awake is called before Start()
    void Awake() {
        foreach (GameObject obj in objects_to_put) {
            grab_object_scripts.Add(obj.GetComponent<GrabObject>());
            initial_positions.Add(obj.transform.position);
            initial_rotations.Add(obj.transform.rotation);
            objects_putted.Add(false);
            all_objects_putted = false;
        }
    }

    // Start is called before the first frame update
    void Start() {
        active = false;
    }

    // Update is called once per frame
    void Update() {
        // If detect a "PointerClick" event
        if ((Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0)) && active) {
            // Find index of the gameObject being grabbed
            int index = objects_to_put.FindIndex(obj => obj.GetComponent<GrabObject>().isGrabbed());
            // If the player is grabbing any of the correct objects, place it in its position
            if (index != -1) {
                // Calculate the distance between the player and the object where to put
                float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
                // If the object where to put is close enough to the player, then the object can be put
                if (distance <= 5.0f) {
                    grab_object_scripts[index].releaseObject(false);
                    objects_to_put[index].transform.position = initial_positions[index];
                    objects_to_put[index].transform.rotation = initial_rotations[index];
                    objects_putted[index] = true;
                    // Play success sound
                    success_audio.Play();

                    // Check if all objects are putted
                    all_objects_putted = true;
                    for (int i = 0; i < objects_putted.Count; i++) {
                        if (objects_putted[i] == false) {
                            all_objects_putted = false;
                        }
                    }

                    // Calculate and assign to canvas the total time (in seconds) last putting the cup or the glass
                    if (objects_to_put[index].name == "cup") {
                        FindInactiveObjectByName("PutCupText").GetComponent<Text>().text = "Colocar la taza: " + Time.time.ToString("F2") + " segundos";
                    } else if (objects_to_put[index].name == "glass") {
                        FindInactiveObjectByName("PutGlassText").GetComponent<Text>().text = "Colocar el vaso: " + Time.time.ToString("F2") + " segundos";
                    }
                }
            }
        }
    }

    // Called when there's a "PointerEnter" event
    public void Activate() {
        active = true;
    }

    // Called when there's a "PointerExit" event
    public void Deactivate() {
        active = false;
    }

    // Called from the "GrabObject" script when an object is grabbed
    public void grabObject(string obj_name) {
        for(int i = 0; i < objects_to_put.Count; i++) {
            if (objects_to_put[i].name == obj_name) {
                objects_putted[i] = false;
            }
        }
    }

    // Called from the "CanvasManager" script
    public bool getAllObjectsPutted() {
        return all_objects_putted;
    }

    // Used to find inactive objects (GameObject.Find() only works for active objects)
    private GameObject FindInactiveObjectByName(string name) {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++) {
            if (objs[i].hideFlags == HideFlags.None) {
                if (objs[i].name == name) {
                    return objs[i].gameObject;
                }
            }
        }
        return null;
    }
}
