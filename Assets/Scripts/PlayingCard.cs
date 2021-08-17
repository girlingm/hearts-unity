using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayingCard : MonoBehaviour {


	private string _suit;
	private int _val;
	public int _displayVal;
	public bool _userOwned = false;
	private Player user;
	public bool _selected = false;
	public bool _discarded = false;
	private bool playable = true;
	private Vector3 move = new Vector3(0, 0, 0);
	private Vector3 start;
	private Vector3 defaultPos;
	private float timeTaken = 0.5f;

	public bool discarded {
		get { return _discarded; }
		set { _discarded = value; }
	}

	public string suit{
		get { return _suit; }
	}
	public int val {
		get { return _val; }
	}
	public int displayVal {
		get { return _displayVal; }
	}
	public bool userOwned {
		get { return _userOwned; }
	}
	public bool selected {
		get { return _selected; }
		set { _selected = value; }
	}
		
	public void SetDefaultPos(Vector3 position)
	{
		transform.position = position;
		defaultPos = position;
	}

	public void SetCard(string suit, int val, Sprite image)
	{
		_suit = suit;
		_val = val;
		_displayVal = (val + 1) * (suit == "c" ? 1 : (suit == "d" ? 100 : (suit == "s" ? 10000 : 1000000)));
		GetComponent<SpriteRenderer> ().sprite = image;
	}

	public void SetUser(Player u, bool AIOwned)
	{
		gameObject.SetActive (!AIOwned);
		user = u;
		_userOwned = !AIOwned;
		_selected = false;
		if (!AIOwned) {
			Debug.Log ("PLAYER CARD");
		}
	}

	private void Highlight(Color c)
	{
		SpriteRenderer sprite = GetComponent<SpriteRenderer> ();
		if (sprite != null) {
			sprite.color = c;
		}
	}

	public void OnMouseEnter()
	{
		if (!selected && userOwned && playable) {
			Vector3 pos = transform.position;
			pos.y += 0.2f;
			transform.position = pos;
		} 
	}

	public void OnMouseExit()
	{
		Vector3 pos = transform.position;
		if (!selected && userOwned && playable && !pos.Equals(defaultPos)) {
			pos.y -= 0.2f;
			transform.position = pos;
		} 
	}

	public void OnMouseDown()
	{
		if (userOwned && playable && user.selectedCards < 3) {
			Highlight (Color.cyan);
		}

	}

	public void OnMouseUp()
	{
		if (selected) {
			user.selectedCards--;
			_selected = !selected;
			Highlight (Color.white);
		} else {
			if (userOwned && playable && user.selectedCards < 3) {
				_selected = !selected;
				user.selectedCards++;
			}
		}
	}

	public void MoveToCenter()
	{
		gameObject.SetActive (true);
		start = transform.position;
		if (user.startPos.y == -2.3f) { //player
			move = new Vector3 (0f, -0.5f, -1f);
			timeTaken = (Math.Abs (transform.position.x) / 20) + 0.2f;
		} else if (user.startPos.y == -1f) {
			start = new Vector3 (-7f, 1f, -1f);
			move = new Vector3 (-1f, 0.5f, -1f);
		} else if (user.startPos.y == 0.5f) {
			timeTaken = 0.2f;
			start = new Vector3 (-0.5f, 4f, -1f);
			move = new Vector3 (0f, 1.5f, -1f);
		} else if (user.startPos.y == 2f) {
			start = new Vector3 (5f, 1f, -1f);
			move = new Vector3 (1f, 0.5f, -1f);
		}	
		StartCoroutine (Move (false));
	}
		
	public void Unplayable()
	{
		playable = false;
		transform.position = defaultPos;
		Highlight (Color.gray);
	}

	public void Playable()
	{
		playable = true;
		Highlight (Color.white);
	}

	public bool IfSelectedRemoveFromPlayerHand()
	{
		if (selected) {
			gameObject.SetActive (false);
			user.RemoveFromHand (this);
			if (userOwned) {
				Vector3 pos = transform.position;
				pos.y -= 0.2f;
				transform.position = pos;
			}
			SpriteRenderer sprite = GetComponent<SpriteRenderer> ();
			if (sprite != null) {
				sprite.color = Color.white;
			}
			return true;
		}
		return false;

	}

	public void RemoveFromGame()
	{
		gameObject.SetActive (false);
		user.RemoveFromHand (this);
		Destroy (this.gameObject);
	}

	public void MoveToPlayer(Player p)
	{
		start = transform.position;
		if (p.startPos.y == -2.3f) { //player
			move = new Vector3 (0f, -4f, -1f);
			timeTaken = 0.3f;
		} else if (p.startPos.y == -1f) {
			move = new Vector3 (-7f, 1f, -1f);
		} else if (p.startPos.y == 0.5f) {
			timeTaken = 0.3f;
			move = new Vector3 (-1f, 4f, -1f);
		} else if (p.startPos.y == 2f) {
			move = new Vector3 (5f, 1f, -1f);
		}	
		StartCoroutine (Move (true));
	}

	private IEnumerator Move(bool destroyAfter)
	{

		float speed = 2.0f;
		float percent = 0.1f;
		float startTime = Time.time;
		while (percent <= 1.0f) {
			float timeSinceStart = Time.time - startTime;
			percent = timeSinceStart / timeTaken;
			transform.position = Vector3.Lerp (start, move, percent);
			yield return new WaitForEndOfFrame();
		}
		if (destroyAfter) {
			RemoveFromGame ();
		}
		//transform.Translate (move * speed * Time.deltaTime);
	}
}
