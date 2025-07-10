using System;
using UnityEngine;

public class BoardLocation : MonoBehaviour
{
    public BoardLocationData locationData;
    
    public void Setup(BoardLocationData data)
    {
        locationData = data;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Piece"))
        {
            Piece otherPiece = other.GetComponent<Piece>();
            if (otherPiece.inPlay) 
                return;
            
            if (otherPiece.isDragging)
                return;

            otherPiece.SetPieceInPlay();
            BoardManager.instance.ActivatePiece(other.GetComponent<Piece>(), locationData);
        }
    }

    private void PlayVFXLocation()
    {
        
    }
}

[Serializable]
public struct BoardLocationData
{
    public Vector2 Position;
    public Vector2 WorldPosition;
    public bool Occupied;
    public Piece Piece;
}
