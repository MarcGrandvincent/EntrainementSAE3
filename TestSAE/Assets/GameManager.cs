using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Networking.UnityWebRequest;

public class GameManager : MonoBehaviour
{

    Turn gameTurn = Turn.Player;
    PlayerState playerState = PlayerState.isWaiting;
    GameState gameState = GameState.Waiting;
    AnnouncementState announcementState = AnnouncementState.isCreating;


    private GameObject SelectedPlayable = null;
    private GameObject OveringTile = null;
    private bool isOvering = false;
    private bool hasBeenClear = true;

    public int XSize;
    public int YSize;
    public int NumberPlayableCharacters;
    public int NumberEnemies;

    private int numberMovement = 0;
    private int enemyMovedCount;

    public List<List<GameObject>> Tiles = new List<List<GameObject>>();
    private List<GameObject> PlayableCharacters = new List<GameObject>();
    private List<GameObject> Enemies = new List<GameObject>();
    private List<GameObject> SelectionTiles = new List<GameObject>();
    private List<GameObject> path = new List<GameObject>();
    private Dictionary<GameObject, int> distances = new Dictionary<GameObject, int>();


    public GameObject TilePrefab;
    public GameObject PlayableCharacterPreFab;
    public GameObject EnemyCharacterPrefab;
    public GameObject SelectionTile;
    public GameObject UI;
    public Camera MainCamera;


    private Bounds mapBounds;

    private Text GameAnnouncementText;

    private float time = 0;

    
    // Start is called before the first frame update
    void Start()
    {
        GameAnnouncementText = UI.transform.Find("GameAnnouncement").GetComponent<Text>();
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
        if (gameState == GameState.TurnedChanged)
        {
            if (announcementState == AnnouncementState.isCreating)
            {
                GameAnnouncementText.fontSize = 350;
                GameAnnouncementText.transform.localScale = new Vector2(2,2);

                if (gameTurn == Turn.Enemy)
                {
                    GameAnnouncementText.text = "Enemies Turn";
                    GameAnnouncementText.color = new Color(1, 0, 0, 1);
                }
                else if (gameTurn == Turn.Player)
                {
                    GameAnnouncementText.text = "Player Turn";
                    GameAnnouncementText.color = new Color(0, 0, 1, 1);
                }
                GameAnnouncementText.GetComponent<TextShakeScript>().SetCoordinateShake(GameAnnouncementText.transform.position.x, GameAnnouncementText.transform.position.y);
                announcementState = AnnouncementState.isMovingStep1;
            }

            if (announcementState == AnnouncementState.isMovingStep1)
            {
                GameAnnouncementText.transform.localScale -= new Vector3(5f * Time.deltaTime,5f * Time.deltaTime,0);

                if (GameAnnouncementText.transform.localScale.x <= 0.5f)
                {
                    announcementState = AnnouncementState.isMovingStep2;
                    MainCamera.GetComponent<CameraScript>().Shake(0.01f, 0.2f);
                    GameAnnouncementText.GetComponent<TextShakeScript>().Shake(10f, 0.3f);
                }
            }

            if (announcementState == AnnouncementState.isMovingStep2)
            {
                time += Time.deltaTime;

                if (time > 2f)
                {
                    announcementState = AnnouncementState.isFinished;
                    time = 0;
                }
             }

            if (announcementState == AnnouncementState.isFinished)
            {
                GameAnnouncementText.transform.localScale -= new Vector3(5f * Time.deltaTime, 5f * Time.deltaTime, 0);

                if (GameAnnouncementText.transform.localScale.x <= 0)
                {
                    GameAnnouncementText.transform.localScale = new Vector3(0, 0, 0);
                    GameAnnouncementText.text = "";
                    gameState = GameState.Waiting;
                    announcementState = AnnouncementState.isCreating;

                    if (gameTurn == Turn.Player)
                    {
                        SetMovementMax();
                        GiveBackMovement(false);
                    }
                }
            }

        }


        if (gameTurn == Turn.Enemy)
        {
            if (gameState == GameState.Waiting)
            {
                gameState = GameState.CalculateAI;

            }

            if (gameState == GameState.CalculateAI)
            {

                string s = "";

                foreach (GameObject enemy in Enemies)
                {
                    s += enemy.name + " :\n";

                    GameObject target = null;
                    int minDistance = int.MaxValue;

                    CalculDistanceTile(Tiles[enemy.GetComponent<EnemyCharacterScript>().X][enemy.GetComponent<EnemyCharacterScript>().Y]);

                    for (int x = 0; x < XSize; x++)
                    {
                        for (int y = 0; y < YSize; y++)
                        {
                            if (Tiles[x][y].GetComponent<TileScript>().HasPlayer)
                            {
                                if (GetDistance(Tiles[x][y]) <= minDistance)
                                {
                                    minDistance = GetDistance(Tiles[x][y]);
                                    target = Tiles[x][y];
                                }
                            }
                        }
                    }


                    List<GameObject> path = CalculCheminTile(target);
                    
                    foreach (GameObject g in path)
                    {
                        s += g.name + "\n";
                    }
                    
                    enemy.GetComponent<EnemyCharacterScript>().Path = path;
                    enemy.GetComponent<EnemyCharacterScript>().PathSaved = path;

                }
                Debug.Log(s);
                enemyMovedCount = NumberEnemies;
                gameState = GameState.MoveAI;
            }

            if (gameState == GameState.MoveAI)
            {
                if (enemyMovedCount < NumberEnemies)
                {
                }
            }

        }

        if (gameTurn == Turn.Player)
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
                    CreateSelectionTile();
                    gameState = GameState.Waiting;
                }

