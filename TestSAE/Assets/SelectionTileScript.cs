using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionTileScript : MonoBehaviour
{

    private int x;
    private int y;
    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }
    
    private List<GameObject> voisins = new List<GameObject>();
    public List<GameObject> Voisins { get => voisins; }

    private bool isNotActionnable = false;

    private bool isOver;
    public bool IsOver { get => isOver; }

    private bool hasBeenClick;
    public bool HasBeenClick { get => hasBeenClick; set => hasBeenClick = value; }

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
        if (!isNotActionnable)
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(gameObject.GetComponent<Renderer>().material.color.r, gameObject.GetComponent<Renderer>().material.color.g, gameObject.GetComponent<Renderer>().material.color.b, 0.5f);
        }
    }

    private void OnMouseExit()
    {
        if (!isNotActionnable)
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(gameObject.GetComponent<Renderer>().material.color.r, gameObject.GetComponent<Renderer>().material.color.g, gameObject.GetComponent<Renderer>().material.color.b, 1f);
            isOver = false;
        }

    }

    private void OnMouseOver()
    {
        if (!isNotActionnable)
        {
            isOver = true;
        }

    }

    private void OnMouseUp()
    {
        hasBeenClick = true;
    }

    public void AddVoisins(GameObject SelectionTile)
    {
        voisins.Add(SelectionTile);
    }

    public void RemoveVoisins(GameObject SelectionTile)
    {
        voisins.Remove(SelectionTile);
    }

    public void IsPath()
    {
        if (!isNotActionnable)
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f, gameObject.GetComponent<Renderer>().material.color.a);
        }
    }

    public void IsNotPath()
    {
        if (!isNotActionnable)
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, gameObject.GetComponent<Renderer>().material.color.a);
        }

    }

    public void IsNotActionnable()
    {
        isNotActionnable = true;
    }

    public bool Actionnable()
    {
        return isNotActionnable;
    }

    public void Hide()
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(gameObject.GetComponent<Renderer>().material.color.r, gameObject.GetComponent<Renderer>().material.color.g, gameObject.GetComponent<Renderer>().material.color.b, 0f);
        isNotActionnable = true;
    }
}