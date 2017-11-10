using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour {

	GameState.BirdState state;
    // Use this for initialization
    void Start () {
        GetComponent<TrailRenderer>().enabled = false;
        GetComponent<TrailRenderer>().sortingLayerName = "Foreground";
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<CircleCollider2D>().radius = 0.6f;
       	state = GameState.BirdState.Thrown;
    }

	void FixedUpdate(){
		if ( state == GameState.BirdState.Thrown
		   && GetComponent<Rigidbody2D> ().velocity.sqrMagnitude <= 1) {
		//	StartCoroutine (DestroyAfter(2));
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnThrow() {
		//GetComponent<AudioSource> ().Play ();
		GetComponent<TrailRenderer> ().enabled = true;
		GetComponent<Rigidbody2D> ().isKinematic = false;
		GetComponent<CircleCollider2D> ().radius = 0.6f;
		state = GameState.BirdState.Thrown;
    }

	IEnumerator DestroyAfter(float seconds){
		yield return new WaitForSeconds (seconds);
		Destroy (gameObject);
	}

}
