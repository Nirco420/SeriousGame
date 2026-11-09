using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/BuildingData")]
public class BuildingData : ScriptableObject
{
    public enum BuildingType
    {
        None,
        STORAGE,
        PRODUKTION,
        RESOURECES
    }

    public string description;
    public int cost;
    public BuildingModel Model;

    public int resourceQuantity;
    public int maxStorage;
    public float zyklusTime;


}
