using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

public class SceneController : MonoBehaviour {
	[SerializeField] private Sprite[] hearts;
	[SerializeField] private Sprite[] clubs;
	[SerializeField] private Sprite[] diamonds;
	[SerializeField] private Sprite[] spades;
	[SerializeField] private PlayerController[] players;
	[SerializeField] private Player[] allPlayers;
	[SerializeField] private Player user;
	[SerializeField] private GameObject gameOverText;
	[SerializeField] private TextMesh hintText;
	[SerializeField] private GameObject passButton;
	private string defaultHintText;
	private Dictionary <Player, List<PlayingCard>> cardsToPass = new Dictionary<Player, List<PlayingCard>> ();
	private List<PlayingCard> cards = new List<PlayingCard> ();
	public bool _heartsBroken = false;
	private bool _queenSpadesPlayed = false;
	private int _gameRound = -1;

	private string _suit = "n";
	private List<PlayingCard> cardsInRound = new List<PlayingCard> ();


	public bool heartsBroken {
		get { return _heartsBroken; }
		set { _heartsBroken = value; }
	}

	public bool queenSpadesPlayed {
		get { return _queenSpadesPlayed; }
		set { _queenSpadesPlayed = value; }
	}

	public string suit {
		get { return _suit; }
		set { _suit = value; }
	}

	public int gameRound {
		get { return _gameRound; }
	}

	// Use this for initialization
	void Start () {
		defaultHintText = hintText.text;
		passButton.SetActive (false);
		gameOverText.SetActive (false);
		//create Card instances
		Card[] allCards = new Card[52];
		int index = 0;
		for (int i = 0; i < hearts.Length; i++) {
			allCards [index] = new Card ("h", i, hearts[i]);
			index++;
		}
		for (int i = 0; i < clubs.Length; i++) {
			allCards [index] = new Card ("c", i, clubs[i]);
			index++;
		}
		for (int i = 0; i < diamonds.Length; i++) {
			allCards [index] = new Card ("d", i, diamonds[i]);
			index++;
		}
		for (int i = 0; i < spades.Length; i++) {
			allCards [index] = new Card ("s", i, spades[i]);
			index++;
		}
		ShuffleArray<Card> (ref allCards);

		index = 0;
		//display all player's cards (for testing, will just be 1 player in final version)
		for (int i = 0; i < allPlayers.Length; i++) {
			PlayingCard startCard = allPlayers [i].originalCard;
			Vector3 startPos = startCard.transform.position;
			for (int j = 0; j < (52 / allPlayers.Length); j++)
			{
				PlayingCard card;
				Card c = allCards [index];
				card = Instantiate (startCard) as PlayingCard;
				card.SetCard (c.suit, c.val, c.image);
				cards.Add (card);
				allPlayers [i].AddToHand (card);
				index++;

			}
			allPlayers [i].GameStart ();
		}


	}

	void Update()
	{
		if (gameRound == -1) {
			if (user.selectedCards < 3) {
				hintText.text = defaultHintText + " " + user.selectedCards + "/3";
				hintText.gameObject.SetActive (true);
				passButton.SetActive (false);
			} else if (user.selectedCards == 3) {
				hintText.gameObject.SetActive (false);
				passButton.SetActive (true);
			}
		}
	}

	private void ShuffleArray<T>(ref T[] arr)
	{
		T[] newArray = arr.Clone () as T[];
		for (int i = 0; i < newArray.Length; i++) {
			T tmp = newArray [i];
			int r = Random.Range (i, newArray.Length);
			newArray [i] = newArray [r];
			newArray [r] = tmp;
		}
		arr = newArray;
	}
		
	private class Card
	{
		public string suit;
		public int val;
		public Sprite image;

		public Card(string suit, int val, Sprite image)
		{
			this.suit = suit;
			this.val = val;
			this.image = image;
		}
	}

