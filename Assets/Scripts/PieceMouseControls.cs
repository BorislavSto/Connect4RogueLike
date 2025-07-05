using UnityEngine;

public class PieceMouseControls : MonoBehaviour
{
    private Vector3 offset;
    private Camera cam;
    private Piece piece;

    private void Start()
    {
        cam = Camera.main;
        piece = GetComponent<Piece>();
    }

    private void OnMouseDown()
    {
        piece.isDragging = true;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0f);
    }

    private void OnMouseDrag()
    {
        if (!piece.isDragging) return;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0f) + offset;
        transform.position = targetPos;
    }

    private void OnMouseUp()
    {
        piece.isDragging = false;

        Debug.Log("Piece dropped");
    }
}