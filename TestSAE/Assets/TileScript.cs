using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{

    private Type type = Type.GROUND;

    private bool canWalk = false;
    public bool CanWalk { get => this.canWalk; }
    
    private bool canInteract = true;
    public bool CanInteract { get => canInteract; set => canInteract = value; }

    private bool hasPlayer = false;
    public bool HasPlayer { get => hasPlayer; set => hasPlayer = value; }

    private bool hasEnemy = false;
    public bool HasEnemy { get => hasEnemy; set => hasEnemy = value; }

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

    public void ChangeType(Type t)
    {
        this.type = t;
        switch (this.type)
        {
            case Type.GROUND:
                {
                    gameObject.GetComponent<Renderer>().material.color = new Color(39/255f, 147/255f, 12/255f);
                    canWalk = true;
                }
                break;
            case Type.TREE:
                {
                    gameObject.GetComponent<Renderer>().material.color = new Color(184/255f, 122/255f, 0/255f);
                    canWalk = false;
                }
                break;
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

    public void AddVoisin(GameObject voisin)
    {
        voisins.Add(voisin);
    }
}

public enum Type
{
    GROUND,
    TREE
}
