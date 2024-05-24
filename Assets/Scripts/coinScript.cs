using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class coinRotation : MonoBehaviour
{


    private GameObject player;
    public Canvas Canvas;
    public int coinAmount = 1;
    private TextMeshProUGUI coinText;
    private AudioSource coinSound;
    private SpriteRenderer colorThing;

    bool canPickUp = true;

    private 

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        coinText = Canvas.transform.Find("coinText").GetComponent<TextMeshProUGUI>();
        coinSound = gameObject.GetComponent<AudioSource>();
        colorThing = gameObject.GetComponent<SpriteRenderer>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.gameObject.tag != "Player" || !canPickUp) { return; }

        coinText.text = (int.Parse(coinText.text) + coinAmount).ToString();
        print(int.Parse(coinText.text));
        coinSound.Play();
        colorThing.color = new Color(1f, 1f, 1f, 0f);
        canPickUp = false;

    }

}
