using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {
	[SerializeField] private Heart heart;
	// Use this for initialization
	void Start () {
		StartCoroutine (handleAnimation ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator handleAnimation()
	{
		int count = 0;
		while (count < 2) {
			for (float i = -6; i < 7; i++) {
				Heart h = Instantiate (heart) as Heart;
			h.transform.position = new Vector3 (i, Random.Range(4,7), -1);
				h.StartFalling();
			}
			yield return new WaitForSeconds (1);
			count++;
		}
	}
}
