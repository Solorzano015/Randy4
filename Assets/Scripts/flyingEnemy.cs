using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flyingEnemy : MonoBehaviour
{

    public int maxRange = 100;
    public float speed = 10;
    public float detectionRange = 50f;
    public float ballThrowDelay = 1f;

    public int damage = 1;
    public Vector2 appliedImpulse;
    public float impulseDuration = 50;

    public float ballForce = 50f;
    public Transform ballPrefab;

    public GameObject player;
    [SerializeField] public LayerMask layerMask;
    private healthController healthController;

    float maxX;
    float minX;

    float lastBallThrow = 0;


    void Start()
    {
        maxX = gameObject.transform.position.x + maxRange;
        minX = gameObject.transform.position.x - maxRange;
        healthController = player.GetComponent<healthController>();
    }

    private void hitPlayer()
    {
        healthController.takeDamage(damage, new Vector2(appliedImpulse.x, appliedImpulse.y), impulseDuration);
    }

    private void createNewBall()
    {
        Vector3 direction = Vector3.Normalize(player.transform.position - gameObject.transform.position);

        Transform ball = Instantiate(ballPrefab) as Transform;
        Physics2D.IgnoreCollision(ball.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>());

        Rigidbody2D ballRigidBody = ball.gameObject.GetComponent<Rigidbody2D>();
        ballRigidBody.transform.position = gameObject.transform.position;

        ballRigidBody.AddForce( new Vector2(direction.x, direction.y) * ballForce);


    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            hitPlayer();
        }

    }

    bool checkForPlayer()
    {

        Vector3 direction = Vector3.Normalize(player.transform.position - gameObject.transform.position);
        RaycastHit2D detectRay = Physics2D.Raycast( gameObject.transform.position, new Vector2(direction.x, direction.y), detectionRange, layerMask);

        return ( detectRay.collider != null && detectRay.collider.gameObject.tag == "Player" );
    }

    // Update is called once per frame
    void Update()
    {

        //Movement

        if (transform.position.x == minX || transform.position.x == maxX)
        {
            speed *= -1f;
        }

        if (checkForPlayer() && Time.fixedTime > lastBallThrow + ballThrowDelay)
        {
            lastBallThrow = Time.fixedTime;
            createNewBall();

        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x + speed * Time.deltaTime, minX, maxX), transform.position.y);

    }
}
