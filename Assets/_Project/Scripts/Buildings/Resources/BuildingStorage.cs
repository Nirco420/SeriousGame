using UnityEngine;

public class BuildingStorage : MonoBehaviour
{
    [SerializeField] private BuildingData buildingData;

    public int currentAmount {  get; private set; }
    public int maxCapacity { get; private set; }
    private int procucedResources;

    public void Setup(BuildingData buildingData)
    {
        this.buildingData = buildingData;
        currentAmount = 0;
        maxCapacity = buildingData.maxStorage;

    }

    public void AddResources(int amount) //Berechnet wieviel Speicher im Gebäude noch frei ist und rechnet dann denn niedrigsten wert drauf
    {
        if (amount  < 0)
        {
            return;
        }
        int freeStorage = maxCapacity - currentAmount;
        amount = Mathf.Min(freeStorage, amount);
        currentAmount += amount;

    }

    public int RemoveResources(int amount) //Schaut wieviele Ressourcen entnommen werden können und rechnet dann den niedrigeren raus
    {
        if (amount < 0)
        {
            return 0;
        }

        amount = Mathf.Min(currentAmount, amount);
        currentAmount -= amount;

        return amount;

    }

}
