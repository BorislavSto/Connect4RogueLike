using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance {get ; private set;}
    
    [SerializeField] private GameObject boardLocationPrefab;

    private List<BoardLocation> boardLocations = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        { 
            instance = this;
        }
    }

    public void StartBoard(Vector2 boardSize)
    {
        for (int x = 0; x < boardSize.x; x++)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                Vector2 worldPos = new Vector2((x + gameObject.transform.position.x) * 1, (y + gameObject.transform.position.y) * 1);
                
                // Spawn board piece
                GameObject newLoc = Instantiate(boardLocationPrefab, worldPos, Quaternion.identity, transform);
                newLoc.name = $"Spot ({x}, {y})";

                // Assign data
                BoardLocationData data = new BoardLocationData
                {
                    Position = new Vector2(x, y),
                    WorldPosition = worldPos,
                    Occupied = false
                };

                // Store info on the spawned GameObject
                BoardLocation boardPiece = newLoc.GetComponent<BoardLocation>();
                if (boardPiece != null)
                    boardPiece.Setup(data);
                
                boardLocations.Add(boardPiece);
            }
        }
    }

    // This is for taking control of the piece, stopping physics and animating it dropping
    public void ActivatePiece(Piece piece, BoardLocationData data)
    {
        // TODO: If piece is dropped stop player from being able to play more this turn
        Rigidbody2D rb = piece.gameObject.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = true;
        
        BoardLocation dropLocation = CheckDropLocation(data);

        dropLocation.locationData.Piece = piece;
        
        Vector2 dropWorldLocation = dropLocation.locationData.WorldPosition;
        
        // Animate the piece to the correct location and switch turns
        float distance = Vector2.Distance(piece.transform.position, dropWorldLocation);
        float speed = 2f;
        float duration = distance / speed;

        piece.transform.DOMove(dropWorldLocation, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                if (!piece.pieceEffects.NormalPiece)
                {
                    switch (piece.pieceEffects.Effect)
                    {
                        case SpecialEffect.DestroyAround:
                            DestroyPiecesAround(dropLocation.locationData.Position);
                            break;
                        default:
                            DestroyPiecesAround(dropLocation.locationData.Position);
                            break;
                    }
                }
                
                dropLocation.locationData.Piece = piece;
                dropLocation.locationData.Occupied = true;

                if (!CheckForWin(dropLocation))
                {
                    SwitchTurn();
                }
                else
                {
                    GameManager.instance.GameOver(piece.pieceOwner);
                }
            });
        
        dropLocation.locationData.Occupied = true;
    }
    
    // This is for the board locations it will check all the pieces in the column and decide where to drop the piece
    // At the end of which if the piece is normal the turn is switched,if it's not it causes additional things and then the turn is ended
    private BoardLocation CheckDropLocation(BoardLocationData data)
    {
        // Get all board locations in the same column "Y" which is represented in Vector2 as "X"
        List<BoardLocation> columnLocations = boardLocations
            .Where(loc => (int)loc.locationData.Position.x == (int)data.Position.x)
            .OrderByDescending(loc => loc.locationData.Position.y) // Top to bottom
            .ToList();

        // Go from top to bottom, find the last unoccupied one
        BoardLocation lastFreeSpot = null;

        foreach (BoardLocation loc in columnLocations)
        {
            if (!loc.locationData.Occupied)
            {
                // Keep updating until we hit the bottom-most free
                lastFreeSpot = loc;
            }
            else
            {
                // Stop as soon as we hit an occupied space
                break;
            }
        }

        return lastFreeSpot;
    }

    private void SwitchTurn()
    {
        GameManager.instance.SwitchTurn();
    }
    
    private bool CheckForWin(BoardLocation lastPlaced)
    {
        Piece piece = lastPlaced.locationData.Piece;
        if (piece == null)
            return false;

        PieceOwner owner = piece.pieceOwner;

        return
            CheckDirection(lastPlaced, 1, 0, owner) || // Horizontal
            CheckDirection(lastPlaced, 0, 1, owner) || // Vertical
            CheckDirection(lastPlaced, 1, 1, owner) || // Diagonal up-right
            CheckDirection(lastPlaced, -1, 1, owner);  // Diagonal up-left
    }

    private bool CheckDirection(BoardLocation startLoc, int dx, int dy, PieceOwner owner)
    {
        int count = 1;

        Vector2 startPos = startLoc.locationData.Position;

        // Check forward direction
        for (int i = 1; i < 4; i++)
        {
            Vector2 checkPos = startPos + new Vector2(dx * i, dy * i);
            if (IsOwnedBy(checkPos, owner))
                count++;
            else
                break;
        }

        // Check backward direction
        for (int i = 1; i < 4; i++)
        {
            Vector2 checkPos = startPos - new Vector2(dx * i, dy * i);
            if (IsOwnedBy(checkPos, owner))
                count++;
            else
                break;
        }

        if (count >= 4)
        {
            // winner found
            return true;
        }

        return false;
    }
    
    private void DestroyPiecesAround(Vector2 centerPos)
    {
        float radius = 1.5f; // to affect only the pieces around this one piece
        var board = BoardManager.instance.GetBoardLocations();

        var affected = board.Where(loc =>
                loc.locationData.Occupied &&
                Vector2.Distance(loc.locationData.Position, centerPos) <= radius &&
                loc.locationData.Position != centerPos
        ).ToList();

        foreach (var loc in affected)
        {
            GameObject.Destroy(loc.locationData.Piece.gameObject);
            loc.PlayVFXAtLocation();
            loc.locationData.Occupied = false;
            loc.locationData.Piece = null;
        }

        ApplyGravityToColumns();
    }

    private void ApplyGravityToColumns()
    {
        var board = BoardManager.instance.GetBoardLocations();
        var columns = board.Select(loc => (int)loc.locationData.Position.x).Distinct();

        foreach (int col in columns)
        {
            var columnSpots = board
                .Where(loc => (int)loc.locationData.Position.x == col)
                .OrderBy(loc => loc.locationData.Position.y)
                .ToList();

            for (int i = 0; i < columnSpots.Count; i++)
            {
                if (!columnSpots[i].locationData.Occupied)
                {
                    for (int j = i + 1; j < columnSpots.Count; j++)
                    {
                        if (columnSpots[j].locationData.Occupied)
                        {
                            MovePiece(columnSpots[j], columnSpots[i]);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void MovePiece(BoardLocation from, BoardLocation to)
    {
        GameObject pieceObj = from.locationData.Piece.gameObject;
        from.locationData.Piece = null;
        from.locationData.Occupied = false;

        // Move to new world position
        pieceObj.transform.DOMove(to.locationData.WorldPosition, 1f)
            .SetEase(Ease.OutQuad);

        to.locationData.Piece = pieceObj.GetComponent<Piece>();
        to.locationData.Occupied = true;
    }

    private bool IsOwnedBy(Vector2 pos, PieceOwner owner)
    {
        BoardLocation loc = boardLocations.FirstOrDefault(l => l.locationData.Position == pos);
        if (loc == null || !loc.locationData.Occupied)
            return false;

        Piece piece = loc.locationData.Piece;
        return piece != null && piece.pieceOwner == owner;
    }

    public List<BoardLocation> GetBoardLocations()
    {
        return boardLocations;
    }
}