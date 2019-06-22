using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    class Card {
        public int Id;
        public string Name;
        public int charges;

        public void Activate () {
            charges--;
            print(Name);
            if (charges <= 0) print("putCardFromHandToDeckBottom");
        }
    }

    List<Card> deck = new List<Card>();
    List<Card> hand = new List<Card>();
    int cardSelected = 0;

    public int deckSizeInitial = 30;
    public int handSizeInitial = 5;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < deckSizeInitial; i++) {
            Card c = new Card();
            c.Id = i;
            c.Name = i.ToString();
            c.charges = Random.Range(1, 5);
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            hand[cardSelected].Activate();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            cycleHandPositive();
        }
    }

    void cycleHandPositive () {
        cardSelected++;
        if (cardSelected > handSizeInitial) cardSelected = 0;
    }

    void putCardFromHandToDeckBottom (int id) {
        // TODO
    }
}
