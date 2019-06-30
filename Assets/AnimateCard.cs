using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AnimateCard : MonoBehaviour
{
    TMPro.TMP_Text[] texts;
    Image image;

    bool flipped;

    // Start is called before the first frame update
    void Start()
    {
        texts = GetComponentsInChildren<TMP_Text>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localRotation.y < 1f) transform.Rotate(Vector3.up, 10f);
        if (transform.localRotation.y >= 0.5f && !flipped) {
            foreach (TMPro.TMP_Text text in texts) {
                text.enabled = false;
            }
            Transform darken = transform.Find("Darken");
            darken.gameObject.SetActive(true);
        }
    }
}
