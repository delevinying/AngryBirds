﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour {

	GameState.BulletState state;
	//GameState.SlingshotState slingshotState;
	private string className = "Bird";
	public bool fg = false;

	// Use this for initialization
	void Start () {
		initBullet ();
	}

	private void initBullet(){
		GetComponent<TrailRenderer>().enabled = false;
		GetComponent<TrailRenderer>().sortingLayerName = "Foreground";
		GetComponent<Rigidbody2D>().isKinematic = true;
		GetComponent<CircleCollider2D>().radius = 0.6f;
		state = GameState.BulletState.BeforeThrown;
		this.gameObject.GetComponent<Rigidbody2D> ().drag = 0f;
	}

	void FixedUpdate(){
		//丢出后，速度降了,我们需要将它静止,这时候设计阻力
		if ( state == GameState.BulletState.Thrown && fg == false
			&& GetComponent<Rigidbody2D> ().velocity.sqrMagnitude <= 1) {
			this.gameObject.GetComponent<Rigidbody2D> ().drag = 0.5f;
			StartCoroutine (ReSetGame(1));
		}
	}

	// Update is called once per frame
	float t;
	Vector3 temp = new Vector3(100,100,100);
	float distance;
	void Update () {
		if (temp != new Vector3(100,100,100)) {
			distance = Vector3.Distance (temp,this.gameObject.transform.position);
			t += Time.deltaTime;
			//TraceLog.traceLog (className, "Update", "distance " + distance);
			//TraceLog.traceLog (className, "Update", "state " + state);
		}
		temp = this.gameObject.transform.position;
	}

	//执行丢出
	public void OnThrow() {
		GetComponent<TrailRenderer> ().enabled = true;
		GetComponent<Rigidbody2D> ().isKinematic = false;
		GetComponent<CircleCollider2D> ().radius = 0.6f;
		state = GameState.BulletState.Thrown;
	}

	private void reSetBullet(){
		this.gameObject.active = false;
		this.StopAllCoroutines ();
		fg = true;
	}

	public void reSetFg(){
		fg = false;
	}

	IEnumerator ReSetGame(float seconds){
		yield return new WaitForSeconds (seconds);
		TraceLog.traceLog (className, "ReSetGame", "okoko");
		//reSetBullet ();
	}

}
