using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    public event Action<BuildingData> OnBuildingSelected;

    [SerializeField] private Button buildingButton;
    [SerializeField] private GameObject buildingPanel;

    [SerializeField] private Button building1;
    [SerializeField] private Button building2;
    [SerializeField] private Button building3;

    [SerializeField] private BuildingData building1x1;
    [SerializeField] private BuildingData building2x2;
    [SerializeField] private BuildingData building3x3;

    public void Start() //Listener für die Buttons erstellen
    {
        buildingButton.onClick.AddListener(OnBuildingButtonClicked);

        building1.onClick.AddListener(OnBuilding1Clicked);
        building2.onClick.AddListener(OnBuilding2Clicked);
        building3.onClick.AddListener(OnBuilding3Clicked);

    }

    public void OnBuildingButtonClicked() //RundesPanel wo die Gebäude Buttons drinne sind sichtbar schalten
    {
        buildingPanel.SetActive(true);
    }

    public void BuildingFinished() //Methode das Panel wir aus zu schalten
    {
        buildingPanel.SetActive(false);
    }

    public void OnBuilding1Clicked() //Event wenn das einer Gebäude gewählt wurde
    {
        OnBuildingSelected?.Invoke(building1x1);
    }

    public void OnBuilding2Clicked()//Event wenn das zweir Gebäude gewählt wurde
    {
        OnBuildingSelected?.Invoke(building2x2);
    }

    public void OnBuilding3Clicked()//Event wenn das dreier Gebäude gewählt wurde
    {
        OnBuildingSelected?.Invoke(building3x3);
    }
}
