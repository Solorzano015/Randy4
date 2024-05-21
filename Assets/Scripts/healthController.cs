using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class healthController : MonoBehaviour
{


    public int maxHealth = 5;
    public float hitCountdown;

    public Canvas Canvas;

    private TextMeshProUGUI healthText;
    private Movement Movement;
    private Transform deathScreen;

    float lastHit = 0;

    private void Start()
    {
        Movement = GetComponent<Movement>();
        healthText = Canvas.transform.Find("Health").GetComponent<TextMeshProUGUI>();
        deathScreen = Canvas.transform.Find("deathScreen");

        deathScreen.gameObject.SetActive(false);
    }

    int Health = 5;
    public void takeDamage(int amount, Vector2 appliedImpulse, float impulseDuration)
    {

        if (lastHit >= Time.time) { return; }
        lastHit = Time.time + hitCountdown;

        Movement.applyImpulse(appliedImpulse, impulseDuration);
        Health = Mathf.Clamp(Health - amount, 0, maxHealth);
        healthText.text = "Health : " + Health.ToString();

        if (Health == 0)
        {
            Movement.canMove = false;
            deathScreen.gameObject.SetActive(true);
        }

    }
}
