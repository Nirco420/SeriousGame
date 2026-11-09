using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private BuildingUI buildingUI;
    [SerializeField] private BuildingPreview previewPrefab;
    [SerializeField] private GridManager grid;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private float tapThreshold = 20f;

    private BuildingData currentBuilding;
    private BuildingPreview currentPreview;
    private bool isDragging;

    private Vector2 touchStartPosition;
    private bool hasMoved;

    private void OnEnable()
    {
        inputManager.OnFingerMoved += OnTouchMoved;
        inputManager.OnFingerBegan += OnTouchBegan;
        inputManager.OnFingerEnded += OnTouchEnded;
        buildingUI.OnBuildingSelected += GetSelectedBuilding;
    }

    public void OnTouchBegan(Vector2 pos) //Ray von Camera zu erzeugten Building erstellen. Prüfen ob Ray Collider vom Building trifft, wenn ja finger StartPosition auf die getroffene Position setzen
    {
        if (currentPreview == null)
        {
            return;
        }  

        Ray ray = mainCamera.ScreenPointToRay(pos);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(currentPreview.transform))
            {
                isDragging = true;
                hasMoved = false;
                touchStartPosition = pos;
                break;
            }
        }
    }

    public void OnTouchMoved(Vector2 pos) //GridSnapping, Ray erzeugen von Camera zu Fingerposition, wenn Raycast hit auf MeshCollider, dann Weltkoordinaten in Grid Koordinaten umrechnen und PreviewPosition aktuallisieren
    {
        if (!isDragging || currentPreview == null)
        {
            return;
        }

        if (Vector2.Distance(touchStartPosition, pos) > tapThreshold)
        {
            hasMoved = true;
        }

        if (!hasMoved)
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(pos);

        if (meshCollider.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
        {
            (int x, int z) = grid.WorldToGridPosition(hitInfo.point);

            currentPreview.transform.position = grid.GridToWorldCenterPosition(x, z);

        }
    }

    public void OnTouchEnded(Vector2 pos) //Prüfen ob getapt wurde, wenn ja und die Position passt, dann Gebäude tatsächlich platzieren
    {
        if (!isDragging)
        {
            return;
        }

        bool isTap = !hasMoved && Vector2.Distance(touchStartPosition, pos) <= tapThreshold;


        isDragging = false;

        if (isTap)
        {
            PlaceBuilding();
        }
    }

    public void GetSelectedBuilding(BuildingData buildingData) //BuildingData setzen und Preview erzeugen
    {
        if(currentPreview != null)
        {
            Destroy(currentPreview.gameObject);
        }

        currentBuilding = buildingData;
        currentPreview = Instantiate(previewPrefab,transform.position, Quaternion.identity, transform);
        currentPreview.Setup(currentBuilding, grid); 
    }

    public void PlaceBuilding() //Überprüfen ob GridCellen frei sind und dann Preview gegen das tatsächliche Gebäude tauschen
    {
        if (currentPreview == null || currentBuilding == null)
        {
            return;
        }

        if (!currentPreview.CanPlace())
        {
            return;
        }

        BuildingModel previewModel = currentPreview.GetBuildingModel();

        BuildingModel placedBuilding = Instantiate(currentBuilding.Model, previewModel.transform.position, previewModel.transform.rotation);
        BuildingStorage storage = placedBuilding.GetComponent<BuildingStorage>();
        ResourceProduction production = placedBuilding.GetComponent<ResourceProduction>();
        
        if(storage != null && production != null)
        {
            storage.Setup(currentBuilding);
            production.Setup(currentBuilding); 
        }

        grid.SetBuildig(placedBuilding, placedBuilding.GetAllBuildingPositions());

        Destroy(currentPreview.gameObject);

        currentPreview = null;
        currentBuilding = null;
        isDragging = false;
        buildingUI.BuildingFinished();
    }



    private void OnDisable() //Events wieder ausschalten
    {
        inputManager.OnFingerMoved -= OnTouchMoved;
        inputManager.OnFingerBegan -= OnTouchBegan;
        inputManager.OnFingerEnded -= OnTouchEnded;
        buildingUI.OnBuildingSelected -= GetSelectedBuilding;
    }
}

    
