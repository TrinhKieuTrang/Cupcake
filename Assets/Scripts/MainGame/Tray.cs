using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tray : MonoBehaviour
{
    public SweetColor trayColor;
    private float moveDuration = 0.15f;
    [SerializeField] private bool onlyX = false, onlyY = false;

    private Camera cam;
    private BoardManager board;
    private bool isDragging;
    private Vector2Int startGridPos;
    [SerializeField] private Vector2Int[] occupiedCells;

    private Dictionary<Vector2Int, Sweet> sweetSlots = new Dictionary<Vector2Int, Sweet>();
    public bool IsDone { get; private set; } = false;
    public bool isMoving { get; private set; } = false;

    void Start()
    {
        cam = Camera.main;
        board = FindFirstObjectByType<BoardManager>();
        UpdateOccupiedCells();
        startGridPos = board.WorldToGrid(transform.position);
        board.PlaceTray(this, startGridPos, occupiedCells);
    }
    private void Update()
    {
        if (IsDone || isMoving || GameManager.Instance.IsGameOver()) return;

        SweetRay ray = board.GetSweetRayAt(startGridPos);
        if (ray != null) ray.PickSweet(this);
    }
    public void Down()
    {
        if (IsDone || GameManager.Instance.IsGameOver()) return;
        if(GameManager.Instance.IsBombMode())
        {
            GameManager.Instance.UseBombOnTray(this);
            return;
        }
        isDragging = true;
        UpdateOccupiedCells();
        startGridPos = board.WorldToGrid(transform.position);
        board.ClearTray(this);
    }

    public void Drag()
    {
        if (!isDragging || IsDone || GameManager.Instance.IsGameOver()) return;
        Vector3 mouseWorld = GetMouseWorldPos();
        Vector2Int gridPos = board.WorldToGrid(mouseWorld);

        if(onlyX) gridPos.y = startGridPos.y;
        if(onlyY) gridPos.x = startGridPos.x;

        if (board.CanMoveTo(this, startGridPos, gridPos, occupiedCells))
        {
            Vector3 target = board.GridToWorld(gridPos);
            board.ClearTray(this);

            transform.DOMove(target, moveDuration).OnComplete(() =>
            {
                if(IsDone) return;
                startGridPos = gridPos;
                board.PlaceTray(this, startGridPos, occupiedCells);
            });
        }
    }


    public void Up()
    {
        if (IsDone) return;
        isDragging = false;
    }

    public void TryAddSweet(Sweet sweet)
    {
        if (IsDone || isMoving) return;
        isMoving = true;
        Transform pos = IsEmpty();
        if(pos != null)
        {
            sweet.transform.SetParent(pos);
            sweet.transform.DOLocalMove(Vector3.zero, 0.3f).OnComplete(() =>
            {
                isMoving = false;
                CheckDone();

            });
        }
    }

    public Transform IsEmpty()
    {
        foreach(Transform pos in transform)
        {
            if(pos.childCount == 0) return pos;
        }
        return null;
    }

    void CheckDone()
    {
        if (IsEmpty() == null && !IsDone)
        {
            IsDone = true;
            startGridPos = board.WorldToGrid(transform.position);
            board.ClearTray(this);
            DoneAnim();
            GameManager.Instance.CheckGameWin();
        }
    }

    void UpdateOccupiedCells()
    {
        occupiedCells = transform.Cast<Transform>() 
            .Where(t => t != transform)             
            .Select(t => board.WorldToGrid(t.position) - board.WorldToGrid(transform.position))
            .ToArray();
    }


    Vector3 GetMouseWorldPos()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        plane.Raycast(ray, out float enter);
        return ray.GetPoint(enter);
    }

    private void DoneAnim()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.3f);
        Vector3 stepUp = transform.position + Vector3.up * 0.1f;
        seq.Append(transform.DOMove(stepUp, 0.05f));
        seq.AppendInterval(0.3f);
        Vector3 finalPos = new Vector3(3f, transform.position.y, transform.position.z);
        seq.Append(transform.DOMove(finalPos, 0.5f));

        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void Boom()
    {
        startGridPos = board.WorldToGrid(transform.position);

        board.ClearTray(this);

        transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            gameObject.SetActive(false);
            GameManager.Instance.CheckGameWin();
        });

    }
}
