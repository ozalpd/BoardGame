using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
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
            }
        }
    }
    private int? _pieceNr;
    private GameObject playerGo;

    public bool CanJump(TileController tile)
    {
        int colDiff = Mathf.Abs(ColNr - tile.ColNr);
        int rowDiff = Mathf.Abs(RowNr - tile.RowNr);

        return colDiff < 3 && rowDiff < 3;
    }

    public bool IsNeighbour(TileController tile)
    {
        int colDiff = Mathf.Abs(ColNr - tile.ColNr);
        int rowDiff = Mathf.Abs(RowNr - tile.RowNr);

        return colDiff < 2 && rowDiff < 2;
    }
}