                if (gameState == GameState.Waiting)
                {
                    isOvering = false;
                    List<GameObject> resultat;
                    foreach (GameObject currentTile in SelectionTiles)
                    {
                        if (currentTile.GetComponent<SelectionTileScript>().IsOver)
                        {
                            isOvering = true;
                            if (currentTile != OveringTile)
                            {
                                hasBeenClear = false;
                                OveringTile = currentTile;

                                // Création du chemin

                                // Initialisation Calcul Distance
                                CalculDistanceSelection(OveringTile);

                                // Initialisation Calcul Chemin
                                foreach (GameObject t in SelectionTiles)
                                {
                                    if (t.GetComponent<SelectionTileScript>().Actionnable())
                                    {

                                        resultat = CalculCheminSelection(t);
                                        path = resultat;
                                        numberMovement = resultat.Count;
                                        foreach (GameObject tile in SelectionTiles)
                                        {
                                            if (resultat.Contains(tile))
                                            {
                                                tile.GetComponent<SelectionTileScript>().IsPath();
                                            }
                                            else
                                            {
                                                tile.GetComponent<SelectionTileScript>().IsNotPath();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }


                    GameObject clickedTile = null;
                    foreach (GameObject selectionTile in SelectionTiles)
                    {
                        if (selectionTile.GetComponent<SelectionTileScript>().HasBeenClick)
                        {
                            clickedTile = selectionTile;
                        }
                    }


                    if (clickedTile != null)
                    {
                        MoveAndUpdatePlayable(clickedTile);
                    }


                    if (!isOvering && !hasBeenClear)
                    {
                        PutTilesWhite();
                    }


                    if (Input.GetKey(KeyCode.Escape) || Input.GetMouseButtonDown(1))
                    {
                        CancelMovement();
                    }


                }
            }

            if (gameState == GameState.IsMovingPlayable && playerState == PlayerState.WaitForAnimation)
            {
                AnimateMovementPlayable();
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
                
                int c = UnityEngine.Random.Range(1, 100);
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


        mapBounds = new Bounds(Tiles[0][0].transform.position,Vector3.zero);

        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {

                if (x + 1 <= XSize - 1)
                {
                    Tiles[x][y].GetComponent<TileScript>().AddVoisin(Tiles[x + 1][y]);
                }

                if (x - 1 >= 0)
                {
                    Tiles[x][y].GetComponent<TileScript>().AddVoisin(Tiles[x - 1][y]);
                }

                if (y + 1 <= YSize - 1)
                {
                    Tiles[x][y].GetComponent<TileScript>().AddVoisin(Tiles[x][y + 1]);
                }

                if (y - 1 >= 0)
                {
                    Tiles[x][y].GetComponent<TileScript>().AddVoisin(Tiles[x][y - 1]);
                }

                mapBounds.Encapsulate(Tiles[x][y].transform.position);
            }
        }

        MainCamera.GetComponent<CameraScript>().SetCoordinates(mapBounds.center);
        MainCamera.transform.position = new Vector3(mapBounds.center.x, mapBounds.center.y , -5);
        MainCamera.orthographicSize = mapBounds.size.x / 1.5f;
    }

    private void SetUpPlayer()
    {
        for (int i = 0; i < NumberPlayableCharacters; i++)
        {
            GameObject Playable = Instantiate(PlayableCharacterPreFab);
            PlayableCharacters.Add(Playable);
            Playable.name = "Playable " + i;
            bool valide = false;
            
            while (!valide)
            {
                int xposition = UnityEngine.Random.Range(0, XSize);
                int yposition = UnityEngine.Random.Range(0, YSize/5);

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
        for (int i = 0; i < NumberEnemies; i++)
        {
            GameObject enemy = Instantiate(EnemyCharacterPrefab);
            Enemies.Add(enemy);
            enemy.name = "Enemy " + i;
            bool valide = false;

            while (!valide)
            {
                int xposition = UnityEngine.Random.Range(0, XSize);
                int yposition = UnityEngine.Random.Range(YSize - YSize / 3, YSize);

                if (Tiles[xposition][yposition].GetComponent<TileScript>().CanWalk && !Tiles[xposition][yposition].GetComponent<TileScript>().HasEnemy)
                {
                    Tiles[xposition][yposition].GetComponent<TileScript>().HasEnemy = true;
                    enemy.transform.position = new Vector3(xposition, yposition, -1);
                    enemy.GetComponent<EnemyCharacterScript>().X = xposition;
                    enemy.GetComponent<EnemyCharacterScript>().Y = yposition;
                    valide = true;
                }
            }
        }
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


    /// <summary>
    /// Calcule de la distance depuis une tile de sélection
    /// </summary>
    /// <param name="tile"></param>
    private void CalculDistanceSelection(GameObject tile)
    {
        List<GameObject> aTraiter = new List<GameObject>();
        ResetDistance();
        aTraiter.Add(tile);
        SetDistance(tile, 0);

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
    }

    private List<GameObject> CalculCheminSelection(GameObject StartTile)
    {
        GameObject tileProcessing = StartTile;

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

        return resultat;
    }


    /// <summary>
    /// Calcule de la distance depuis une tile normal
    /// </summary>
    /// <param name="tile"></param>
    private void CalculDistanceTile(GameObject tile)
    {
        List<GameObject> aTraiter = new List<GameObject>();
        ResetDistance();
        aTraiter.Add(tile);
        SetDistance(tile, 0);

        while (aTraiter.Count > 0)
        {
            GameObject tileEnCours = aTraiter[0];
            aTraiter.RemoveAt(0);

            foreach (GameObject t in tileEnCours.GetComponent<TileScript>().Voisins)
            {
                if (GetDistance(t) == -1)
                {
                    SetDistance(t, GetDistance(tileEnCours) + 1);
                    aTraiter.Add(t);
                }
            }
        }
    }


    private List<GameObject> CalculCheminTile(GameObject StartTile)
    {
        GameObject tileProcessing = StartTile;

        List<GameObject> resultat = new List<GameObject>();

        if (GetDistance(tileProcessing) != 0)
        {
            while (GetDistance(tileProcessing) > 0)
            {
                GameObject PreviousTile = null;
                foreach (GameObject t in tileProcessing.GetComponent<TileScript>().Voisins)
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

        return resultat;
    }



    private void DestroySelectionTile()
    {
        for (int i = 0; i < SelectionTiles.Count; i++)
        {
            Destroy(SelectionTiles[i]);
        }
        SelectionTiles.Clear();
    }

    private void CreateSelectionTile()
    {
        int xposition = SelectedPlayable.GetComponent<PlayableCharacterScript>().X;
        int yposition = SelectedPlayable.GetComponent<PlayableCharacterScript>().Y;
        int movement = SelectedPlayable.GetComponent<PlayableCharacterScript>().Movement;

        // We create the point 0
        GameObject DefaultSelectedTile = Instantiate(SelectionTile);
        DefaultSelectedTile.transform.position = new Vector3(xposition, yposition, 0);
        DefaultSelectedTile.name = "SelectionTile " + xposition + " " + yposition;
        DefaultSelectedTile.GetComponent<SelectionTileScript>().X = xposition;
        DefaultSelectedTile.GetComponent<SelectionTileScript>().Y = yposition;
        DefaultSelectedTile.GetComponent<SelectionTileScript>().IsNotActionnable();
        SelectionTiles.Add(DefaultSelectedTile);


        for (int i = 0; i < movement; i++)
        {
            if (xposition <= XSize - 1 && xposition >= 0 && yposition + i + 1 <= YSize - 1 && yposition + i + 1 >= 0)
            {
                if (Tiles[xposition][yposition + i + 1].GetComponent<TileScript>().CanWalk && !Tiles[xposition][yposition + i + 1].GetComponent<TileScript>().HasPlayer && !Tiles[xposition][yposition + i + 1].GetComponent<TileScript>().HasEnemy)
                {
                    GameObject SelectedTile = Instantiate(SelectionTile);
                    SelectedTile.transform.position = new Vector3(xposition, yposition + i + 1, -2);
                    SelectedTile.name = "SelectionTile " + xposition + " " + (yposition + i + 1);
                    SelectedTile.GetComponent<SelectionTileScript>().X = xposition;
                    SelectedTile.GetComponent<SelectionTileScript>().Y = yposition + i + 1;
                    SelectionTiles.Add(SelectedTile);

                }
            }
        }


        for (int i = 0; i < movement; i++)
        {
            if (xposition <= XSize - 1 && xposition >= 0 && yposition - i - 1 <= YSize - 1 && yposition - i - 1 >= 0)
            {
                if (Tiles[xposition][yposition - i - 1].GetComponent<TileScript>().CanWalk && !Tiles[xposition][yposition - i - 1].GetComponent<TileScript>().HasPlayer && !Tiles[xposition][yposition - i - 1].GetComponent<TileScript>().HasEnemy)
                {
                    GameObject SelectedTile2 = Instantiate(SelectionTile);
                    SelectedTile2.transform.position = new Vector3(xposition, yposition - i - 1, -2);
                    SelectedTile2.name = "SelectionTile " + xposition + " " + (yposition - i - 1);
                    SelectedTile2.GetComponent<SelectionTileScript>().X = xposition;
                    SelectedTile2.GetComponent<SelectionTileScript>().Y = yposition - i - 1;
                    SelectionTiles.Add(SelectedTile2);

                }
            }
        }


        for (int i = 0; i < movement; i++)
        {
            //Horizontal Haut
            for (int j = 0; j < movement - i; j++)
            {
                if (xposition + j + 1 <= XSize - 1 && xposition + j + 1 >= 0 && yposition + i <= YSize - 1 && yposition + i >= 0)
                {
                    if (Tiles[xposition + j + 1][yposition + i].GetComponent<TileScript>().CanWalk && !Tiles[xposition + j + 1][yposition + i].GetComponent<TileScript>().HasPlayer && !Tiles[xposition + j + 1][yposition + i].GetComponent<TileScript>().HasEnemy)
                    {
                        GameObject SelectedTile3 = Instantiate(SelectionTile);
                        SelectedTile3.transform.position = new Vector3(xposition + j + 1, yposition + i, -2);
                        SelectedTile3.name = "SelectionTile " + (xposition + j + 1) + " " + (yposition + i);
                        SelectedTile3.GetComponent<SelectionTileScript>().X = xposition + j + 1;
                        SelectedTile3.GetComponent<SelectionTileScript>().Y = yposition + i;
                        SelectionTiles.Add(SelectedTile3);
                    }
                }
            }

            for (int j = 0; j < movement - i; j++)
            {
                if (xposition - j - 1 <= XSize - 1 && xposition - j - 1 >= 0 && yposition + i <= YSize - 1 && yposition + i >= 0)
                {
                    if (Tiles[xposition - j - 1][yposition + i].GetComponent<TileScript>().CanWalk && !Tiles[xposition - j - 1][yposition + i].GetComponent<TileScript>().HasPlayer && !Tiles[xposition - j - 1][yposition + i].GetComponent<TileScript>().HasEnemy)
                    {
                        GameObject SelectedTile4 = Instantiate(SelectionTile);
                        SelectedTile4.transform.position = new Vector3(xposition - j - 1, yposition + i, -2);
                        SelectedTile4.name = "SelectionTile " + (xposition - j - 1) + " " + (yposition + i);
                        SelectedTile4.GetComponent<SelectionTileScript>().X = xposition - j - 1;
                        SelectedTile4.GetComponent<SelectionTileScript>().Y = yposition + i;
                        SelectionTiles.Add(SelectedTile4);
                    }
                }
            }

            // Horizontal Bas
            for (int j = 0; j < movement - i - 1; j++)
            {
                if (xposition + j + 1 <= XSize - 1 && xposition + j + 1 >= 0 && yposition - i - 1 <= YSize - 1 && yposition - i - 1 >= 0)
                {
                    if (Tiles[xposition + j + 1][yposition - i - 1].GetComponent<TileScript>().CanWalk && !Tiles[xposition + j + 1][yposition - i - 1].GetComponent<TileScript>().HasPlayer && !Tiles[xposition + j + 1][yposition - i - 1].GetComponent<TileScript>().HasEnemy)
                    {
                        GameObject SelectedTile5 = Instantiate(SelectionTile);
                        SelectedTile5.transform.position = new Vector3(xposition + j + 1, yposition - i - 1, -2);
                        SelectedTile5.name = "SelectionTile " + (xposition + j + 1) + " " + (yposition - i - 1);
                        SelectedTile5.GetComponent<SelectionTileScript>().X = xposition + j + 1;
                        SelectedTile5.GetComponent<SelectionTileScript>().Y = yposition - i - 1;
                        SelectionTiles.Add(SelectedTile5);
                    }
                }
            }

            for (int j = 0; j < movement - i - 1; j++)
            {
                if (xposition - j - 1 <= XSize - 1 && xposition - j - 1 >= 0 && yposition - i - 1 <= YSize - 1 && yposition - i - 1 >= 0)
                {
                    if (Tiles[xposition - j - 1][yposition - i - 1].GetComponent<TileScript>().CanWalk && !Tiles[xposition - j - 1][yposition - i - 1].GetComponent<TileScript>().HasPlayer && !Tiles[xposition - j - 1][yposition - i - 1].GetComponent<TileScript>().HasEnemy)
                    {
                        GameObject SelectedTile6 = Instantiate(SelectionTile);
                        SelectedTile6.transform.position = new Vector3(xposition - j - 1, yposition - i - 1, -2);
                        SelectedTile6.name = "SelectionTile " + (xposition - j - 1) + " " + (yposition - i - 1);
                        SelectedTile6.GetComponent<SelectionTileScript>().X = xposition - j - 1;
                        SelectedTile6.GetComponent<SelectionTileScript>().Y = yposition - i - 1;
                        SelectionTiles.Add(SelectedTile6);
                    }
                }
            }
        }

        // On récupére les voisins
        foreach (GameObject currentTile in SelectionTiles)
        {
            foreach (GameObject tile in SelectionTiles)
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



        // On supprime les chemins qui ne sont pas accessible ou qui ont plus de mouvement que movement 

        List<GameObject> tileToDestroy = new List<GameObject>();

        foreach (GameObject tile in SelectionTiles)
        {
            if (!tile.GetComponent<SelectionTileScript>().Actionnable())
            {
                // Initialisation Calcul Distance
                CalculDistanceSelection(tile);

                // Initialisation Calcul Chemin
                foreach (GameObject Arrivee in SelectionTiles)
                {
                    if (Arrivee.GetComponent<SelectionTileScript>().Actionnable())
                    {
                        List<GameObject> chemin = CalculCheminSelection(Arrivee);

                        if (chemin.Count > 0)
                        {
                            if (!chemin[0].GetComponent<SelectionTileScript>().Voisins.Contains(Arrivee))
                            {
                                tileToDestroy.Add(tile);
                            }
                            else if (chemin.Count > movement)
                            {
                                tileToDestroy.Add(tile);
                            }
                        }
                        else
                        {
                            tileToDestroy.Add(tile);
                        }
                    }
                }
            }
        }



        // Destruction des tiles inutiles
        for (int i = 0; i < tileToDestroy.Count; i++)
        {

            SelectionTiles.Remove(tileToDestroy[i]);

            foreach (GameObject t in SelectionTiles)
            {
                t.GetComponent<SelectionTileScript>().RemoveVoisins(tileToDestroy[i]);
            }
            Destroy(tileToDestroy[i]);
        }
        tileToDestroy.Clear();
    }


    private void PutTilesWhite()
    {
        foreach (GameObject t in SelectionTiles)
        {
            t.GetComponent<SelectionTileScript>().IsNotPath();
        }
        hasBeenClear = true;
        OveringTile = null;
    }

    private void MoveAndUpdatePlayable(GameObject clickedTile)
    {
        Tiles[SelectedPlayable.GetComponent<PlayableCharacterScript>().X][SelectedPlayable.GetComponent<PlayableCharacterScript>().Y].GetComponent<TileScript>().HasPlayer = false;


        Tiles[clickedTile.GetComponent<SelectionTileScript>().X][clickedTile.GetComponent<SelectionTileScript>().Y].GetComponent<TileScript>().HasPlayer = true;
        
        SelectedPlayable.GetComponent<PlayableCharacterScript>().IsNotClicked();


        SelectedPlayable.GetComponent<PlayableCharacterScript>().Movement -= numberMovement;

        if (SelectedPlayable.GetComponent<PlayableCharacterScript>().Movement == 0)
        {
            SelectedPlayable.GetComponent<PlayableCharacterScript>().CanInteract = false;
        }
        else
        {
            SelectedPlayable.GetComponent<PlayableCharacterScript>().CanInteract = true;
        }
        //DestroySelectionTile();
        HideSelectionTile();
        path.Reverse();
        gameState = GameState.IsMovingPlayable;
        playerState = PlayerState.WaitForAnimation;
    }

    private void AnimateMovementPlayable()
    {
        if (path.Count > 0)
        {
            if (path[path.Count - 1].GetComponent<SelectionTileScript>().X > SelectedPlayable.GetComponent<PlayableCharacterScript>().X)
            {
                if (SelectedPlayable.transform.position.x < path[path.Count - 1].GetComponent<SelectionTileScript>().X)
                {
                    SelectedPlayable.transform.Translate(Vector3.right * 10f * Time.deltaTime);
                }
                else
                {
                    SelectedPlayable.GetComponent<PlayableCharacterScript>().X = path[path.Count - 1].GetComponent<SelectionTileScript>().X;
                    SelectedPlayable.GetComponent<PlayableCharacterScript>().Y = path[path.Count - 1].GetComponent<SelectionTileScript>().Y;
                    SelectedPlayable.transform.position = new Vector3(path[path.Count - 1].GetComponent<SelectionTileScript>().X, path[path.Count - 1].GetComponent<SelectionTileScript>().Y, -1);
                    path.RemoveAt(path.Count - 1);
                }
            }

            else if (path[path.Count - 1].GetComponent<SelectionTileScript>().X < SelectedPlayable.GetComponent<PlayableCharacterScript>().X)
            {
                if (SelectedPlayable.transform.position.x > path[path.Count - 1].GetComponent<SelectionTileScript>().X)
                {
                    SelectedPlayable.transform.Translate(Vector3.left * 10f * Time.deltaTime);
                }
                else
                {
                    SelectedPlayable.GetComponent<PlayableCharacterScript>().X = path[path.Count - 1].GetComponent<SelectionTileScript>().X;
                    SelectedPlayable.GetComponent<PlayableCharacterScript>().Y = path[path.Count - 1].GetComponent<SelectionTileScript>().Y;
                    SelectedPlayable.transform.position = new Vector3(path[path.Count - 1].GetComponent<SelectionTileScript>().X, path[path.Count - 1].GetComponent<SelectionTileScript>().Y, -1);
                    path.RemoveAt(path.Count - 1);
                }
            }

            else if (path[path.Count - 1].GetComponent<SelectionTileScript>().Y > SelectedPlayable.GetComponent<PlayableCharacterScript>().Y)
            {
                if (SelectedPlayable.transform.position.y < path[path.Count - 1].GetComponent<SelectionTileScript>().Y)
                {
                    SelectedPlayable.transform.Translate(Vector3.up * 10f * Time.deltaTime);
                }
                else
                {
                    SelectedPlayable.GetComponent<PlayableCharacterScript>().X = path[path.Count - 1].GetComponent<SelectionTileScript>().X;
                    SelectedPlayable.GetComponent<PlayableCharacterScript>().Y = path[path.Count - 1].GetComponent<SelectionTileScript>().Y;
                    SelectedPlayable.transform.position = new Vector3(path[path.Count - 1].GetComponent<SelectionTileScript>().X, path[path.Count - 1].GetComponent<SelectionTileScript>().Y, -1);
                    path.RemoveAt(path.Count - 1);
                }
            }

            else if (path[path.Count - 1].GetComponent<SelectionTileScript>().Y < SelectedPlayable.GetComponent<PlayableCharacterScript>().Y)
            {
                if (SelectedPlayable.transform.position.y > path[path.Count - 1].GetComponent<SelectionTileScript>().Y)
                {
                    SelectedPlayable.transform.Translate(Vector3.down * 10f * Time.deltaTime);
                }
                else
                {
                    SelectedPlayable.GetComponent<PlayableCharacterScript>().X = path[path.Count - 1].GetComponent<SelectionTileScript>().X;
                    SelectedPlayable.GetComponent<PlayableCharacterScript>().Y = path[path.Count - 1].GetComponent<SelectionTileScript>().Y;
                    SelectedPlayable.transform.position = new Vector3(path[path.Count - 1].GetComponent<SelectionTileScript>().X, path[path.Count - 1].GetComponent<SelectionTileScript>().Y, -1);
                    path.RemoveAt(path.Count - 1);
                }
            }

        }
        else
        {
            DestroySelectionTile();
            if (!HasMovement())
            {
                gameTurn = Turn.Enemy;
                gameState = GameState.TurnedChanged;
                playerState = PlayerState.isWaiting;
            }
            else
            {
                GiveBackMovement(true);
                gameState = GameState.Waiting;
                playerState = PlayerState.isWaiting;
            }
        }
    }

    private void CancelMovement()
    {
        DestroySelectionTile();
        SelectedPlayable.GetComponent<PlayableCharacterScript>().IsNotClicked();
        foreach (GameObject t in PlayableCharacters)
        {
            t.GetComponent<PlayableCharacterScript>().CanInteract = true;
        }
        playerState = PlayerState.isWaiting;
        gameState = GameState.Waiting;
    }

    private void HideSelectionTile()
    {
        foreach (GameObject tile in SelectionTiles)
        {
            tile.GetComponent<SelectionTileScript>().Hide();
        }
    }

    private void GiveBackMovement(bool movement)
    {
        if (movement)
        {
            foreach (GameObject p in PlayableCharacters)
            {
                if (!p.GetComponent<PlayableCharacterScript>().HasNoMovement)
                {
                    p.GetComponent<PlayableCharacterScript>().CanInteract = true;
                }
            }
        }
        else
        {
            foreach (GameObject p in PlayableCharacters)
            {
                p.GetComponent<PlayableCharacterScript>().isInteractive();
            }
        }

    }

    private bool HasMovement()
    {
        bool isTrue = false;

        foreach (GameObject p in PlayableCharacters)
        {
            if (!p.GetComponent<PlayableCharacterScript>().HasNoMovement)
            {
                isTrue = true;
            }
        }

        return isTrue;
    }

    private void SetMovementMax()
    {
        foreach (GameObject p in PlayableCharacters)
        {
            p.GetComponent<PlayableCharacterScript>().Movement = p.GetComponent<PlayableCharacterScript>().MaxMovement;
        }
    }
}


public enum GameState
{
    Waiting, 
    IsCreatingSelectionTile,
    IsMovingPlayable,
    TurnedChanged,
    CalculateAI,
    MoveAI
}
public enum PlayerState
{
    isWaiting,
    WaitForAnimation,
    isMoving
}

public enum AnnouncementState
{
    isCreating,
    isMovingStep1,
    isMovingStep2,
    isFinished
}

public enum Turn
{
    Player,
    Enemy
}
