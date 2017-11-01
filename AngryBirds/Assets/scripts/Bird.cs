using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour {


    // Use this for initialization
    void Start () {
        GetComponent<TrailRenderer>().enabled = false;
        GetComponent<TrailRenderer>().sortingLayerName = "Foreground";
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<CircleCollider2D>().radius = 0.6f;
       // GameState.BirdState state = GameState.BirdState.Thrown;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnThrow() {

    }
}
