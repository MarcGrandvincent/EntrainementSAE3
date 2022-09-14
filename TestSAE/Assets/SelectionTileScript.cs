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
       gameObject.GetComponent<Renderer>().material.color = new Color(gameObject.GetComponent<Renderer>().material.color.r, gameObject.GetComponent<Renderer>().material.color.g, gameObject.GetComponent<Renderer>().material.color.b, 0.5f);
    }

    private void OnMouseExit()
    {
       gameObject.GetComponent<Renderer>().material.color = new Color(gameObject.GetComponent<Renderer>().material.color.r, gameObject.GetComponent<Renderer>().material.color.g, gameObject.GetComponent<Renderer>().material.color.b, 1f);
    }

    public void AddVoisins(GameObject SelectionTile)
    {
        voisins.Add(SelectionTile);
    }

    public void RemoveVoisins(GameObject SelectionTile)
    {
        voisins.Remove(SelectionTile);
    }
}
