using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using utk;

public class Gameplay : MonoBehaviour
{
    class Card {
        public int Id;
        public string Name;
        public int chargesInitial;
        public int chargesCurrent;

        public void Activate () {
            print(Name + " activated");
        }
    }

    List<Card> deck = new List<Card>();
    List<Card> hand = new List<Card>();
    int cardSelected = 0;

    public int deckSizeInitial;
    public int handSizeInitial;

    void Start()
    {
        for (int i = 0; i < deckSizeInitial; i++) {
            Card c = new Card();
            c.Id = i;
            c.Name = i.ToString();
            c.chargesInitial = Random.Range(1, 5);
            c.chargesCurrent = c.chargesInitial;
            deck.Add(c);
        }

        Shuffler.Shuffle(deck);

        hand = Draw(handSizeInitial);
    }

    List<Card> Draw (int amount) {
        List<Card> cards = new List<Card>();
        int cardsDrawn = 0;
        for (int i = 0; i < amount;) {
            if (cardsDrawn < amount) {
                cards.Add(deck[i]);
                deck.RemoveAt(i);
                cardsDrawn++;
                i++;
            } else break;
        }
        return cards;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            ActivateCard(hand[cardSelected].Id);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            cycleHandPositive();
        }
    }

    void cycleHandPositive () {
        cardSelected++;
        if (cardSelected >= hand.Count) cardSelected = 0;
    }

    void ActivateCard (int id) {
        foreach (Card c in hand) {
            if (c.Id == id) {
                c.Activate();
                c.chargesCurrent--;
                if (c.chargesCurrent <= 0) {
                    putCardFromHandToDeckBottom(id);
                    drawCardFromDeck();
                }
                break;
            }
        }
    }

    void putCardFromHandToDeckBottom (int id) {
        foreach (Card c in hand) {
            if (c.Id == id) {
                deck.Add(c);
                c.chargesCurrent = c.chargesInitial;
                hand.Remove(c);
                cycleHandPositive();
                break;
            }
        }
    }

    void drawCardFromDeck () {
        hand.Add(deck[0]);
        deck.RemoveAt(0);
    }
}
