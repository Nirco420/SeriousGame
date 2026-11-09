using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private BuildingUI buildingUI;
    [SerializeField] private Material positive;
    [SerializeField] private Material negative;
    [SerializeField] private GridManager grid;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private float tapThreshold = 20f;

    private BuildingModel currentPreview;
    private bool isDragging;
    private Vector2 touchStartPosition;
    private bool hasMoved;

    private void OnEnable() //BuildingPlacement als Empfänger hinzufügen
    {
        inputManager.OnFingerMoved += OnTouchMoved;
        inputManager.OnFingerBegan += OnTouchBegan;
        inputManager.OnFingerEnded += OnTouchEnded;
        buildingUI.OnBuildingSelected += GetSelectedBuilding;
    }

    private void Update() //Pro Frame wird geprüft, ob eine Preview existiert, ob sie mind eine ShepUnit hat und alle Zellen darunter bebaubar sind
    {
        if (currentPreview == null)
        {
            return;
        }

        List<Vector3> positions = currentPreview.GetAllBuildingPositions();
        if (grid.CanBuild(positions))
        {
            currentPreview.SetPreviewMaterial(positive);
        }
        else
        {
            currentPreview.SetPreviewMaterial(negative);
        }
    }

    public void OnTouchBegan(Vector2 pos) //Erzeugt einen Ray von der Camera zum Objekt. Wenn einer der Objektcollider getroffen wurde, dann treffer position übergeben
    {
        isDragging = false;
        hasMoved = false;
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
                touchStartPosition = pos;
                break;
            }
        }
    }

    public void OnTouchMoved(Vector2 pos) //Ein Ray von der camera zur Fingerposition erzeugen, wenn der MeshCollider der Baufläche getroffen wird, den Trefferpunkt in GridKoordinaten umrechen. Das Objekt dann auf diese GridKoordianten setzen
    {
        if (!isDragging || currentPreview == null)
        {
            return;
        }

        if (Vector2.Distance(touchStartPosition, pos) > tapThreshold)
        {
            hasMoved = true;
        }

        if (hasMoved == false)
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

    public void OnTouchEnded(Vector2 pos) //Wenn die Bewegungschwelle
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

    public void GetSelectedBuilding(BuildingModel buildingPrefab)//Erzeugt das Gebäude mit den richtigen Gebäudedaten, erstmal nur als Vorschau
    {
        ClearPreview();
        if (buildingPrefab == null || buildingPrefab.Data == null || positive == null || negative == null)
        {
            Debug.LogError("Gebaeudedaten, Modell oder Vorschau-Materialien fehlen.", this);
            return;
        }

        currentPreview = Instantiate(buildingPrefab, transform.position, Quaternion.identity);
        currentPreview.BeginPreview();
    }

    public void PlaceBuilding()//Hier wird das gebäude platziert, es werden nochmal alle Zellen geprüft und wenn diese frei sind, durch das Building belegt. Alle werte werden dann wieder zurückgesetzt
    {
        if (currentPreview == null)
        {
            return;
        }

        // Das Modell registriert sich und verwendet seine eigenen Gebaeudedaten.
        if (!currentPreview.CompletePlacement(grid))
        {
            return;
        }

        // Dieselbe Instanz bleibt bestehen.
        currentPreview = null;
        isDragging = false;
        hasMoved = false;
        buildingUI.BuildingFinished();
    }

    private void ClearPreview() //Entfernt die Vorschau bzw. das Modell bei einem Auswahlswechsel
    {
        if (currentPreview != null)
        {
            // Sofort ausblenden, da Destroy erst am Frame Ende ausgeführt wird.
            currentPreview.gameObject.SetActive(false);
            Destroy(currentPreview.gameObject);
        }

        currentPreview = null;
        isDragging = false;
        hasMoved = false;
    }

    private void OnDisable() //Events wieder abmelden und alle werte resetten
    {
        inputManager.OnFingerMoved -= OnTouchMoved;
        inputManager.OnFingerBegan -= OnTouchBegan;
        inputManager.OnFingerEnded -= OnTouchEnded;
        buildingUI.OnBuildingSelected -= GetSelectedBuilding;
        ClearPreview();
    }
}
