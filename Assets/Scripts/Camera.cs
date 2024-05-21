using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    //Values

    public float cameraSmooth = 0.25f;
    public float cameraZoom = 10;
    public float cameraMultiplier = 100;

    //Objects

    public GameObject character;
    private Camera cameraComponent;

    private void Start()
    {
        cameraComponent = GetComponent<Camera>();
    }

    void Update()
    {

        Vector3 charPos = character.transform.position;
        Vector3 camPos = transform.position;

        float distance = Vector2.Distance(character.transform.position, transform.position);
        Vector3 direction = new Vector2(charPos.x, charPos.y) - new Vector2(camPos.x, camPos.y);
        direction.Normalize();

        transform.Translate(((distance * direction) / cameraSmooth) * Time.deltaTime);
        cameraComponent.orthographicSize = cameraZoom + (cameraZoom * (distance / cameraSmooth) / cameraMultiplier);

    }
}
