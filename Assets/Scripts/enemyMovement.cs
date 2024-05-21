using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyMovement : MonoBehaviour
{

    public float movementSpeed = 20; // Default = 25 (TBC)
    public float jumpHeight = 1.25f; // Default = 1.25
    public float gravity = -2.45f; // Default = -2.45
    public float vRaycastOffset = 0.1f; //Default = 0.1
    public float hRaycastOffset = 0.35f; //Default = 0.35
    public int damage = 1;
    public Vector2 appliedImpulse;
    public float impulseDuration = 50;

    [SerializeField] private LayerMask layerMask;
    private GameObject player;
    private healthController healthController;


    //Gravity Values

    float verticalPull;

    //Collision values

    float moveTo;
    float vScale;
    float hScale;

    float minY = -float.MaxValue;
    float maxY = float.MaxValue;
    float minX = -float.MaxValue;
    float maxX = float.MaxValue;

    //Collision functions

    RaycastHit2D collisionRaycast(Vector3 pos, Vector2 direction, Vector3 offset)
    {

        RaycastHit2D hit1 = Physics2D.Raycast(pos + offset * 1, direction, Mathf.Infinity, layerMask);
        RaycastHit2D hit2 = Physics2D.Raycast(pos + offset * 0, direction, Mathf.Infinity, layerMask);
        RaycastHit2D hit3 = Physics2D.Raycast(pos + offset * -1, direction, Mathf.Infinity, layerMask);

        RaycastHit2D targetRay = hit2;

        if (hit1.distance <= targetRay.distance)
        {
            targetRay = hit1;
        }
        if (hit3.distance <= targetRay.distance)
        {
            targetRay = hit3;
        }

        return targetRay;

    }

    void hitPlayer()
    {
        healthController.takeDamage(damage, new Vector2(appliedImpulse.x * moveTo, appliedImpulse.y), impulseDuration);
    }


    void Start()
    {
        vScale = transform.localScale.y / 2;
        hScale = transform.localScale.x / 2;
        moveTo = Random.Range(0f, 1f) == 0 ? -1f : 1f;
        player = GameObject.FindGameObjectWithTag("Player");
        healthController = player.GetComponent<healthController>();

    }

    //Objects

    void Update()
    {

        float horizontalInput = Input.GetAxis("Horizontal");
        float tempGravity = gravity;

        //Jump & gravity
        verticalPull = (transform.position.y <= minY) ? 0 : Mathf.Clamp(verticalPull + tempGravity * Time.deltaTime, tempGravity, 0);

        //Collisions

        //Floor collider

        RaycastHit2D floorHit = collisionRaycast(transform.position + new Vector3(0, -vScale + vRaycastOffset, 0), Vector2.down, new Vector3(hScale - vRaycastOffset, 0, 0));
        if (floorHit.collider != null)
        {
            minY = floorHit.point.y + vScale;
        }
        else
        {
            minY = -float.MaxValue;
        }

        //Roof collider

        RaycastHit2D roofHit = collisionRaycast(transform.position + new Vector3(0, vScale - vRaycastOffset, 0), Vector2.up, new Vector3(hScale - vRaycastOffset, 0, 0));
        if (roofHit.collider != null)
        {
            maxY = roofHit.point.y - vScale;
        }
        else
        {
            maxY = float.MaxValue;
        }

        //Right collider

        RaycastHit2D rightHit = collisionRaycast(transform.position + new Vector3(0, hScale - hRaycastOffset, 0), Vector2.right, new Vector3(0, vScale - hRaycastOffset, 0));
        if (rightHit.collider != null)
        {
            maxX = rightHit.point.x - hScale;
        }
        else
        {
            maxX = float.MaxValue;
        }

        //Left collider

        RaycastHit2D leftHit = collisionRaycast(transform.position + new Vector3(0, -hScale + hRaycastOffset, 0), Vector2.left, new Vector3(0, vScale - hRaycastOffset, 0));
        if (leftHit.collider != null)
        {
            minX = leftHit.point.x + hScale;
        }
        else
        {
            minX = -float.MaxValue;
        }


        Vector3 movement = new Vector3(moveTo, verticalPull) * movementSpeed * Time.deltaTime;
        transform.Translate(movement);

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);


        //Damage collisions

        //Left damager
        if (transform.position.x == minX)
        {
            if (leftHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                hitPlayer();
            }
            moveTo = 1;
        }
        else if (transform.position.x == maxX)
        {
            if (rightHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                hitPlayer();
            }
            moveTo = -1;
        }
        else if (transform.position.y == maxY)
        {
            if (roofHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                hitPlayer();
            }
        }
        else if (transform.position.y == minY)
        {
            if (floorHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                hitPlayer();
            }
        }

        /*
        print("MaxX : " + maxX);
        print("MinX : " + minX);
        print("MaxY : " + maxY);
        print("MinY : " + minY);
        print("--------------");
        */

    }
}
