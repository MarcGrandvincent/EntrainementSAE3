using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayableCharacterScript : MonoBehaviour
{

    private int x;
    private int y;
    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }




    private int maxMovement = 3;
    public int MaxMovement { get => maxMovement; }

    private int movement = 0;
    public int Movement { get => movement; set => movement = value; }

    private bool hasBeenClicked = false;
    public bool HasBeenClicked { get => hasBeenClicked; }
    
    private bool canInteract = true;
    public bool CanInteract { get => canInteract; set => canInteract = value; }


    private bool hasNoMovement;
    public bool HasNoMovement { get => hasNoMovement; }

    private bool isMoving = false;
    public bool IsMoving { get => isMoving; set => isMoving = value; }

    // Start is called before the first frame update
    void Start()
    {
        movement = MaxMovement;
    }

    // Update is called once per frame
    void Update()
    {

        if (Movement == 0)
        {
            hasNoMovement = true;
            gameObject.GetComponent<Renderer>().material.color = new Color(gameObject.GetComponent<Renderer>().material.color.r, gameObject.GetComponent<Renderer>().material.color.g, gameObject.GetComponent<Renderer>().material.color.b, 0.5f);
        }
        else
        {
            hasNoMovement = false;
        }
    }


    private void OnMouseEnter()
    {
        if (CanInteract)
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(gameObject.GetComponent<Renderer>().material.color.r, gameObject.GetComponent<Renderer>().material.color.g, gameObject.GetComponent<Renderer>().material.color.b, 0.5f);
        }
    }

    private void OnMouseExit()
    {
        if (CanInteract)
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(gameObject.GetComponent<Renderer>().material.color.r, gameObject.GetComponent<Renderer>().material.color.g, gameObject.GetComponent<Renderer>().material.color.b, 1);
        }
    }

    private void OnMouseUp()
    {
        if (CanInteract)
        {
            if (!hasNoMovement)
            {
                hasBeenClicked = true;
                gameObject.GetComponent<Renderer>().material.color = new Color(gameObject.GetComponent<Renderer>().material.color.r, gameObject.GetComponent<Renderer>().material.color.g, gameObject.GetComponent<Renderer>().material.color.b, 0.5f);
            }
        }
    }


    public void isInteractive()
    {
        CanInteract = true;
        gameObject.GetComponent<Renderer>().material.color = new Color(gameObject.GetComponent<Renderer>().material.color.r, gameObject.GetComponent<Renderer>().material.color.g, gameObject.GetComponent<Renderer>().material.color.b, 1);
    }

    public void IsNotClicked()
    {
        hasBeenClicked = false;
        gameObject.GetComponent<Renderer>().material.color = new Color(gameObject.GetComponent<Renderer>().material.color.r, gameObject.GetComponent<Renderer>().material.color.g, gameObject.GetComponent<Renderer>().material.color.b, 1);
    }

}
