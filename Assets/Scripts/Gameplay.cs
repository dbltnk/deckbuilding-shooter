﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTK;
using UnityEngine.UI;
using TMPro;

public class Gameplay : MonoBehaviour
{
    public class Card {
        public int Id;
        public string Name;
        public int chargesInitial;
        public int chargesCurrent;
        public Color color;

        public void Activate () {
            print(Name + " activated");
        }
    }

    List<Card> deck = new List<Card>();
    List<Card> hand = new List<Card>();
    int cardSelected = 0;

    public int deckSizeInitial;
    public int handSizeInitial;

    public GameObject PrefCard;
    public Transform HandUIRoot;
    public GameObject PrefSelector;
    public Transform HUDRoot;
    GameObject PrefSelectorInstance;

    public GameObject Character;
    RubyController controller;
    private Camera cam;

    void Start ()
    {
        for (int i = 0; i < deckSizeInitial; i++) {
            Card c = new Card();
            c.Id = i;
            c.Name = i.ToString();
            c.chargesInitial = Random.Range(1, 5);
            c.chargesCurrent = c.chargesInitial;
            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            c.color = new Color(r, g, b);
            deck.Add(c);
        }

        Cards.Shuffle(deck);

        hand = Draw(handSizeInitial);

        controller = Character.GetComponent<RubyController>();
        cam = Camera.main;
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

        foreach (Transform child in HandUIRoot) {
            GameObject.Destroy(child.gameObject);
        }

        int counter = 0;
        foreach (Card c in hand) {
            GameObject go = Instantiate(PrefCard, HandUIRoot);
            TMP_Text nameCard = go.transform.Find("Name").GetComponent<TMP_Text>();
            TMP_Text chargesInitialCard = go.transform.Find("Charges").transform.Find("ChargesInitial").GetComponent<TMP_Text>();
            TMP_Text chargesCurrentCard = go.transform.Find("Charges").transform.Find("ChargesCurrent").GetComponent<TMP_Text>();
            nameCard.text = c.Name;
            chargesInitialCard.text = c.chargesInitial.ToString();
            chargesCurrentCard.text = c.chargesCurrent.ToString();
            go.GetComponent<Image>().color = c.color;

            float scale = 1f;
            switch (counter) {
                case 0: scale = 1f; break;
                case 1: scale = 1.1f; break;
                case 2: scale = 1.2f; break;
                case 3: scale = 1.1f; break;
                case 4: scale = 1f; break;
                default: scale = 1f; break;
            }
            go.transform.localScale = new Vector3(scale, scale, scale);

            bool selected = (counter == cardSelected) ? true : false;
            go.transform.Find("Frame").gameObject.SetActive(selected);
        
            counter++;
        }

        if (PrefSelectorInstance != null) GameObject.Destroy(PrefSelectorInstance);
        Transform uiCardSelected = HandUIRoot.GetChild(cardSelected);
        PrefSelectorInstance = Instantiate(PrefSelector, uiCardSelected.position + new Vector3(0f, 40f * uiCardSelected.localScale.magnitude, 0f), uiCardSelected.rotation, HUDRoot);

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
                //cycleHandPositive();
                break;
            }
        }
    }

    void drawCardFromDeck () {
        hand.Add(deck[0]);
        deck.RemoveAt(0);
    }

    void OnGUI () {
        Vector3 point = new Vector3();
        Event currentEvent = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;

        point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

        if (Input.GetKeyDown(KeyCode.Mouse0)) controller.LaunchProjectile(point, hand[cardSelected]);
    }
}
