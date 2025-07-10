using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance {get ; private set;}
    
    private List<BoardLocation> boardLocations = new();
    
    public GameObject boardLocationPrefab;

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

        Vector2 dropWorldLocation = dropLocation.locationData.WorldPosition;
        
        // Animate the piece to the correct location and switch turns
        float distance = Vector2.Distance(piece.transform.position, dropWorldLocation);
        float speed = 3f;
        float duration = distance / speed;

        piece.transform.DOMove(dropWorldLocation, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(SwitchTurn);
        
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
}
