using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacterScript : MonoBehaviour
{

    private int x;
    private int y;
    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }


    private int movement = 3;
    public int Movement { get => movement; }

    private bool hasBeenClicked = false;
    public bool HasBeenClicked { get => hasBeenClicked; }
    
    private bool canInteract = true;
    public bool CanInteract { get => canInteract; set => canInteract = value; }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
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
        hasBeenClicked = true; 
    }
}
