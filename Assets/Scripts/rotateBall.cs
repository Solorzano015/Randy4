using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateBall : MonoBehaviour
{

    public int damage = 1;
    public Vector2 appliedImpulse;
    public float impulseDuration = 50;
    private GameObject player;

    private healthController healthController;

    private void hitPlayer()
    {
        healthController.takeDamage(damage, new Vector2(appliedImpulse.x, appliedImpulse.y), impulseDuration);
    }


    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        healthController = player.GetComponent<healthController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            hitPlayer();
        }
        Destroy(gameObject);
    }

    void Update()
    {

        gameObject.transform.rotation = Quaternion.Euler(0, 0, gameObject.transform.rotation.z + 1);

    }
}
