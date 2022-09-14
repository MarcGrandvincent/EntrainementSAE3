using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    Turn GameTurn = Turn.Player;
    PlayerState playerState = PlayerState.isWaiting;
    GameState gameState = GameState.Waiting;


    private GameObject SelectedPlayable = null;
    private GameObject OveringTile = null;

    public int XSize;
    public int YSize;
    public int NumberPlayableCharacters;
    public int NumberEnemies;

    public List<List<GameObject>> Tiles = new List<List<GameObject>>();
    private List<GameObject> PlayableCharacters = new List<GameObject>();
    private List<GameObject> SelectionTiles = new List<GameObject>();
    private Dictionary<GameObject, int> distances = new Dictionary<GameObject, int>();


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

                // We create the point 0
                GameObject DefaultSelectedTile = Instantiate(SelectionTile);
                DefaultSelectedTile.transform.position = new Vector2(xposition, yposition);
                DefaultSelectedTile.name = "SelectionTile " + xposition + " " + yposition;
                DefaultSelectedTile.GetComponent<SelectionTileScript>().X = xposition;
                DefaultSelectedTile.GetComponent<SelectionTileScript>().Y = yposition;
                DefaultSelectedTile.GetComponent<SelectionTileScript>().IsNotActionnable();
                SelectionTiles.Add(DefaultSelectedTile);

                for (int i = 0; i < movement; i++)
                {
                    // Vertical
                    if (xposition <= XSize - 1 && xposition >= 0 && yposition + i + 1 <= YSize - 1 && yposition + i + 1 >= 0)
                    {
                        if (Tiles[xposition][yposition + i + 1].GetComponent<TileScript>().CanWalk && !Tiles[xposition][yposition + i + 1].GetComponent<TileScript>().HasPlayer)
                        {
                            GameObject SelectedTile = Instantiate(SelectionTile);
                            SelectedTile.transform.position = new Vector3(xposition, yposition + i + 1, -2);
                            SelectedTile.name = "SelectionTile " + xposition + " " + (yposition + i + 1);
                            SelectedTile.GetComponent<SelectionTileScript>().X = xposition;
                            SelectedTile.GetComponent<SelectionTileScript>().Y = yposition + i + 1;
                            SelectionTiles.Add(SelectedTile);
                            
                        }

                    }

                    if (xposition <= XSize - 1 && xposition >= 0 && yposition - i - 1 <= YSize - 1 && yposition - i - 1 >= 0)
                    {
                        if (Tiles[xposition][yposition - i - 1].GetComponent<TileScript>().CanWalk && !Tiles[xposition][yposition - i - 1].GetComponent<TileScript>().HasPlayer)
                        {
                            GameObject SelectedTile2 = Instantiate(SelectionTile);
                            SelectedTile2.transform.position = new Vector3(xposition, yposition - i - 1, -2);
                            SelectedTile2.name = "SelectionTile " + xposition + " " + (yposition - i - 1);
                            SelectedTile2.GetComponent<SelectionTileScript>().X = xposition;
                            SelectedTile2.GetComponent<SelectionTileScript>().Y = yposition - i - 1;
                            SelectionTiles.Add(SelectedTile2);

                        }

                    }


                    //Horizontal Haut
                    for (int j = 0; j < movement - i; j++)
                    {
                        if (xposition + j + 1 <= XSize - 1 && xposition + j + 1 >= 0 && yposition + i <= YSize - 1 && yposition + i >= 0)
                        {
                            if (Tiles[xposition + j + 1][yposition + i].GetComponent<TileScript>().CanWalk && !Tiles[xposition + j + 1][yposition + i].GetComponent<TileScript>().HasPlayer)
                            {
                                GameObject SelectedTile3 = Instantiate(SelectionTile);
                                SelectedTile3.transform.position = new Vector3(xposition + j + 1, yposition + i, -2);
                                SelectedTile3.name = "SelectionTile " + (xposition + j + 1) + " " + (yposition + i);
                                SelectedTile3.GetComponent<SelectionTileScript>().X = xposition + j + 1;
                                SelectedTile3.GetComponent<SelectionTileScript>().Y = yposition + i;
                                SelectionTiles.Add(SelectedTile3);
                            }
                            else
                            {
                                j += 1;
                            }
                        }
                    }

                    for (int j = 0; j < movement - i; j++)
                    {
                        if (xposition - j - 1 <= XSize - 1 && xposition - j - 1 >= 0 && yposition + i <= YSize - 1 && yposition + i >= 0)
                        {
                            if (Tiles[xposition - j - 1][yposition + i].GetComponent<TileScript>().CanWalk && !Tiles[xposition - j - 1][yposition + i].GetComponent<TileScript>().HasPlayer)
                            {
                                GameObject SelectedTile4 = Instantiate(SelectionTile);
                                SelectedTile4.transform.position = new Vector3(xposition - j - 1, yposition + i, -2);
                                SelectedTile4.name = "SelectionTile " + (xposition - j - 1) + " " + (yposition + i);
                                SelectedTile4.GetComponent<SelectionTileScript>().X = xposition - j - 1;
                                SelectedTile4.GetComponent<SelectionTileScript>().Y = yposition + i;
                                SelectionTiles.Add(SelectedTile4);
                            }
                            else
                            {
                                j += 1;
                            }
                        }
                    }

                    // Horizontal Bas
                    for (int j = 0; j < movement - i - 1; j++)
                    {
                        if (xposition + j + 1 <= XSize - 1 && xposition + j + 1 >= 0 && yposition - i - 1 <= YSize - 1 && yposition - i - 1 >= 0)
                        {
                            if (Tiles[xposition + j + 1][yposition - i - 1].GetComponent<TileScript>().CanWalk && !Tiles[xposition + j + 1][yposition - i - 1].GetComponent<TileScript>().HasPlayer)
                            {
                                GameObject SelectedTile5 = Instantiate(SelectionTile);
                                SelectedTile5.transform.position = new Vector3(xposition + j + 1, yposition - i - 1, -2);
                                SelectedTile5.name = "SelectionTile " + (xposition + j + 1) + " " + (yposition - i - 1);
                                SelectedTile5.GetComponent<SelectionTileScript>().X = xposition + j + 1;
                                SelectedTile5.GetComponent<SelectionTileScript>().Y = yposition - i - 1;
                                SelectionTiles.Add(SelectedTile5);
                            }
                            else
                            {
                                j += 1;
                            }
                        }
                    }

                    for (int j = 0; j < movement - i - 1; j++)
                    {
                        if (xposition - j - 1 <= XSize - 1 && xposition - j - 1 >= 0 && yposition - i - 1 <= YSize - 1 && yposition - i - 1 >= 0)
                        {
                            if (Tiles[xposition - j - 1][yposition - i - 1].GetComponent<TileScript>().CanWalk && !Tiles[xposition - j - 1][yposition - i - 1].GetComponent<TileScript>().HasPlayer)
                            {
                                GameObject SelectedTile6 = Instantiate(SelectionTile);
                                SelectedTile6.transform.position = new Vector3(xposition - j - 1, yposition - i - 1, -2);
                                SelectedTile6.name = "SelectionTile " + (xposition - j - 1) + " " + (yposition - i - 1);
                                SelectedTile6.GetComponent<SelectionTileScript>().X = xposition - j - 1;
                                SelectedTile6.GetComponent<SelectionTileScript>().Y = yposition - i - 1;
                                SelectionTiles.Add(SelectedTile6);
                            }
                            else
                            {
                                j += 1;
                            }
                        }
                    }
                }

                // On récupére les voisins
                foreach(GameObject currentTile in SelectionTiles)
                {
                    foreach(GameObject tile in SelectionTiles)
                    {
                        if (tile.GetComponent<SelectionTileScript>().X == currentTile.GetComponent<SelectionTileScript>().X + 1 &&
                            tile.GetComponent<SelectionTileScript>().Y == currentTile.GetComponent<SelectionTileScript>().Y ||
                            tile.GetComponent<SelectionTileScript>().Y == currentTile.GetComponent<SelectionTileScript>().Y + 1 &&
                            tile.GetComponent<SelectionTileScript>().X == currentTile.GetComponent<SelectionTileScript>().X ||
                            tile.GetComponent<SelectionTileScript>().X == currentTile.GetComponent<SelectionTileScript>().X - 1 &&
                            tile.GetComponent<SelectionTileScript>().Y == currentTile.GetComponent<SelectionTileScript>().Y ||
                            tile.GetComponent<SelectionTileScript>().Y == currentTile.GetComponent<SelectionTileScript>().Y - 1 &&
                            tile.GetComponent<SelectionTileScript>().X == currentTile.GetComponent<SelectionTileScript>().X)
                        {
                            currentTile.GetComponent<SelectionTileScript>().AddVoisins(tile);
                        }
                    }
                }


                // On vérifie si des cases n'ont pas de voisins
                foreach (GameObject currentTile in SelectionTiles)
                {
                    if (currentTile.GetComponent<SelectionTileScript>().Voisins.Count == 0)
                    {
                        SelectionTiles.Remove(currentTile);
                        Destroy(currentTile);
                    }
                }
                gameState = GameState.Waiting;
            }

            if (gameState == GameState.Waiting)
            {
                foreach (GameObject currentTile in SelectionTiles)
                {
                    if (currentTile.GetComponent<SelectionTileScript>().IsOver && currentTile != OveringTile)
                    {
                        OveringTile = currentTile;
                        
                        // Initialisation Calcul Distance
                        List<GameObject> aTraiter = new List<GameObject>();
                        ResetDistance();
                        aTraiter.Add(currentTile);
                        SetDistance(currentTile, 0);

                        while (aTraiter.Count > 0)
                        {
                            GameObject tileEnCours = aTraiter[0];
                            aTraiter.RemoveAt(0);

                            foreach (GameObject t in tileEnCours.GetComponent<SelectionTileScript>().Voisins)
                            {
                                if (GetDistance(t) == -1)
                                {
                                    SetDistance(t, GetDistance(tileEnCours) + 1);
                                    aTraiter.Add(t);
                                }
                            }
                        }


                        // Initialisation Calcul Chemin
                        GameObject tileProcessing = null;
                        foreach (GameObject t in SelectionTiles)
                        {
                            if (t.GetComponent<SelectionTileScript>().Actionnable())
                            {
                                tileProcessing = t;
                            }
                        }

                        List<GameObject> resultat = new List<GameObject>();

                        if (GetDistance(tileProcessing) != 0)
                        {
                            while (GetDistance(tileProcessing) > 0)
                            {
                                GameObject PreviousTile = null;
                                foreach (GameObject t in tileProcessing.GetComponent<SelectionTileScript>().Voisins)
                                {
                                    if (GetDistance(t) == GetDistance(tileProcessing) - 1)
                                    {
                                        PreviousTile = t;
                                    }
                                }
                                resultat.Add(PreviousTile);
                                tileProcessing = PreviousTile;
                            }
                        }

                        string s = "";

                        foreach (GameObject t in resultat)
                        {
                            s += t.name + "\n";
                        }

                        Debug.Log(s);

                        foreach (GameObject t in SelectionTiles)
                        {
                            if (resultat.Contains(t))
                            {
                                t.GetComponent<SelectionTileScript>().IsPath();
                            }
                            else
                            {
                                t.GetComponent<SelectionTileScript>().IsNotPath();
                            }
                        }
                    }
                }
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
                if (c <= 10)
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
        GameObject Playable2 = Instantiate(PlayableCharacterPreFab);
        Playable2.transform.position = new Vector3(XSize/2, YSize/2, -1);
        Playable2.GetComponent<PlayableCharacterScript>().X = XSize/2;
        Playable2.GetComponent<PlayableCharacterScript>().Y = YSize/2;
        PlayableCharacters.Add(Playable2);
        Playable2.name = "Playable " + 4;
    }

    private void SetUpEnemy()
    {

    }

    private void SetDistance(GameObject Tile, int distance)
    {
        distances.Add(Tile, distance);
    }

    private int GetDistance(GameObject Tile)
    {
        if (!distances.ContainsKey(Tile))
        {
            return -1;
        }
        else
        {
            return distances[Tile];
        }
    }

    private void ResetDistance()
    {
        distances.Clear();
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
