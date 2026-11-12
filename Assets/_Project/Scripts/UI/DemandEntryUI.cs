using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemandEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI demandResource;
    [SerializeField] private TextMeshProUGUI demandTitle;
    [SerializeField] private TextMeshProUGUI quanatityText;
    [SerializeField] private TextMeshProUGUI remainingRoundsLeft;
    [SerializeField] private Button giveAll;
    [SerializeField] private Button giveHalf;
    [SerializeField] private Button needMoreTime;
   

    public void Setup(DemandData currentDemand, int remainingRounds) //Initialisiert die Aktuelle Forderung in der Forderungsübersicht
    {
        demandTitle.text = currentDemand.name;
        demandResource.text = currentDemand.resource.name;
        quanatityText.text = currentDemand.quantity.ToString();
        remainingRoundsLeft.text = "Fällig in " + remainingRounds;

    }
}
