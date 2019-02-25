using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public GameObject tilePrefab;
    public Material selectMaterial;

    public GameObject[] playPieces;
    public Quaternion[] PieceRotations { get; private set; }


    [Range(2, 100)]
    [Tooltip("Number of columns")]
    public int cols = 8;

    [Range(2, 100)]
    [Tooltip("Number of rows")]
    public int rows = 8;

    [Range(2, 4)]
    public int players = 2;
    private int turn = 0;

    private float posX;
    private float posZ;
    private List<TileController> tiles;

    public TileController SelectedTile
    {
        get { return _selTile; }
        set
        {
            if (_selTile != null && tileMat != null)
            {
                _selTile.GetComponent<Renderer>().material = tileMat;
            }
            _selTile = value;
            if (_selTile != null)
            {
                tileMat = _selTile.GetComponent<Renderer>().material;
                _selTile.GetComponent<Renderer>().material = selectMaterial;
            }
            else
            {
                tileMat = null;
            }
        }
    }
    private TileController _selTile;
    private Material tileMat;

    private void Awake()
    {
        var boardGO = new GameObject("Board");
        tiles = new List<TileController>();
        posX = -0.5f * ((float)cols - 1);
        posZ = -0.5f * ((float)rows - 1);

        PieceRotations = new Quaternion[playPieces.Length];
        float[] rotY = new float[] { 45f, 225f, 135f, 315f };

        for (int i = 0; i < playPieces.Length; i++)
        {
            PieceRotations[i] = Quaternion.Euler(0, rotY[i % rotY.Length], 0);
        }

        for (int c = 0; c < cols; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                var go = Instantiate(tilePrefab, new Vector3(posX + c, 0f, posZ + r), Quaternion.identity);
                go.transform.SetParent(boardGO.transform);

                var tile = go.GetComponent<TileController>();
                tile.Board = this;
                tile.ColNr = c;
                tile.RowNr = r;
                if (c == 0 && r == 0)
                {
                    tile.PieceNr = 0;
                }
                else if (c == cols - 1 && r == rows - 1)
                {
                    tile.PieceNr = 1;
                }
                else if (c == 0 && r == rows - 1 && players > 2)
                {
                    tile.PieceNr = 2;
                }
                else if (c == cols - 1 && r == 0 && players > 3)
                {
                    tile.PieceNr = 3;
                }

                tiles.Add(tile);
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
            {
                var tile = hitInfo.collider.GetComponent<TileController>();
                if (tile != null)
                {
                    OnTileClicked(tile);
                }
            }
        }
    }

    private void OnTileClicked(TileController tile)
    {
        if (tile.PieceNr == turn)
        {
            SelectedTile = tile;
        }
        else if (SelectedTile != null && !tile.HasPiece
             && (tile.CanJump(SelectedTile) || tile.IsNeighbour(SelectedTile)))
        {
            tile.PieceNr = SelectedTile.PieceNr;
            foreach (var t in tiles)
            {
                if (t.PieceNr != tile.PieceNr && t.HasPiece && t.IsNeighbour(tile))
                    t.PieceNr = SelectedTile.PieceNr;
            }
            if (!tile.IsNeighbour(SelectedTile))
            {
                SelectedTile.ClearPiece();
            }
            SelectedTile = null;
            turn = (turn + 1) % players;
            DisplayScores();
        }
    }

    RaycastHit hitInfo = new RaycastHit();
    Ray ray = new Ray();

    public void DisplayScores()
    {
        for (int i = 0; i < players; i++)
        {
            int score = tiles.Count(t => t.PieceNr == i);
            Debug.Log("Player" + (i + 1) + ": " + score);
        }
    }
}
