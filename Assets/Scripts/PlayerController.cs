using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : Player {

	public PlayingCard toPlay;
	void Start () {
		startPos = originalCard.transform.position;
		defaultScoreText = scoreText.text;
	}
		
	public override void GameStart()
	{
		startPos = originalCard.transform.position;
		//highlight cards to get rid of
		PassCards();
	//	DisplayCards ();
		Destroy (_originalCard.gameObject);
	}
		

	public void PresentCard()
	{
		foreach (PlayingCard c in displayOrder) {
			if (c.selected) {
				c.MoveToCenter ();
			}
		}
	}

	public override void AddToHand(PlayingCard c)
	{
		cards.Add (c);
		c.SetUser (this, true);
		if (c.displayVal == 1) {
			has2C = true;
		}
		if (c.displayVal == 110000) {
			hasQoS = true;
		}
	}

	public override void AddToHand(List<PlayingCard> cardsToAdd)
	{
		string s = "";
		foreach (PlayingCard c in cardsToAdd)
		{
			cards.Add(c);
			c.SetUser (this, true);
			s += c.displayVal + ",";
			if (c.displayVal == 1) {
				has2C = true;
			}
			if (c.displayVal == 110000) {
				hasQoS = true;
			}
		}
		Debug.Log (s);
	}

	private void PassCards()
	{
		List<PlayingCard> cardsToPass = new List<PlayingCard> ();
		clubs = 
			(from card in cards
		 where card.suit == "c"
			orderby card.val descending
				select card).ToList();
		diamonds = 
			(from card in cards
		 where card.suit == "d"
			orderby card.val descending
				select card).ToList();
		spades = 
			(from card in cards
		 where card.suit == "s"
			orderby card.val descending
				select card).ToList();
		hearts = 
			(from card in cards
		 where card.suit == "h"
			orderby card.val descending
				select card).ToList();

		while (cardsToPass.Count () < 3) {
			int countC = clubs.Count ();
			int countD = diamonds.Count ();
			int countS = spades.Count ();
			int countH = hearts.Count ();

			if (countC <= 2 - cardsToPass.Count() && countC > 0) {
				cardsToPass.Add(removeHighest (ref clubs));
				continue;
			}

			if (countS <= 2 - cardsToPass.Count() && countS > 0) {
				cardsToPass.Add(removeHighest (ref spades));
				continue;
			}

			if (countD <= 2 - cardsToPass.Count() && countD > 0) {
				cardsToPass.Add(removeHighest (ref diamonds));
				continue;
			}

			if (countH <=  - cardsToPass.Count() && countH > 0) {
				cardsToPass.Add(removeHighest (ref hearts));
				continue;
			}

			if (countS > 0 && spades.ElementAt (0).val > 11) {
				cardsToPass.Add(removeHighest (ref spades));
				continue;
			}

			if (countH > 0 && hearts.ElementAt (0).val > 10) {
				cardsToPass.Add (removeHighest (ref hearts));
				continue;
			}

			if (countC > 0 && clubs.ElementAt(0).val > 10) {
				cardsToPass.Add(removeHighest (ref clubs));
				continue;
			}

			if (countS > 0 && spades.ElementAt(0).val > 10) {
				cardsToPass.Add(removeHighest (ref spades));
				continue;
			}

			if (countD > 0 && diamonds.ElementAt(0).val > 10) {
				cardsToPass.Add(removeHighest (ref diamonds));
				continue;
			}

			if (countH > 0 && hearts.ElementAt(0).val > 10) {
				cardsToPass.Add(removeHighest (ref hearts));
				continue;
			}

			if (countH > 0) {
				cardsToPass.Add(removeHighest (ref hearts));
				continue;
			}

			if (countS > 0) {
				cardsToPass.Add(removeHighest (ref spades));
				continue;
			}

			if (countC > 0) {
				cardsToPass.Add(removeHighest (ref clubs));
				continue;
			}
				
			if (countD > 0) {
				cardsToPass.Add(removeHighest (ref diamonds));
				continue;
			}

	
		}


		foreach (PlayingCard goal in cardsToPass) {
			int i = 0;
			foreach (PlayingCard c in cards) {
				if (goal.displayVal == c.displayVal) {
					//c.Highlight (Color.cyan);
					c.selected = true;
					break;
				}
				i++;
			}
			//cards.RemoveAt (i);
		}
		controller.SubmitCardsToPass (this, cardsToPass);			
	}

	public void PlayCard()
	{

	}

	public void StartRound(ref List<PlayingCard> cardsInRound)
	{
		if (has2C) {
			foreach (PlayingCard c in cards) {
				if (c.displayVal == 1) {
					c.MoveToCenter ();
					cardsInRound.Add (c);
					controller.suit = c.suit;
					c.selected = true;
					has2C = false;
					return;
				}
			}
		}
		clubs = 
			(from card in cards
				where card.suit == "c"
				orderby card.val ascending
				select card).ToList();
		diamonds = 
			(from card in cards
				where card.suit == "d"
				orderby card.val ascending
				select card).ToList();
		spades = 
			(from card in cards
				where card.suit == "s"
				orderby card.val ascending
				select card).ToList();
		hearts = 
			(from card in cards
				where card.suit == "h"
				orderby card.val ascending
				select card).ToList();

		List<List<PlayingCard>> suits = new List<List<PlayingCard>> ();
		int count = 0;
		suits.Add (clubs);
		suits.Add (diamonds);
		suits.Add (spades);
		suits.Add (hearts);
		suits = (from suit in suits
				where suit.Count() > 0
		         orderby suit.Count() ascending
		         select suit).ToList ();
		List<PlayingCard> fewestSuit = suits.ElementAt (0);
		if (fewestSuit.Equals (spades) && fewestSuit.ElementAt (0).displayVal >= 110000 && suits.Count() > count + 1) {
			count++;
			fewestSuit = suits.ElementAt (count);
		}
		if (fewestSuit.Equals (hearts) && !controller.heartsBroken  && suits.Count () > count + 1) {
			count++;
			fewestSuit = suits.ElementAt (count);
		}
		PlayingCard prev = fewestSuit.ElementAt (0);
		foreach (PlayingCard c in fewestSuit) {
			if (c.val == prev.val) {
				continue;
			}
			if (c.val > prev.val + 1) {
				break;
			}
			prev = c;
		}
		prev.MoveToCenter ();
		cardsInRound.Add (prev);
		controller.suit = prev.suit;
		prev.selected = true;

	}

	public override void PlayCard(ref List<PlayingCard> cardsInRound)
	{
		bool firstRound = controller.gameRound == 0;
		List<PlayingCard> suit = new List<PlayingCard> ();
		List<PlayingCard> cardsOfSuit = new List<PlayingCard> ();
		toPlay = null;
		suit = (from card in cards
			where card.suit.Equals(controller.suit)
				orderby card.val ascending
		        select card).ToList ();
		cardsOfSuit = (from card in cardsInRound where card.suit.Equals(controller.suit) orderby card.val descending
		                select card).ToList ();
		PlayingCard highestInRound = cardsOfSuit.ElementAt (0);
		if (suit.Count () > 0) {
			toPlay = suit.ElementAt (0);
			foreach (PlayingCard c in suit) {
				if (c.val == toPlay.val) {
					continue;
				}
				if (c.val > highestInRound.val) {
					break;
				}
				toPlay = c;
			}
			if (cardsInRound.Count () == 3) {
				bool heartsOrQueen = false;
				foreach (PlayingCard c in cardsInRound) {
					if (c.suit.Equals ("h") || c.displayVal.Equals (110000)) {
						heartsOrQueen = true;
						break;
					}
				}
				if (!heartsOrQueen) {
					suit = (from card in suit orderby card.val descending
						select  card).ToList ();
					toPlay = suit.ElementAt (0);
				}				
			}
				
		} else {
			if (hasQoS && !firstRound) {
				foreach (PlayingCard c in spades) {
					if (c.val == 10) {
						toPlay = c;
						break;
					}
				}
			} else {
				if ((from card in cards
				     where card.val > 8
				     select card).Count () > 0) {
					if (controller.queenSpadesPlayed) {
						hearts = 
							(from card in cards
						  where card.suit == "h"
								orderby card.val descending
						  select card).ToList ();
						if (hearts.Count () > 0) {
							if (controller.heartsBroken && !firstRound) {
								if (cards.Count () < 5) {
									toPlay = hearts.ElementAt (0);
								} else {
									SuitSelect (ref toPlay);
								}
							} else {
								SuitSelect (ref toPlay);
							}
						} else {
							spades = 
								(from card in cards
							  where card.suit == "s"
									orderby card.val descending
							  select card).ToList ();
							if (spades.ElementAt (0).val > 10 && (!(firstRound && spades.ElementAt(0).val == 11))) {
								toPlay = spades.ElementAt (0);
							} else {
								SuitSelect (ref toPlay);
							}
						}
					} else {
						SuitSelect (ref toPlay);
					}
				}
			}
		}
		if (toPlay == null) {
			SuitSelect (ref toPlay);
		}
		if (toPlay.suit.Equals("h") && !controller.heartsBroken) {
			controller.heartsBroken = true;
		}
		if (toPlay.displayVal == 110000) {
			hasQoS = false;
		}
		toPlay.selected = true;
		toPlay.MoveToCenter ();
		cardsInRound.Add (toPlay);
	}

	private void SuitSelect (ref PlayingCard toPlay)
	{
		clubs = 
			(from card in cards
				where card.suit == "c"
				orderby card.val descending
				select card).ToList();
		diamonds = 
			(from card in cards
				where card.suit == "d"
				orderby card.val descending
				select card).ToList();
		spades = 
			(from card in cards
				where card.suit == "s"
				orderby card.val descending
				select card).ToList();
		hearts = 
			(from card in cards
				where card.suit == "h"
				orderby card.val descending
				select card).ToList();

		List<List<PlayingCard>> suits = new List<List<PlayingCard>> ();
		suits.Add (clubs);
		suits.Add (diamonds);
		suits.Add (spades);
		suits.Add (hearts);
		suits = (from suit in suits
			where suit.Count() > 0
			orderby suit.Count() ascending
			select suit).ToList ();

		if (hearts.Count () > 0 && controller.gameRound != 0) {
			if (!(hearts.ElementAt (0).val < 8)) {
				toPlay = hearts.ElementAt (0);
				return;
			} 
		} 
		int count = 1;
		List<PlayingCard> fewest = suits.ElementAt (0);
		while ((fewest.Equals (hearts) && controller.gameRound == 0 && suits.Count () > count) || (fewest.Equals(spades) && hasQoS && suits.Count() > count)) {
			fewest = suits.ElementAt (count);
			count++;
		}
		toPlay = fewest.ElementAt (0);
		if (toPlay.displayVal == 110000 && controller.gameRound == 0) {
			if (fewest.Count > 1) {
				toPlay = fewest.ElementAt (2);
			} else if (suits.Count() > count) {
				fewest = suits.ElementAt (count);
				toPlay = fewest.ElementAt (0);
			}
		}
		toPlay.selected = true;

	}

	private PlayingCard removeHighest(ref List<PlayingCard> suit)
	{
		PlayingCard highest = suit.ElementAt (0);
		if (highest.displayVal == 1) {
			has2C = false;
		}
		if (highest.displayVal == 110000) {
			hasQoS = false;
		}
		suit.RemoveAt (0);
		return highest;
	}


		
}
