using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour {
	private float timeTaken = 5f;
	private Vector3 startPos;
	private bool finishedFalling = false;
	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
		if (finishedFalling) {
			StartCoroutine (Fall ());
		}
	}

	public void StartFalling()
	{
		startPos = transform.position;
		StartCoroutine (Fall ());
	}

	private IEnumerator Fall()
	{
		finishedFalling = false;
		float speed = 2.0f;
		float percent = 0.1f;
		timeTaken = Random.Range (3, 10);
		float startTime = Time.time;
		Vector3 move = new Vector3 (transform.position.x, -5f, -1); 
		while (percent <= 1.0f) {
			float timeSinceStart = Time.time - startTime;
			percent = timeSinceStart / timeTaken;
			transform.position = Vector3.Lerp (startPos, move, percent);
			yield return new WaitForEndOfFrame();
		}
		finishedFalling = true;
	}
}
