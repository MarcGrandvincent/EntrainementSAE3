using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacterScript : MonoBehaviour
{

    private int x;
    private int y;
    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }


    private List<GameObject> path;
    public List<GameObject> Path { get => path; set => path = value; }

    private List<GameObject> pathSaved;
    public List<GameObject> PathSaved { get => pathSaved; set => pathSaved = value; }



    private int movement;
    public int Movement { get => movement; set => movement = value; }

    private int maxMovement = 3;
    public int MaxMovement { get => maxMovement; }

    // Start is called before the first frame update
    void Start()
    {
        movement = maxMovement;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