	public void SubmitCardsToPass(Player player, List<PlayingCard> cards)
	{
		cardsToPass.Add (player, cards);
	}

	public void TradeCards()
	{
		passButton.SetActive (false);
		user.PassCards ();
		int i = 0;
		foreach (PlayingCard c in cards) {
			i = c.IfSelectedRemoveFromPlayerHand () ? i + 1: i;
		}

		Player[] temp = allPlayers;
		int index = allPlayers.Length - 1;
		foreach (Player p in allPlayers) {
			List<PlayingCard> value;
			if (cardsToPass.TryGetValue (temp [index], out value)) {
				p.AddToHand (value);
			}
			index--;
		}
		user.DisplayCards ();
		Debug.Log (i);
		StartCoroutine (StartGame ());
	}

	private IEnumerator StartGame()
	{
		int count = 0;
		int playerToStart = 0;
		bool userTurn = false;
		_gameRound++;
		hintText.gameObject.SetActive (false);
		//List<PlayingCard> cardsInRound = new List<PlayingCard> ();
		if (user.has2C) {
			playerToStart = 3;
		} else {
			foreach (PlayerController p in players) {
				if (p.has2C) {
					playerToStart = count;
					break;
				}
				count++;
			}
		}
		yield return new WaitForSeconds(1);
		count = 0;
		int roundWinner = -1;
		while (user.cardNum > 0) {
			if (roundWinner != -1){
				playerToStart = roundWinner;
			}
			if (playerToStart == 3) {
				user.SelectCard ();
				while (user.selectedCards < 1) {
					yield return null;
				}
				user.PlayCard (ref cardsInRound);
				suit = cardsInRound.ElementAt (0).suit;
				count = 0;
			} else {
				players [playerToStart].StartRound (ref cardsInRound);
				count = playerToStart + 1;
			}
		//Debug.Log (playerToStart + " COUNT: " + count);
			PlayingCard highest = cardsInRound.ElementAt(0);
			roundWinner = playerToStart;
			while (count < players.Length + 1 && count != playerToStart) {
				Debug.Log (count + " PLAYER TURN");
				Debug.Log ("card count: " +  cardsInRound.Count());
				yield return new WaitForSeconds (1);
				if (count == players.Length) {
					user.SelectCard ();
					while (user.selectedCards < 1) {
						yield return null;
					}
					user.PlayCard (ref cardsInRound);
					if (cardsInRound.Last ().val > highest.val && cardsInRound.Last ().suit.Equals (suit)) {
						highest = cardsInRound.Last ();
						roundWinner = count;
					}
					count = 0;
				} else {
					players [count].PlayCard (ref cardsInRound);
					if (cardsInRound.Last ().val > highest.val && cardsInRound.Last ().suit.Equals (suit)) {
						highest = cardsInRound.Last ();
						roundWinner = count;
					}
					count++;
				}
			}
			user.cardNum--;
			user.selectedCards = 0;
			suit = "n";
			//_suit = "n";
			yield return new WaitForSeconds(1);

			int roundScore = 0;
			foreach (PlayingCard c in cardsInRound)
			{
				if (c.suit.Equals ("h")) {
					roundScore++;
				} else if (c.displayVal == 110000) {
					roundScore += 13;
				}
			}
			if (roundWinner == 3) {
				user.AddToScore (roundScore);
			} else {
				players [roundWinner].AddToScore (roundScore);
			}
			foreach (PlayingCard c in cardsInRound)
			{
				if (c.suit.Equals ("h")) {
					roundScore++;
				} else if (c.displayVal == 110000) {
					roundScore += 13;
				}
				c.MoveToPlayer (roundWinner == 3 ? user : players [roundWinner]);
			}
			cardsInRound.Clear();
			_gameRound++;
			yield return new WaitForSeconds (1f);
			user.DisplayCards ();
		}
		gameOverText.SetActive (true);
		yield return new WaitForSeconds (3);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex - 1);
	}
		
}
