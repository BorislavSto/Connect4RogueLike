using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool NormalPiece;
    public PieceEffects pieceEffects;
    public bool inPlay;
    public bool isDragging;
}

// Will contain all the information to for special effects such as destroying other pieces or sideways movement
public struct PieceEffects
{
    
}