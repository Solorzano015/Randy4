using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Movement : MonoBehaviour
{

    public float movementSpeed = 20; // Default = 25 (TBC)
    public float jumpHeight = 1.25f; // Default = 1.25
    public float jumpCooldown = 0.5f; //Default = 0.5
    public float gravity = -2.45f; // Default = -2.45
    public float Dash = 3.5f; //Default = 10
    public float dashFriction = -10; //Default = -20
    public float dashCooldown = 2; //Default = 2
    public float vRaycastOffset = 0.1f; //Default = 0.1
    public float hRaycastOffset = 0.35f; //Default = 0.35
    public float impulseMult = 0.85f; //Default = 1
    public bool canMove = false;

    public bool leftRaycast;
    public bool rightRaycast;
    public bool roofRaycast;
    public bool floorRaycast;

    public GameObject winScreen;

    [SerializeField] private LayerMask layerMask;
    private Transform Randy;
    private SpriteRenderer randySpriteRenderer;
    private Animator randyAnim;

    //Jump values

    Vector2 Impulse = new Vector2(0, 0);
    float verticalPull = 0;
    float jumpForce = 0;
    float lastJump = 0;
    bool isGrounded = false;
    bool isInWater = false;

    //Dash values

    float dashForce = 0;
    float dashTimer = float.MaxValue;
    bool canDash = true;

    //Collision values

    float vScale;
    float hScale;
    public int rayAmount = 2;


    float minY = -float.MaxValue;
    float maxY = float.MaxValue;
    float minX = -float.MaxValue;
    float maxX = float.MaxValue;

    //Collision functions

    RaycastHit2D collisionRaycast(Vector3 pos, Vector2 direction, float amountOfRays, Vector2 vectorAdder, bool testEnabled)
    {

        RaycastHit2D returnHit = Physics2D.Raycast(new Vector2(), new Vector2());
        Vector2 newPos = new Vector2(pos.x, pos.y);
        float tempOffset = vectorAdder == new Vector2(1, 0) ? vRaycastOffset : hRaycastOffset;
        float tempScale = vectorAdder == new Vector2(1, 0) ? hScale : vScale;

        for (float i = 0; i < amountOfRays; i++)
        {
            float mult = i / (amountOfRays - 1);
            Vector2 testpos = ((tempOffset * vectorAdder) + newPos) + (vectorAdder * mult) * ((tempScale - tempOffset) * 2);

            RaycastHit2D currentHit = Physics2D.Raycast(testpos, direction, Mathf.Infinity, layerMask);
            if (returnHit.collider == null || returnHit.distance > currentHit.distance)
            {
                returnHit = currentHit;
            }

        }

        if (returnHit.collider != null)
        {

            if (returnHit.collider.gameObject.tag == "onlyFloorH" && direction != Vector2.down)
            {
                return Physics2D.Raycast(new Vector2(), new Vector2());
            }

        }

        return returnHit;


    }


    public void applyImpulse(Vector2 applyImpulse, float impulseDuration)
    {
        for (int i = 0; i < impulseDuration; i++)
        {
            Task.Delay((int)Time.deltaTime);
            Impulse += applyImpulse * (1 / impulseDuration);
        }

    }

    void Start()
    {
        vScale = transform.localScale.y / 2;
        hScale = transform.localScale.x / 2;
        Randy = transform.Find("Randy");
        randySpriteRenderer = Randy.GetComponent<SpriteRenderer>();
        randyAnim = Randy.GetComponent<Animator>();

    }

    //Objects

    void Update()
    {
        if (!canMove) { return; }

        float horizontalInput = Input.GetAxis("Horizontal");
        float tempGravity = gravity;

        //Paraguas
        if (Input.GetKey(KeyCode.E) && (verticalPull + jumpForce < 0))
        {
            tempGravity *= 0.15f;
        }

        //Impulse

        if (isGrounded)
        {
            Impulse *= new Vector2(0.5f, 0.5f);
        }
        Impulse = Impulse - (Impulse / 1.5f) * Time.deltaTime;

        //Jump & gravity
        verticalPull = (transform.position.y <= minY) ? 0 : Mathf.Clamp(verticalPull + tempGravity * Time.deltaTime, tempGravity, 0);
        jumpForce = Mathf.Clamp(jumpForce + tempGravity * Time.deltaTime, 0, jumpHeight);
        isGrounded = (transform.position.y == minY) ? true : false;

        //Dash
        dashTimer += Time.deltaTime;
        dashForce = Mathf.Clamp(dashForce + dashFriction * Time.deltaTime, 1, Dash);
        canDash = (dashTimer >= dashCooldown) ? true : false;

        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) && isGrounded && lastJump < Time.time)
        {
            lastJump = Time.time + jumpCooldown;
            jumpForce = jumpHeight;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            dashForce = Dash;
            dashTimer = 0;
        }

        //Sprite animations

        randySpriteRenderer.flipX = horizontalInput < 0;
        randyAnim.SetBool("isRunning", horizontalInput != 0);


        //Win screen

        if (transform.position.y > 613)
        {
            canMove = false;
            winScreen.gameObject.SetActive(true);
        }

        //Collisions

        bool isInWater;

        //Floor collider

        RaycastHit2D floorHit = collisionRaycast(transform.position + new Vector3(-hScale, -vScale + vRaycastOffset, 0), Vector2.down, rayAmount, new Vector2(1, 0), floorRaycast);
        if (floorHit.collider != null)
        {
            minY = floorHit.point.y + vScale;
        }
        else
        {
            minY = -float.MaxValue;
        }

        //Roof collider

        RaycastHit2D roofHit = collisionRaycast(transform.position + new Vector3(-hScale, vScale - vRaycastOffset, 0), Vector2.up, rayAmount, new Vector2(1, 0), roofRaycast);
        if (roofHit.collider != null)
        {
            maxY = roofHit.point.y - vScale;
        }
        else
        {
            maxY = float.MaxValue;
        }

        //Right collider

        RaycastHit2D rightHit = collisionRaycast(transform.position + new Vector3(hScale - hRaycastOffset, -vScale, 0), Vector2.right, rayAmount, new Vector2(0, 1), rightRaycast);
        if (rightHit.collider != null)
        {
            maxX = rightHit.point.x - hScale;
        }
        else
        {
            maxX = float.MaxValue;
        }

        //Left collider

        RaycastHit2D leftHit = collisionRaycast(transform.position + new Vector3(-hScale + hRaycastOffset, -vScale, 0), Vector2.left, rayAmount, new Vector2(0, 1), leftRaycast);
        if (leftHit.collider != null)
        {
            minX = leftHit.point.x + hScale;
        }
        else
        {
            minX = -float.MaxValue;
        }




        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        Vector3 movement = new Vector3(horizontalInput * dashForce + Impulse.x, verticalPull + jumpForce + Impulse.y) * movementSpeed * Time.deltaTime;
        transform.Translate(movement);


        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);

        if ((transform.position.y == maxY) || ((transform.position.y == minY)))
        {
            jumpForce = jumpForce/10;
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
