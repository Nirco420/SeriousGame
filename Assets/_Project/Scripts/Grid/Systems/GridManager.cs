using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize = 1f;

    [SerializeField] private MeshCollider meshCollider;


    private BuildingGridCell[,] grid;
    public bool IsReady { get; private set; }

    private void Awake() //Grid erzeugen und je nach größe der Widht und Height neue Felder erzeugen und ins 2D-Array aufnehmen
    {
        if (width <= 0 || height <= 0 || cellSize <= 0f || meshCollider == null)
        {
            return;
        }

        grid = new BuildingGridCell[width, height];

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                grid[x, z] = new BuildingGridCell();
                grid[x, z].SetBuildable(IsCellOnCollider(x, z));
            }
        }
        IsReady = true;
    }

    public Vector3 GridToWorldPosition(int x, int z) //Berechnet die Grid koordinaten in Worldkoordinaten um 
    {
        return new Vector3(transform.position.x + x * cellSize, transform.position.y, transform.position.z + z * cellSize);
    }

    public (int x, int z) WorldToGridPosition(Vector3 worldPos) //Berechnet die Worldposition in GridPositionen um 
    {
        int x = Mathf.FloorToInt((worldPos - transform.position).x / cellSize);
        int z = Mathf.FloorToInt((worldPos - transform.position).z / cellSize);
        return (x,z);
    }

    public bool TrySetBuilding(BuildingModel building, List<Vector3> allBuildingPositions) //Trägt das Gebäude auf die Zellen ein, wenn diese frei sind
    {
        if (building == null || CanBuild(allBuildingPositions) == false)
        {
            return false;
        }

        foreach (Vector3 buildingPos in allBuildingPositions)
        {
            (int x, int z) = WorldToGridPosition(buildingPos);
            grid[x, z].SetBuilding(building);
        }
        return true;
    }

    public bool CanBuild(List<Vector3> allBuildingPositions) //Geht alle ZellPositionen durch und prüft, ob sie im Grid liegen, ob sie frei sind und ob sie auf dem meshCollider liegen
    {

        if (IsReady == false || allBuildingPositions == null || allBuildingPositions.Count == 0)
        {
            return false;
        }
        foreach (Vector3 buildingPos in allBuildingPositions)
        {
            (int x, int z) = WorldToGridPosition(buildingPos);
            
            if(x < 0 || x >= width || z < 0 || z >= height)
            
            {
                return false;
            }

            if (!grid[x,z].IsEmpty())
            {
                return false;
            }
        }
        return true;
    }

    public void OnDrawGizmos() //Über einen Gizmos das Grid Zeichnen
    {
        Gizmos.color = Color.green;
        if (cellSize <= 0f || width <= 0 || height <= 0)
        {
            return;
        }

        if (grid != null)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y].GetBuildable() == true)
                    {
                        Vector3 currentCellPos = GridToWorldPosition(x, y);
                        Gizmos.DrawWireCube(new Vector3(currentCellPos.x + cellSize / 2, currentCellPos.y, currentCellPos.z + cellSize / 2), new Vector3(cellSize, 0f, cellSize));
                    }
                }
            }
        }
        
    }

    public bool IsCellOnCollider(int x, int z) //Erzeugt in jeder Zellecke einen Ray der prüft ob die Celle auf dem Collider liegt oder nicht. 
    {
        Vector3 currentCellPos = GridToWorldPosition(x, z);
        Ray leftUpperRay = new Ray(new Vector3(currentCellPos.x + 0.3f, currentCellPos.y + 10f, currentCellPos.z + 0.3f), Vector3.down);
        Ray rightUpperRay = new Ray(new Vector3(currentCellPos.x + (cellSize - 0.3f), currentCellPos.y + 10f, currentCellPos.z + 0.3f), Vector3.down);
        Ray leftBottomRay = new Ray(new Vector3(currentCellPos.x + 0.3f, currentCellPos.y + 10f, currentCellPos.z + (cellSize - 0.3f)), Vector3.down);
        Ray rightBottomRay = new Ray(new Vector3(currentCellPos.x + (cellSize - 0.3f), currentCellPos.y + 10f, currentCellPos.z + (cellSize - 0.3f)), Vector3.down);


        if (meshCollider.Raycast(leftUpperRay, out RaycastHit hitInfo, Mathf.Infinity) == true && meshCollider.Raycast(rightUpperRay, out RaycastHit hitInfo1, Mathf.Infinity) == true && meshCollider.Raycast(leftBottomRay, out RaycastHit hitInfo2, Mathf.Infinity) == true && meshCollider.Raycast(rightBottomRay, out RaycastHit hitInfo3, Mathf.Infinity) == true)
        {
            return true;
        }
        return false;
    }

    public Vector3 GridToWorldCenterPosition(int x, int z) //Hilfsmethode für das GridSnapping um den Mittelpunkt für jede Zelle bestimmen zu können.
    {
        return GridToWorldPosition(x, z) + new Vector3(cellSize / 2f, 0f, cellSize / 2f);
    }
}

public class BuildingGridCell
{

    private BuildingModel building;

    private bool isBuildable;

    public void SetBuilding(BuildingModel building) //Setzt das aktuell zugewiesene BuildingModell als aktuelles Modell
    {
        this.building = building;
    }

    public void SetBuildable(bool isBuild) //Wird im GridManager aufgerufen um isBuildable zu setzen
    {
        isBuildable = isBuild;
    }

    public bool GetBuildable() //gibt zurück ob eine Zelle bebaubar ist oder nicht
    {
        return isBuildable;
    }

    public bool IsEmpty() // prüft ob eine Zelle bebaubar ist, also auf dem MeshCollider liegt und ob sie leer ist also kein Gebäude drauf ist
    {
        if(isBuildable == true && building == null)
        {
            return true;
        }
        return false;
    }
}
