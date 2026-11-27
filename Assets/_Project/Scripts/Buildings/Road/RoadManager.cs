using NUnit.Framework;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    private enum BuildingState
    {
        IDLE,
        PLACING, 
        DRAGING,
        CONFIRMING
    }

    [SerializeField] private GridManager grid;
    [SerializeField] private Segment segment;
    [SerializeField] private InputManager input; 
    [SerializeField] private BuildingUI buildingUI;
    [SerializeField] private BuildingPlacement placement;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private MeshCollider meshCollider;
    
    
    private RoadDirection roadDirection;
    private BuildingModel currentBuilding;
    private BuildingState currentBuildingState;

    private Vector3 startPos; 
    private Vector2Int endPosition;
    private Vector2Int constructionDirection;

    private List<Vector2Int> previews;
    private List<Vector2Int> planedWayPoints;

    private GameObject rightArrow;
    private GameObject leftArrow;
    private GameObject topArrow;
    private GameObject bottomArrow;


    public void OnEnable()
    {
        buildingUI.OnBuildingSelected += OnRoadStartBuilding;
        input.OnFingerBegan += OnArrowTouchStart;
        input.OnFingerMoved += OnArrowDragged;
    }

    public void Awake()
    {
        currentBuildingState = BuildingState.IDLE;
    }
  
    void Update()
    {
        switch (currentBuildingState)
        {
            case BuildingState.IDLE:
                break;

            case BuildingState.PLACING:
                GetStartPosition();
                break;

            case BuildingState.DRAGING:
                break;

            case BuildingState.CONFIRMING:
                break;
        }
    }

    private void OnRoadStartBuilding(BuildingModel road) //Wenn ein Weg ausgewählt wird, dann event Triggern und State aktualisieren
    {
        currentBuilding = road;

        if (currentBuilding.BuildingData.type == BuildingData.BuildingType.ROAD)
        {
            currentBuildingState = BuildingState.PLACING;
        }
        
    }

    private void GetStartPosition() //aktuelles Grid holen, sowie die GridKoordinaten der Preview. Dann prüfen ob Zelle bebaubar ist und obs eine Straße ist. Wenn ja pfeile Aktivieren
    {
        if (placement.currentPreview == null)
        {
            return;
        }


        grid = placement.grid;
        roadDirection = placement.currentPreview.GetComponent<RoadDirection>();
        (int x, int z) = grid.WorldToGridPosition(placement.currentPreview.transform.position);
        if (grid.CanBuild(placement.currentPreview.GetAllBuildingPositions()) == true && placement.currentPreview.BuildingData.type == BuildingData.BuildingType.ROAD)
        {
            CreateArrows(x, z);
        }
        else
        {
            roadDirection.DeactivateArrows();
        }

    }

    private void CreateArrows(int x, int z) //Nachbarn der aktuellen Zelle suchen und prüfen ob die nachbarn bebaut werden können oder nicht. Wenn ja pfeil anzeigen wenn nein nicht 
    {
        Vector2Int startPos = new Vector2Int(x, z);
        List <Vector2Int> neighborCells = new List<Vector2Int>();

        Vector2Int rightNeighbor = new Vector2Int(x + 1, z);
        Vector2Int upperNeighbor = new Vector2Int(x, z+1);

        Vector2Int leftNeighbor = new Vector2Int (x -1, z);
        Vector2Int downNeighbor = new Vector2Int (x, z - 1);

        neighborCells.Add(rightNeighbor);
        neighborCells.Add(upperNeighbor);
        neighborCells.Add(leftNeighbor);
        neighborCells.Add(downNeighbor);

        

        foreach (Vector2Int neighbors in neighborCells)
        {
            List <Vector3> neighborPositions = new List<Vector3> ();
            Vector3 neighborPos = grid.GridToWorldCenterPosition(neighbors.x, neighbors.y);
            neighborPositions.Add(neighborPos);

            Vector2Int direction = neighbors - startPos;

            roadDirection.GetDirection(direction, grid.CanBuild(neighborPositions));
            
        }
    }

    public void OnArrowTouchStart(Vector2 pos) //Prüfen welcher Pfeil getroffen wurde und dann seine Richtung speichern
    {
        if (placement.currentPreview == null)
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(pos);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
        GameObject hitChild = null;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(placement.currentPreview.transform))
            {
                hitChild = hit.collider.gameObject;
                currentBuildingState = BuildingState.DRAGING;
                break;
            }
        }
        ArrowDirection currentArrow = hitChild.GetComponent<ArrowDirection>();
        constructionDirection = currentArrow.direction;
    }

    public void OnArrowDragged(Vector2 pos)
    {
        startPos = placement.currentPreview.transform.position;
        Ray ray = mainCamera.ScreenPointToRay(pos);

        if(meshCollider.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
        {
            (int x, int z) = grid.WorldToGridPosition(hitInfo.point);
        }

    }

    public void OnDisable()
    {
        buildingUI.OnBuildingSelected -= OnRoadStartBuilding;
        input.OnFingerMoved -= OnArrowDragged;
        input.OnFingerBegan -= OnArrowTouchStart;
    }


}
