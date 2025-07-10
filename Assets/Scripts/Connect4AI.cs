using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Connect4AI : MonoBehaviour
{
    [SerializeField] private GameObject aiPiecePrefab;
    [SerializeField] private Transform aiPieceContainer;
    [SerializeField] private float spawnHeight = 3f;
    [SerializeField] private float thinkTime = 0.5f;

    private void OnEnable()
    {
        EventManager.TurnSwitch += OnAITurn;
    }

    private void OnDisable()
    {
        EventManager.TurnSwitch -= OnAITurn;
    }

    private void OnAITurn(CurrentTurn currentTurn)
    {
        if (currentTurn == CurrentTurn.AI)
            StartCoroutine(PlayTurn());
    }

    private IEnumerator PlayTurn()
    {
        yield return new WaitForSeconds(thinkTime);

        var availableColumns = GetAvailableColumns();
        if (availableColumns.Count == 0)
        {
            GameManager.instance.SwitchTurn();
            yield break;
        }

        int chosenColumn = availableColumns[Random.Range(0, availableColumns.Count)];
        DropPiece(chosenColumn);
    }

    private List<int> GetAvailableColumns()
    {
        var board = BoardManager.instance.GetBoardLocations();
        var columns = board.Select(loc => (int)loc.locationData.Position.x).Distinct().ToList();
        List<int> available = new List<int>();

        foreach (int col in columns)
        {
            var columnSpots = board
                .Where(loc => (int)loc.locationData.Position.x == col)
                .OrderBy(loc => loc.locationData.Position.y)
                .ToList();

            if (columnSpots.Any(loc => !loc.locationData.Occupied))
                available.Add(col);
        }

        return available;
    }

    private BoardLocation GetLowestEmptyInColumn(int column)
    {
        var board = BoardManager.instance.GetBoardLocations();
        var columnSpots = board
            .Where(loc => (int)loc.locationData.Position.x == column)
            .OrderBy(loc => loc.locationData.Position.y)
            .ToList();

        foreach (var loc in columnSpots)
        {
            if (!loc.locationData.Occupied)
                return loc;
        }

        return null;
    }

    private void DropPiece(int column)
    {
        var target = GetLowestEmptyInColumn(column);
        if (target == null)
        {
            Debug.LogWarning("AI: No free spot in column!");
            GameManager.instance.SwitchTurn();
            return;
        }

        Vector2 spawnPos = new Vector2(
            target.locationData.WorldPosition.x,
            target.locationData.WorldPosition.y + spawnHeight
        );

        GameObject pieceObj = Instantiate(aiPiecePrefab, spawnPos, Quaternion.identity, aiPieceContainer);
        Piece aiPiece = pieceObj.GetComponent<Piece>();
        
        PieceEffects effects = new PieceEffects { NormalPiece = true };
        aiPiece.SetupPiece(PieceOwner.AI, null, effects);

        Rigidbody2D rb = pieceObj.GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = pieceObj.AddComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.freezeRotation = true;
    }
}