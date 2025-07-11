using System;
using UnityEngine;

[Serializable]
public struct PieceEffects
{
    [SerializeField]
    public bool NormalPiece;
    public SpecialEffect Effect;
}

[Serializable]
public struct PieceAndCardData
{
    public string title;
    public string description;
    public Sprite art;
    public PieceEffects effect;
    public PieceOwner owner;
}

[Serializable]
public struct BoardLocationData
{
    public Vector2 Position;
    public Vector2 WorldPosition;
    public bool Occupied;
    public Piece Piece;
}

public enum SpecialEffect
{
    DestroyAround,
}

public enum PieceOwner
{
    Player,
    AI,
}

public enum CurrentTurn
{
    Player,
    AI,
}
