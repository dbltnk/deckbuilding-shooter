using System.Collections;
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
        public int ChargesInitial;
        public int ChargesCurrent;
        public Color color;

        public void Activate () {
            print(Name + " activated");
        }

        public Ability Ability;
    }

    public class Ability {
        public string Name;
        public float ProjectileSpeed;
        public float ProjectileSize;
        public float ProjectileLifetime;
        public Color ProjectileColor;
        public float Timeout;
    }

    public List<Ability> AbilitiesAvailable = new List<Ability>();

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
        Ability fireball = new Ability();
        fireball.Name = "Fireball";
        fireball.ProjectileSpeed = 2f;
        fireball.ProjectileSize = 0.6f;
        fireball.ProjectileLifetime = 1.5f;
        fireball.ProjectileColor = Color.red;
        fireball.Timeout = 3f;
        AbilitiesAvailable.Add(fireball);

        Ability lightningOrb = new Ability();
        lightningOrb.Name = "Lightning Orb";
        lightningOrb.ProjectileSpeed = 4f;
        lightningOrb.ProjectileSize = 0.3f;
        lightningOrb.ProjectileLifetime = 0.75f;
        lightningOrb.ProjectileColor = Color.yellow;
        lightningOrb.Timeout = 1f;
        AbilitiesAvailable.Add(lightningOrb);

        Ability ping = new Ability();
        ping.Name = "Ping";
        ping.ProjectileSpeed = 8f;
        ping.ProjectileSize = 0.15f;
        ping.ProjectileLifetime = 0.15f;
        ping.ProjectileColor = Color.gray;
        ping.Timeout = 0.33f;
        AbilitiesAvailable.Add(ping);

        for (int i = 0; i < deckSizeInitial; i++) {
            Card c = new Card();
            int ability = Random.Range(0, AbilitiesAvailable.Count);
            c.Ability = AbilitiesAvailable[ability];
            c.Id = i;
            c.ChargesInitial = Random.Range(1, 5);
            c.ChargesCurrent = c.ChargesInitial;
            c.Name = c.Ability.Name + " (" + c.ChargesInitial.ToString() + ")";
            c.color = c.Ability.ProjectileColor;
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
            chargesInitialCard.text = c.ChargesInitial.ToString();
            chargesCurrentCard.text = c.ChargesCurrent.ToString();
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
                c.ChargesCurrent--;
                if (c.ChargesCurrent <= 0) {
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
                c.ChargesCurrent = c.ChargesInitial;
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

        // TODO TIMEOUT BETWEEN ABILITY USES
        if (Input.GetKeyDown(KeyCode.Mouse0)) controller.LaunchProjectile(point, hand[cardSelected]);
    }
}
