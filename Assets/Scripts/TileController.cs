using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileController : MonoBehaviour
{
    private void Awake()
    {
        _neighbourTiles = new List<TileController>();
        _moveTiles = new List<TileController>();
    }

    public string Address
    {
        get
        {
            return string.Format("{0}x{1}", ColNr, RowNr);
        }
    }

    public BoardController Board { get; set; }
    public int ColNr { get; set; }
    public int RowNr { get; set; }

    public bool HasPiece { get { return _pieceNr.HasValue; } }

    public IEnumerable<TileController> EmptyMoveableTiles
    {
        get { return _moveTiles.Where(t => !t.HasPiece); }
    }

    public IEnumerable<TileController> EmptyNeighbourTiles
    {
        get { return _neighbourTiles.Where(t => !t.HasPiece); }
    }


    public IEnumerable<TileController> MoveableTiles
    {
        get { return _moveTiles; }
    }
    private List<TileController> _moveTiles;

    public IEnumerable<TileController> NeighbourTiles
    {
        get { return _neighbourTiles; }
    }
    private List<TileController> _neighbourTiles;

    public int PieceNr
    {
        get { return _pieceNr ?? -1; }
        set
        {
            if (value >= 0 && value < Board.playPieces.Length)
            {
                _pieceNr = value;
                if (playerGo != null)
                    Destroy(playerGo);

                playerGo = Instantiate(Board.playPieces[value],
                                transform.position,
                                Board.PieceRotations[value]);
                playerGo.transform.SetParent(transform);
                _piece = playerGo.GetComponent<BoardPiece>();
            }
        }
    }
    private int? _pieceNr;
    private GameObject playerGo;

    public BoardPiece Piece { get { return _piece; } }
    private BoardPiece _piece;

    public bool CanJump(TileController tile)
    {
        int colDiff = Mathf.Abs(ColNr - tile.ColNr);
        int rowDiff = Mathf.Abs(RowNr - tile.RowNr);

        return colDiff < 3 && rowDiff < 3;
    }

    public void ClearPiece()
    {
        if (playerGo != null)
            Destroy(playerGo);

        playerGo = null;
        _piece = null;
        _pieceNr = null;
    }

    public bool IsNeighbour(TileController tile)
    {
        int colDiff = Mathf.Abs(ColNr - tile.ColNr);
        int rowDiff = Mathf.Abs(RowNr - tile.RowNr);

        return tile != this && colDiff < 2 && rowDiff < 2;
    }

    public void SetRelatedTiles()
    {
        _neighbourTiles = new List<TileController>();
        _moveTiles = new List<TileController>();

        foreach (var t in Board.Tiles)
        {
            if (IsNeighbour(t))
                _neighbourTiles.Add(t);

            if (IsNeighbour(t) || CanJump(t))
                _moveTiles.Add(t);
        }
    }
}
