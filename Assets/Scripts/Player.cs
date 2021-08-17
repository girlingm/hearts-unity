using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour {

	[SerializeField] protected PlayingCard _originalCard;
	[SerializeField] protected SceneController controller;
	[SerializeField] protected TextMesh scoreText;
	public bool has2C = false;
	public bool hasQoS = false;

	protected List<PlayingCard> cards = new List<PlayingCard> ();
	protected List<PlayingCard> clubs;
	protected List<PlayingCard> diamonds;
	protected List<PlayingCard> spades;
	protected List<PlayingCard> hearts;
	protected List<PlayingCard> displayOrder;
	protected string defaultScoreText;

	protected int score;
	private int _selectedCards = 0;

	public Vector3 startPos;
	private int _cardNum = 0;

	public int cardNum {
		get { return _cardNum; }
		set { _cardNum = value; }
	}
	public int selectedCards
	{
		get { return _selectedCards; }
		set { _selectedCards = value; }
	}

	private int numSelected = 0;

	public const float offsetX = 1f;
	public const float offsetY = 2.5f;

	public PlayingCard originalCard
	{
		get {return _originalCard;}
	}
	// Use this for initialization
	void Start () {
		defaultScoreText = scoreText.text;

	}



	public virtual void GameStart()
	{
		startPos = originalCard.transform.position;
		DisplayCards ();
		Destroy (_originalCard.gameObject);
	}

	public virtual void AddToHand(PlayingCard c)
	{
		cards.Add (c);
		c.SetUser (this, false);
		if (c.displayVal == 1) {
			has2C = true;
		}
		if (c.displayVal == 110000) {
			hasQoS = true;
		}
		_cardNum++;
	}

	public void RemoveFromHand(PlayingCard c)
	{
		cards.Remove (c);
	}

	public virtual void AddToHand(List<PlayingCard> cardsToAdd)
	{
		string s = "";
		foreach (PlayingCard c in cardsToAdd)
		{
			cards.Add(c);
			c.SetUser (this, false);
			if (c.displayVal == 1) {
				has2C = true;
			}
			if (c.displayVal == 110000) {
				hasQoS = true;
			}
			_cardNum++;
		}
		Debug.Log (s);
	}

	public void DisplayCards()
	{
		displayOrder = 
			(from card in cards
				orderby card.displayVal
				select card).ToList();
		Debug.Log ("offset: " + displayOrder.Count ());
		float offset = (13 - displayOrder.Count ()) * (offsetX / 2);
		int i = 0;
		string display = "";
		foreach (PlayingCard c in displayOrder) {
			float posX = offset + (offsetX * i) + startPos.x;
			float posY = startPos.y;
			c.SetDefaultPos(new Vector3(posX, posY, startPos.z - 1));
			i++;
			display += ", " + c.displayVal;
		}
		selectedCards = 0;
	}

	public void PassCards()
	{
		List<PlayingCard> cardsToPass = new List<PlayingCard> ();
		foreach (PlayingCard c in cards) {
			if (c.selected) {
				cardsToPass.Add (c);
				if (c.displayVal == 1) {
					has2C = false;
				}
				if (c.displayVal == 110000) {
					hasQoS = false;
				}
				_cardNum--;
			}
		}
		controller.SubmitCardsToPass (this, cardsToPass);
	}

	public void SelectCard()
	{
		foreach (PlayingCard c in cards) {
				c.Playable ();
		}
		if (controller.gameRound == 0) {
			foreach (PlayingCard c in cards) {
				if (c.displayVal == 110000) {
					c.Unplayable ();
				}
				else if (c.suit.Equals ("h")) {
					c.Unplayable ();
				}
			}
		}
		if (!controller.suit.Equals ("n")) {
			Debug.Log ("not selecting");
			if ((from card in cards
			     where card.suit == controller.suit
			     select card).Count () > 0) {
				foreach (PlayingCard c in cards) {
					if (!c.suit.Equals (controller.suit)) {
						c.Unplayable ();
					}
				}
			}
		}
		else if (has2C) {
			foreach (PlayingCard c in cards) {
				if (!(c.displayVal == 1)) {
					c.Unplayable ();
				}
			}
		} else if (!controller.heartsBroken) {
			Debug.Log ("hearts not broken");
			foreach (PlayingCard c in cards) {
				if (c.suit.Equals ("h")) {
					c.Unplayable ();
				}
			}
		}
			
	}

	public virtual void PlayCard(ref List<PlayingCard> cardsInRound)
	{
		foreach (PlayingCard c in cards) {
			if (c.selected) {
				c.MoveToCenter ();
				cardsInRound.Add (c);
				if (c.displayVal == 1) {
					has2C = false;
				}
				if (!controller.heartsBroken && c.suit.Equals ("h")) {
					controller.heartsBroken = true;
				}
			} else {
				c.Unplayable ();
			}
		}
	}

	public void AddToScore(int val)
	{
		score += val;
		scoreText.text = score.ToString();
	}
}
