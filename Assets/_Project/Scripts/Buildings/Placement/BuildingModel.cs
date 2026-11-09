using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingModel : MonoBehaviour
{
    [SerializeField] private GameObject wrapper;
    public float rotation;
    private BuildingShapeUnit[] shapeUnits;


    public void Awake() //Holt sich alle ShepUnits welche Child von BuildingModel sind
    {
        shapeUnits = GetComponentsInChildren<BuildingShapeUnit>();
    }

    public void Rotate(float rotationStep) //Hier kann der Wrapper gedreht werden, somit also das ganze Gebäude
    {
       wrapper.transform.Rotate(new(0, rotationStep,0));
    }

    public List<Vector3> GetAllBuildingPositions() //Holt sich von jeder Shapunit die Position und speichert sie in der Liste
    {
        return shapeUnits.Select(unit => unit.transform.position).ToList();
    }
}
