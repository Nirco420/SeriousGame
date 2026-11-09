using System.Collections.Generic;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    public enum BuildingState
    {
        None,
        POSITIVE,
        NEGATIVE
    }

    public BuildingState state;
    [SerializeField] private BuildingData buildingData;
    [SerializeField] private BuildingModel buildingModel;
    [SerializeField] private GridManager gridManager;

    private List<Renderer> renderers;

    [SerializeField] private Material positive;
    [SerializeField] private Material negative;

    public void Setup(BuildingData data, GridManager grid) //Preview erzeugen und all renderers holen
    {
        this.gridManager = grid;
        this.buildingData = data;
        buildingModel = Instantiate(buildingData.Model, transform.position, Quaternion.identity, transform);

        renderers = new List<Renderer>(buildingModel.GetComponentsInChildren<Renderer>(true));
    }

    public void Update() //prüfen an welcher Position die ShapeUnits sind, prüfen ob die Zellen frei sind und dann preview Material setzen
    {
        List<Vector3> positions = new List<Vector3>(buildingModel.GetAllBuildingPositions());
        bool isFree = positions.Count > 0 && gridManager.CanBuild(positions);

        if (isFree == true)
        {
            SetPreview(BuildingState.POSITIVE);
        }
        else
        {
            SetPreview(BuildingState.NEGATIVE);
        }

    }

    public BuildingModel GetBuildingModel() //Gibt das aktuelle BuildingModel zurück, um es in BuildingPlacement auf zu rufen
    {
          return buildingModel;
    }

    public bool CanPlace() //Gibt einen boll zurück ob alle referenzen Hinterlegt sind und ob die Zellen, wo die ShapeUnits drauf sind frei sind
    {
        return buildingModel != null && gridManager != null && gridManager.CanBuild(buildingModel.GetAllBuildingPositions());

    }


    public void SetPreview(BuildingState newState) //Je nach State das entsprechende Material setzen
    {
        if (state == newState)
        {
            return;
        }
        
        state = newState;

            switch (state)
            {
                case BuildingState.POSITIVE:
                    foreach (var r in renderers)
                    {
                        r.material = positive;
                    }

                    break;

                case BuildingState.NEGATIVE:
                    foreach (var r in renderers)
                    {
                        r.material = negative;
                    }
                    break;
            }
    }
}
