using System;
using UnityEngine;

public class BoardLocation : MonoBehaviour
{
    [SerializeField] private GameObject vfxPrefab;

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
    
    public void PlayVFXAtLocation()
    {
        if (vfxPrefab == null)
        {
            Debug.LogWarning("VFX Prefab not assigned!");
            return;
        }

        GameObject vfxInstance = Instantiate(vfxPrefab, transform.position, Quaternion.identity);

        // Destroy the VFX after 2 seconds (adjust timing as needed)
        Destroy(vfxInstance, 2f);
    }
}
