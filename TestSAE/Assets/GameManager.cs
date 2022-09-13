using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{

    Turn GameTurn = Turn.Player;
    PlayerState playerState = PlayerState.isWaiting;
    GameState gameState = GameState.Waiting;


    private GameObject SelectedPlayable = null;

    public int XSize;
    public int YSize;
    public int NumberPlayableCharacters;
    public int NumberEnemies;

    public List<List<GameObject>> Tiles = new List<List<GameObject>>();
    private List<GameObject> PlayableCharacters = new List<GameObject>();
    private List<List<GameObject>> SelectionTiles = new List<List<GameObject>>();

    public GameObject TilePrefab;
    public GameObject PlayableCharacterPreFab;
    public GameObject SelectionTile;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < YSize; i++)
        {
            Tiles.Add(new List<GameObject>());
        }
        for (int i = 0; i < YSize; i++)
        {
            SelectionTiles.Add(new List<GameObject>());
        }
        SetUpTiles();
        SetUpPlayer();
        SetUpEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState == PlayerState.isWaiting)
        {
            foreach (GameObject playable in PlayableCharacters)
            {
                if (playable.GetComponent<PlayableCharacterScript>().HasBeenClicked)
                {
                    SelectedPlayable = playable;
                    for (int x = 0; x < XSize; x++)
                    {
                        for (int y = 0; y < YSize; y++)
                        {
                            Tiles[x][y].GetComponent<TileScript>().CanInteract = false;
                        }
                    }
                    foreach (GameObject p in PlayableCharacters)
                    {
                        p.GetComponent<PlayableCharacterScript>().CanInteract = false;
                    }
                    playerState = PlayerState.isMoving;
                    gameState = GameState.IsCreatingSelectionTile;
                }
            }
        }

        if (playerState == PlayerState.isMoving)
        {
            if (gameState == GameState.IsCreatingSelectionTile)
            {
                int xposition = SelectedPlayable.GetComponent<PlayableCharacterScript>().X;
                int yposition = SelectedPlayable.GetComponent<PlayableCharacterScript>().Y;
                int movement = SelectedPlayable.GetComponent<PlayableCharacterScript>().Movement;

                for (int i = 0; i < movement; i++)
                {


                    // Colonne Haut 
                    if (xposition <= XSize - 1 && xposition >= 0 && yposition + i + 1 <= YSize - 1 && yposition + i + 1 >= 0)
                    {
                        if (Tiles[xposition][yposition + i + 1].GetComponent<TileScript>().CanWalk && !Tiles[xposition][yposition + i + 1].GetComponent<TileScript>().HasPlayer)
                        {
                            GameObject SelectedTile = Instantiate(SelectionTile);
                            SelectedTile.transform.position = new Vector3(xposition, yposition + i + 1, -1);
                        }
                    }

                    // Côté Haut
                    for (int j = 0; j < movement - 1 - i; j++)
                    {
                        if (xposition + j + 1 <= XSize - 1 && xposition + j + 1 >= 0 && yposition + i + 1 <= YSize - 1 && yposition + i + 1 >= 0)
                        {
                            if (Tiles[xposition + j + 1][yposition + i + 1].GetComponent<TileScript>().CanWalk && !Tiles[xposition + j + 1][yposition + i + 1].GetComponent<TileScript>().HasPlayer)
                            {
                                GameObject SelectedTile2 = Instantiate(SelectionTile);
                                SelectedTile2.transform.position = new Vector3(xposition + j + 1, yposition + i + 1, -1);
                            }
                        }
                        
                        if (xposition - j - 1 <= XSize - 1 && xposition - j - 1 >= 0 && yposition + i + 1 <= YSize && yposition + i + 1 >= 0)
                        {
                            if (Tiles[xposition - j - 1][yposition + i + 1].GetComponent<TileScript>().CanWalk && !Tiles[xposition - j - 1][yposition + i + 1].GetComponent<TileScript>().HasPlayer)
                            {
                                GameObject SelectedTile3 = Instantiate(SelectionTile);
                                SelectedTile3.transform.position = new Vector3(xposition - j - 1, yposition + i + 1, -1);
                            }
                        }

                    }

                    // Vertical droite 
                    if (xposition + i + 1 <= XSize - 1 && xposition + i + 1 >= 0 && yposition <= YSize -1 && yposition >= 0)
                    {
                        if (Tiles[xposition + i + 1][yposition].GetComponent<TileScript>().CanWalk && !Tiles[xposition + i + 1][yposition].GetComponent<TileScript>().HasPlayer)
                        {
                            GameObject SelectedTile4 = Instantiate(SelectionTile);
                            SelectedTile4.transform.position = new Vector3(xposition + i + 1, yposition, -1);
                        }
                    }

                    // Vertical gauche
                    if (xposition - i - 1 <= XSize - 1 && xposition - i - 1 <= 0 && yposition <= YSize - 1 && yposition >= 0)
                    {
                        if (Tiles[xposition - i - 1][yposition].GetComponent<TileScript>().CanWalk && !Tiles[xposition - i - 1][yposition].GetComponent<TileScript>().HasPlayer)
                        {
                            GameObject SelectedTile5 = Instantiate(SelectionTile);
                            SelectedTile5.transform.position = new Vector3(xposition - i - 1, yposition, -1);
                        }

                    }

                    // Colonne bas

                    if (xposition <= XSize - 1 && xposition >= 0 && yposition - i - 1 <= YSize - 1 && yposition - i - 1 >= 0)
                    {
                        if (Tiles[xposition][yposition - i - 1].GetComponent<TileScript>().CanWalk && !Tiles[xposition][yposition - i - 1].GetComponent<TileScript>().HasPlayer)
                        {
                            GameObject SelectedTile6 = Instantiate(SelectionTile);
                            SelectedTile6.transform.position = new Vector3(xposition, yposition - i - 1, -1);
                        }
                    }


                    // Côté, bas
                    for (int j = 0; j < movement - 1 - i; j++)
                    {
                        if (xposition + j + 1 <= XSize - 1 && xposition + j + 1 >= 0 && yposition - i - 1 <= YSize - 1 && yposition - i - 1 >= 0)
                        {
                            if (Tiles[xposition + j + 1][yposition - i - 1].GetComponent<TileScript>().CanWalk && !Tiles[xposition + j + 1][yposition - i - 1].GetComponent<TileScript>().HasPlayer)
                            {
                                GameObject SelectedTile7 = Instantiate(SelectionTile);
                                SelectedTile7.transform.position = new Vector3(xposition + j + 1, yposition - i - 1, -1);
                            }
                        }

                        if (xposition - j - 1 <= XSize - 1 && xposition - j - 1 >= 0 && yposition - i - 1 <= YSize - 1 && yposition - i - 1 >= 0)
                        {
                            if (Tiles[xposition - j - 1][yposition - i - 1].GetComponent<TileScript>().CanWalk && !Tiles[xposition - j - 1][yposition - i - 1].GetComponent<TileScript>().HasPlayer)
                            {
                                GameObject SelectedTile8 = Instantiate(SelectionTile);
                                SelectedTile8.transform.position = new Vector3(xposition - j - 1, yposition - i - 1, -1);
                            }
                        }

                    }
                }
                gameState = GameState.Waiting;

                /*
                for (int y = 0; y < i; y++)
                {
                    GameObject SelectedTile1 = Instantiate(SelectionTile);
                    SelectedTile1.transform.position = new Vector3(xposition + y + 1, yposition + i + 1, -1);
                }
                */
            }
        }
    }

    /// <summary>
    /// Generate the terrain
    /// </summary>
    private void SetUpTiles()
    {
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                GameObject tile = Instantiate(TilePrefab);
                tile.name = "Tile " + x + " " + y;
                tile.transform.position = new Vector2(x, y);
                int c = Random.Range(1, 100);
                if (c <= -1)
                {
                    tile.GetComponent<TileScript>().ChangeType(Type.TREE);
                }
                else
                {
                    tile.GetComponent<TileScript>().ChangeType(Type.GROUND);
                }

                Tiles[x].Add(tile);
            }
        }
    }

    private void SetUpPlayer()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject Playable = Instantiate(PlayableCharacterPreFab);
            PlayableCharacters.Add(Playable);
            Playable.name = "Playable " + i;
            bool valide = false;
            
            while (!valide)
            {
                int xposition = Random.Range(0, XSize);
                int yposition = Random.Range(0, YSize/5);

                if (Tiles[xposition][yposition].GetComponent<TileScript>().CanWalk && !Tiles[xposition][yposition].GetComponent<TileScript>().HasPlayer)
                {
                    Tiles[xposition][yposition].GetComponent<TileScript>().HasPlayer = true;
                    Playable.transform.position = new Vector3(xposition, yposition,-1);
                    Playable.GetComponent<PlayableCharacterScript>().X = xposition;
                    Playable.GetComponent<PlayableCharacterScript>().Y = yposition;
                    valide = true;
                }

            }
        }
    }

    private void SetUpEnemy()
    {

    }
}


public enum GameState
{
    Waiting, 
    IsCreatingSelectionTile
}
public enum PlayerState
{
    isWaiting,
    isMoving
}
public enum Turn
{
    Player,
    Enemy
}
