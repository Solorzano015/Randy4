using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class catScript : MonoBehaviour
{

    public int catID;
    public Canvas Canvas;

    private Transform catsFolder;
    private Transform catImage;

    private RawImage RawImage;

    void Start()
    {
        string catName = catID.ToString();

        catsFolder = Canvas.transform.Find("Cats");
        catImage = catsFolder.Find(catName);
        RawImage = catImage.gameObject.GetComponent<RawImage>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {;

        catImage.gameObject.SetActive(true);
        gameObject.SetActive(false);

    }

}
