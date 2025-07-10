using UnityEngine;

public class PieceMouseControls : MonoBehaviour
{
    private Vector3 offset;
    private Camera cam;
    private Piece piece;

    private void Awake()
    {
        cam = Camera.main;
        piece = GetComponent<Piece>();
    }

    private void Update()
    {
        if (!piece.isDragging)
            return;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0f) + offset;
        transform.position = targetPos;
        
        if (Input.GetMouseButtonUp(0))
            piece.isDragging = false;
    }
    
    private void OnMouseDown()
    {
        if (piece.inPlay) return;
        
        piece.isDragging = true;

        StartDragging();
    }
    
    // private void OnMouseDrag()
    // {
    //     if (!piece.isDragging) return;
    //
    //     Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
    //     Vector3 targetPos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0f) + offset;
    //     transform.position = targetPos;
    // }
    
    public void StartDragging()
    {
        if (piece.inPlay) return;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        gameObject.transform.position = mouseWorldPos;
        offset = transform.position - new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0f);
        offset.z = 0;
    }

    private void OnMouseUp()
    {
        piece.isDragging = false;

        Debug.Log("Piece dropped");
    }
}
