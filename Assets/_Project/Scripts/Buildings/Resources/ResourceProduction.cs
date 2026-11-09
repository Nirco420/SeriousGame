using UnityEngine;

public class ResourceProduction : MonoBehaviour
{
    [SerializeField] private BuildingData buildingData;

    [SerializeField] private BuildingStorage buildingStorage;
    private int productionQuantity;
    private float productionRate;

    public void Setup(BuildingData buildingData)
    {
        this.buildingData = buildingData;
        this.buildingStorage = GetComponent<BuildingStorage>();
        productionQuantity = buildingData.resourceQuantity;
        productionRate = buildingData.zyklusTime;
        
    }

    public void Update()
    {
        if(buildingStorage == null)
        {
            return;
        }

        CheckProduction();
        Debug.Log("Ressourcen werden Produziert, aktueller Lagerbestand: " + buildingStorage.currentAmount);
        Debug.Log("Das Lager hat: " + buildingStorage.maxCapacity);
    }

    public void ProduceResource()
    {
        buildingStorage.AddResources(productionQuantity);
    }

    public void CheckProduction()
    {
        if (IsInvoking("ProduceResource") == true && buildingStorage.currentAmount >= buildingStorage.maxCapacity)
        {
            CancelInvoke("ProduceResource");
        }
        else if (IsInvoking("ProduceResource") == false && buildingStorage.currentAmount < buildingStorage.maxCapacity)
        {
            InvokeRepeating("ProduceResource", productionRate, productionRate);
        }
    }

    
}
