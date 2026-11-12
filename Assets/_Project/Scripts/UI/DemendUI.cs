using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemendUI : MonoBehaviour
{
    [SerializeField] private DemendManger demendManger;

    [SerializeField] private GameObject demandPanel;
    [SerializeField] private TextMeshProUGUI headlineText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI resourceName;
    [SerializeField] private TextMeshProUGUI demandQuantity;
    [SerializeField] private TextMeshProUGUI demandDeadline;
    [SerializeField] private Button closeButton;

    private ResourceData resourceData;

    


    private void OnEnable()
    {
        demendManger.OnDemandDisplayRequested += DemandPopUP;
    }

    public void Start()
    {
        closeButton.onClick.AddListener(ClosePopUp);
        
    }

    public void DemandPopUP(DemandData currentDemand, int currentRound)
    {
        demandPanel.SetActive(true);
        resourceData = currentDemand.resource;
        int currentdemandQuantity = currentDemand.quantity;
        int currentdemandDeadline = currentDemand.deadline;


        headlineText.text = currentDemand.name;
        descriptionText.text = currentDemand.description;
        resourceName.text = resourceData.name;
        demandQuantity.text = "Menge: " + currentdemandQuantity.ToString();
        demandDeadline.text = "In Runden: " + currentdemandDeadline.ToString();

    }

    public void ClosePopUp()
    {
        demandPanel?.SetActive(false);
    }

    private void OnDisable()
    {
        demendManger.OnDemandDisplayRequested -= DemandPopUP;
    }
}
