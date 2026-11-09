using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingModel : MonoBehaviour
{
    [SerializeField] private GameObject wrapper;
    [SerializeField] private BuildingData buildingData;

    [SerializeField] private GridManager grid;
    public BuildingData Data => buildingData;
    public float rotation;
    private BuildingShapeUnit[] shapeUnits;

    private Renderer[] renderers;
    private Material[][] originalMaterials;
    private Material previewMaterial;
    private BuildingStorage storage;
    private ResourceProduction production;
    private bool isPreview;
    private bool isPlaced;

    public void Awake() //Alle wichtigen Compoonents suchen und zuweisen
    {
        shapeUnits = GetComponentsInChildren<BuildingShapeUnit>();
        renderers = GetComponentsInChildren<Renderer>(true);
        originalMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].sharedMaterials;
        }

        storage = GetComponent<BuildingStorage>();
        production = GetComponent<ResourceProduction>();

       //Erst nach Registrierung starten
        if (storage != null)
        {
            storage.enabled = false;
        }
        if (production != null)
        {
            production.enabled = false;
        }
    }

    private void Start() 
    {
        if (isPreview || isPlaced)
        {
            return;
        }

        if (buildingData == null)
        {
            Debug.LogError($"Gebaeude '{name}': BuildingData fehlt.", this);
            return;
        }

        if (grid == null)
        {
            var grids = FindObjectsByType<GridManager>()
                .Where(candidate => candidate.isActiveAndEnabled && candidate.gameObject.scene == gameObject.scene)
                .ToArray();
            if (grids.Length != 1)
            {
                Debug.LogError($"Gebaeude '{name}': Kein eindeutiges aktives Grid gefunden. Bitte Grid im Inspector zuweisen.", this);
                return;
            }
            grid = grids[0];
        }

        // Das Grid wird in Awake vorbereitet, bevor feste Gebäude Start ausführen.
        if (!CompletePlacement(grid))
        {
            Debug.LogError($"Gebaeude '{name}': Registrierung in Grid '{grid.name}' fehlgeschlagen. Grid muss bereit sein und alle Grundflaechenzellen muessen frei und bebaubar sein. Lager und Produktion bleiben deaktiviert.", this);
        }
    }

    public void BeginPreview() //Kennzeichnet das Preview als Spilervorschau
    {
        if (!isPlaced)
        {
            isPreview = true;
        }
    }

    public void SetPreviewMaterial(Material material) //Setzt Grün oder Rot für die Vorschau. Für jeden Renderer wird ein Array erstellt mit der ursprünglichen Anzahl an Materials. Dann wird jedes Material im Array mit dem Vorschau Material ersetzt. 
    {
        if (!isPreview || previewMaterial == material)
        {
            return;
        }

        previewMaterial = material;
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = new Material[originalMaterials[i].Length];
            for (int slot = 0; slot < materials.Length; slot++)
            {
                materials[slot] = material;
            }

            // Nur die Zuweisungen wechseln; die Material-Assets bleiben unveraendert.
            renderers[i].sharedMaterials = materials;
        }
    }

    public bool CompletePlacement(GridManager targetGrid)
    {
        if (isPlaced || buildingData == null || targetGrid == null)
        {
            return false;
        }

        // Erst die gesamte Grundflaeche pruefen und registrieren, dann Gameplay starten.
        if (!targetGrid.TrySetBuilding(this, GetAllBuildingPositions()))
        {
            return false;
        }

        grid = targetGrid;
        isPlaced = true;
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sharedMaterials = originalMaterials[i];
        }
        isPreview = false;
        previewMaterial = null;

        if (storage != null)
        {
            storage.Setup(buildingData);
            storage.enabled = true;
        }
        if (production != null && storage != null)
        {
            production.Setup(buildingData);
            production.enabled = true;
        }
        return true;
    }

    public void Rotate(float rotationStep)
    {
        wrapper.transform.Rotate(new Vector3(0, rotationStep, 0));
    }

    public List<Vector3> GetAllBuildingPositions()
    {
        return shapeUnits.Select(unit => unit.transform.position).ToList();
    }
}
