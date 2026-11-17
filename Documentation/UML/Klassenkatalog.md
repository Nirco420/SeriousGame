# Klassen und Originalsignaturen

Stand: 16. November 2026. Die Angaben stammen aus den gespeicherten C#-Dateien. `+`/`-` in den Diagrammen bedeuten öffentlich/privat; Properties mit privatem Setter sind hier in ihrer Originaldeklaration dokumentiert. Initialisierer sind nur hier erhalten.

## ProjectFolderColors

Editor-only: färbt Ordnersymbole anhand ihrer Projektpfade.

Quelle: [Assets/_Project/Editor/Tools/ProjectFolderColors.cs](../../Assets/_Project/Editor/Tools/ProjectFolderColors.cs) · Zeile 10 · statische Klasse

```csharp
private static readonly Dictionary<string, Color?> ColorCache = new Dictionary<string, Color?>();
private static readonly KeyValuePair<string, Color>[] Rules =
static ProjectFolderColors()
private static KeyValuePair<string, Color> Rule(string path, Color color)
private static void ClearCache()
private static Color? GetFolderColor(string guid)
private static void DrawFolder(string guid, Rect selectionRect)
```

## BuildingModel

Hält Gebäudedaten und Form-Marker, wechselt Vorschau-Materialien und initialisiert Produktion nach erfolgreicher Registrierung.

Quelle: [Assets/_Project/Scripts/Buildings/Placement/BuildingModel.cs](../../Assets/_Project/Scripts/Buildings/Placement/BuildingModel.cs) · Zeile 4 · MonoBehaviour

```csharp
private GameObject wrapper;
private BuildingData buildingData;
public BuildingData Data
public float rotation;
private BuildingShapeUnit[] shapeUnits;
private Renderer[] renderers;
private Material[][] originalMaterials;
private Material previewMaterial;
private ResourceProduction production;
private bool isPreview;
private bool isPlaced;
public void Awake()
public void BeginPreview()
public void SetPreviewMaterial(Material material)
public bool CompletePlacement(GridManager targetGrid, SegmentStorage segmentStorage)
public void Rotate(float rotationStep)
public List<Vector3> GetAllBuildingPositions()
```

## BuildingPlacement

Erzeugt genau eine Vorschauinstanz, verschiebt/rotiert sie und bestätigt die Platzierung nach Grid- und Kostenprüfung.

Quelle: [Assets/_Project/Scripts/Buildings/Placement/BuildingPlacement.cs](../../Assets/_Project/Scripts/Buildings/Placement/BuildingPlacement.cs) · Zeile 5 · MonoBehaviour

```csharp
private Camera mainCamera;
private InputManager inputManager;
private BuildingUI buildingUI;
private Material positive;
private Material negative;
private MainStorrage mainStorrage;
private SegmentRotation segmentRotation;
private GameObject placementPanel;
private Button confirmButton;
private Button cancelButton;
private Button rotateButton;
private BuildingModel currentPreview;
private ResourceData buildingResource;
private bool isDragging;
private GridManager grid;
private MeshCollider meshCollider;
private SegmentStorage segmentStorage;
public bool isPlacing => currentPreview != null;
public void Start()
private void OnEnable()
private void Update()
private void CanclePlacement()
private void RotatePreview()
public void OnTouchBegan(Vector2 pos)
public void OnTouchMoved(Vector2 pos)
public void OnTouchEnded(Vector2 pos)
public void GetSelectedBuilding(BuildingModel buildingPrefab)
public void PlaceBuilding()
private void ClearPreview()
private void OnDisable()
```

## BuildingShapeUnit

Leere Marker-Komponente; ihre Transform-Position liefert eine Zelle der Gebäudegrundfläche.

Quelle: [Assets/_Project/Scripts/Buildings/Placement/BuildingShapeUnit.cs](../../Assets/_Project/Scripts/Buildings/Placement/BuildingShapeUnit.cs) · Zeile 5 · MonoBehaviour

Keine eigenen Felder oder Methoden. Die geerbte Unity-Transform-Referenz wird als Markerposition verwendet.

## BuildingStartPlacement

Richtet Startgebäude per Editor-Kontextmenü am Grid aus und registriert sie in Start().

Quelle: [Assets/_Project/Scripts/Buildings/Placement/BuildingStartPlacement.cs](../../Assets/_Project/Scripts/Buildings/Placement/BuildingStartPlacement.cs) · Zeile 3 · MonoBehaviour

```csharp
private BuildingModel building;
private GridManager grid;
private SegmentStorage segmentStorage;
private void SnapToGrid()
void Start()
```

## MainStorrage

Hält globale Bestände und prüft/addiert/entnimmt Ressourcen nach ResourceType.

Quelle: [Assets/_Project/Scripts/Buildings/Resources/MainStorrage.cs](../../Assets/_Project/Scripts/Buildings/Resources/MainStorrage.cs) · Zeile 4 · MonoBehaviour

```csharp
public int wood {  get; private set; }
public int wheed { get; private set; }
public int flachs { get; private set; }
public int gold {  get; private set; }
private int startWood;
private int startWheed;
private int startFlachs;
private int startGold;
public void Start()
public void AddResources(int amount, ResourceData resource)
public void RemoveResource(int amount, ResourceData resource)
public bool HasResources(int amount, ResourceData resource)
```

## ResourceProduction

Übernimmt Produktionsmenge und Ressource aus BuildingData; liefert pro Aufruf eine Menge an sein Segmentlager.

Quelle: [Assets/_Project/Scripts/Buildings/Resources/ResourceProduction.cs](../../Assets/_Project/Scripts/Buildings/Resources/ResourceProduction.cs) · Zeile 3 · MonoBehaviour

```csharp
private SegmentStorage producerForStorage;
private int productionQuantity;
private ResourceData producedResource;
private ResourceDisplayUI displayUI;
public void Setup(BuildingData buildingData, SegmentStorage segmentStorage)
public void ProduceResource()
public void OnDestroy()
```

## SegmentStorage

Speichert die Menge einer Ressource, registrierte Produzenten und die Anzahl zugewiesener Wagen.

Quelle: [Assets/_Project/Scripts/Buildings/Resources/SegmentStorage.cs](../../Assets/_Project/Scripts/Buildings/Resources/SegmentStorage.cs) · Zeile 4 · MonoBehaviour

```csharp
private BuildingData buildingData;
public ResourceData storedResource {  get; private set; }
private List<ResourceProduction> resourceBuildings = new List<ResourceProduction>();
public int currentAmount {  get; private set; }
public int maxCapacity { get; private set; }
public int assignedCarts { get; private set; }
public void Awake()
public void AddResources(int amount)
public int RemoveResources(int amount)
public void RegisterProducer(ResourceProduction productionBuilding)
public void RemoveProducer(ResourceProduction productionBuilding)
public void ProducePerRound()
public bool AssignCart()
public bool RemoveCarts()
```

## DemendManger

Verwaltet Forderungsreihenfolge und eine aktive Forderung, Abgaben, Frist, Aufschub und Barometer-Veränderungen.

Quelle: [Assets/_Project/Scripts/Core/Managers/DemendManger.cs](../../Assets/_Project/Scripts/Core/Managers/DemendManger.cs) · Zeile 8 · MonoBehaviour

```csharp
public event Action<DemandData, int> OnDemandDisplayRequested;
public event Action OnDemandCompleted;
private List<DemandData> demands = new List<DemandData>();
private MainStorrage mainStorrage;
private BarometerManager barometerManager;
private DemandData currentDemend;
private int nextDemandIndex;
private int dueRound;
private int latestRound;
public void TriggerDemand(DemandData demand, int currentRound)
public void CheckDemandsForRound(int currentRound)
public void GiveAll()
public void GiveHalf()
public void MoreTime()
public bool HasActiveDemand()
public bool CanGiveAll()
public bool CanGiveHalf()
private void CompleteDemand()
public int GetRemainingRounds()
```

## RoundManager

Startet Transporte, lässt Lager produzieren, erhöht die Runde und prüft Forderungen.

Quelle: [Assets/_Project/Scripts/Core/Managers/RoundManager.cs](../../Assets/_Project/Scripts/Core/Managers/RoundManager.cs) · Zeile 5 · MonoBehaviour

```csharp
private BuildingUI ui;
private List<SegmentStorage> storages = new List<SegmentStorage>();
private DemendManger demendManger;
public int currentRound {  get; private set; }
private List<TransportManager> transports = new List<TransportManager>();
private void OnEnable()
public void StartNextRound(bool isRoundFinished)
private void OnDisable()
```

## Segment

Bündelt Grid, Oberfläche, Lager, erlaubte Ressource und Gebäude-Elternobjekt; kein eigener Ablauf.

Quelle: [Assets/_Project/Scripts/Core/Managers/Segment.cs](../../Assets/_Project/Scripts/Core/Managers/Segment.cs) · Zeile 3 · MonoBehaviour

```csharp
public GridManager grid;
public MeshCollider surface;
public SegmentStorage segmentStorage;
public ResourceData allowedResources;
public Transform buildingParents;
```

## SegmentRotation

Verarbeitet Swipes, rotiert den gemeinsamen Pivot und bestimmt das aktive Segment; schließt das Baumenü.

Quelle: [Assets/_Project/Scripts/Core/Managers/SegmentRotation.cs](../../Assets/_Project/Scripts/Core/Managers/SegmentRotation.cs) · Zeile 4 · MonoBehaviour

```csharp
private InputManager inputManager;
private float rotationStep = 120f;
private float rotationDuration = 0.4f;
private BuildingPlacement buildingPlacement;
private GameObject rotationPivot;
private BuildingUI buildingUI;
private Segment[] segments;
private int currentSegmentIndex;
public Segment currentSegment => segments[currentSegmentIndex];
public bool isRotating;
private void OnEnable()
private void RotateOnSwipe (int amount)
private IEnumerator Rotate(int amount)
private void OnDisable()
```

## GridManager

Erzeugt lokale Grid-Zellen, prüft die Segmentoberfläche und registriert Gebäude nur auf freien bebaubaren Zellen.

Quelle: [Assets/_Project/Scripts/Grid/Systems/GridManager.cs](../../Assets/_Project/Scripts/Grid/Systems/GridManager.cs) · Zeile 4 · MonoBehaviour

```csharp
private int width;
private int height;
private float cellSize = 1f;
private MeshCollider meshCollider;
private BuildingGridCell[,] grid;
public bool IsReady { get; private set; }
private void Awake()
public Vector3 GridToWorldPosition(int x, int z)
public (int x, int z) WorldToGridPosition(Vector3 worldPos)
public bool TrySetBuilding(BuildingModel building, List<Vector3> allBuildingPositions)
public bool CanBuild(List<Vector3> allBuildingPositions)
public void OnDrawGizmos()
public bool IsCellOnCollider(int x, int z)
public Vector3 GridToWorldCenterPosition(int x, int z)
```

## BuildingGridCell

Gewöhnliches C#-Objekt für Bebaubarkeit und die optionale Gebäudereferenz einer Zelle.

Quelle: [Assets/_Project/Scripts/Grid/Systems/GridManager.cs](../../Assets/_Project/Scripts/Grid/Systems/GridManager.cs) · Zeile 144 · C#-Klasse

```csharp
private BuildingModel building;
private bool isBuildable;
public void SetBuilding(BuildingModel building)
public void SetBuildable(bool isBuild)
public bool GetBuildable()
public bool IsEmpty()
```

## InputManager

Liest EnhancedTouch und veröffentlicht Finger-, Tap- und Swipe-Events.

Quelle: [Assets/_Project/Scripts/Input/InputManager.cs](../../Assets/_Project/Scripts/Input/InputManager.cs) · Zeile 11 · MonoBehaviour

```csharp
public event Action<Vector2> OnFingerMoved;
public event Action<Vector2> OnFingerBegan;
public event Action<Vector2> OnFingerEnded;
public event Action<int> OnSwipe;
public event Action<Vector2> OnTap;
private float swipeRange = 120f;
void OnEnable()
void Update()
public Vector2 TrackFingerPosition(Touch touch)
public Vector2 TrackFingerStartPosition(Touch touch)
public Vector2 TrackFingerEndPosition(Touch touch)
public void OnDisable()
```

## BuildingData

Gebäudekonfiguration: Baukosten, produzierte Ressource/Menge, Typ und Lagerkapazität.

Quelle: [Assets/_Project/Scripts/ScriptableObjects/BuildingData.cs](../../Assets/_Project/Scripts/ScriptableObjects/BuildingData.cs) · Zeile 4 · ScriptableObject

```csharp
public string description;
public ResourceData costResource;
public int cost;
public BuildingType type;
public ResourceData resource;
public int resourceQuantity;
public int maxStorage;
```

## BuildingData.BuildingType

Verschachtelter Enum: BuildingData.

Quelle: [Assets/_Project/Scripts/ScriptableObjects/BuildingData.cs](../../Assets/_Project/Scripts/ScriptableObjects/BuildingData.cs) · Zeile 6 · Enum

```csharp
None,
STORAGE,
PRODUKTION,
RESOURECES
```

## DemandData

Forderungsdaten: Text, Ressource/Menge, früheste Runde, Zeitlimit, Aufschub und vier Barometer-Veränderungen.

Quelle: [Assets/_Project/Scripts/ScriptableObjects/DemandData.cs](../../Assets/_Project/Scripts/ScriptableObjects/DemandData.cs) · Zeile 4 · ScriptableObject

```csharp
public string name;
public string description;
public ResourceData resource;
public int quantity;
public int triggerRound;
public int deadline;
public int deferralPeriod;
public int changeInValuePositive;
public int changeInValueHalf;
public int changeInValueNegative;
public int changeInDeferalInValue;
```

## ResourceData

Ressourcentyp, Anzeigename und Sprite.

Quelle: [Assets/_Project/Scripts/ScriptableObjects/ResourceData.cs](../../Assets/_Project/Scripts/ScriptableObjects/ResourceData.cs) · Zeile 5 · ScriptableObject

```csharp
public ResourceType resourceType;
public string name;
public Sprite icon;
```

## ResourceData.ResourceType

Verschachtelter Enum: ResourceData.

Quelle: [Assets/_Project/Scripts/ScriptableObjects/ResourceData.cs](../../Assets/_Project/Scripts/ScriptableObjects/ResourceData.cs) · Zeile 7 · Enum

```csharp
None,
WOOD,
WHEED,
FLACHS
```

## CartManager

Verwaltet den globalen Wagenvorrat und verändert die Wagenanzahl eines übergebenen Segmentlagers.

Quelle: [Assets/_Project/Scripts/Transport/Logistics/CartManager.cs](../../Assets/_Project/Scripts/Transport/Logistics/CartManager.cs) · Zeile 3 · MonoBehaviour

```csharp
private int totalCarts = 2;
public int TotalCarts => totalCarts;
public int availableCarts { get; private set; }
private void Awake()
public bool AssignCart(SegmentStorage storage)
public bool RemoveCart(SegmentStorage storage)
```

## TransportManager

Transportiert die vor Produktionsbeginn entnommene Ladung entlang einer Spline; schreibt sie bei Ankunft dem Hauptlager gut.

Quelle: [Assets/_Project/Scripts/Transport/Vehicles/TransportManager.cs](../../Assets/_Project/Scripts/Transport/Vehicles/TransportManager.cs) · Zeile 8 · MonoBehaviour

```csharp
private SegmentStorage segmentStorage;
private MainStorrage mainStorage;
private Transform startStoragPos;
private Transform mainStoragePos;
private GameObject cart;
private SplineAnimate splineAnimate;
private ResourceDisplayUI resourceDisplayUI;
private int transportCapacity;
public int currentLoad {  get; private set; }
public float moveSpeed;
private bool isDriving = false;
public ResourceData resourceData {  get; private set; }
private void Update()
public void StartTransport()
public IEnumerator MoveToMainStorage(float kmh)
public IEnumerator MoveBackToSegmentStorage(float kmh)
public IEnumerator TranportLifeCycle(Transform startPos, Transform targetPos, float kmh)
```

## BarometerManager

Hält den Wert von -4 bis +4 und bewegt den UI-Kreis per Coroutine; protokolliert Niederlage an den Grenzen.

Quelle: [Assets/_Project/Scripts/UI/BarometerManager.cs](../../Assets/_Project/Scripts/UI/BarometerManager.cs) · Zeile 5 · MonoBehaviour

```csharp
private float cellSize;
private RectTransform circle;
private Vector2 middlePosition;
private float duration = 5f;
private bool isGameOver;
private int currentValue;
public void Awake()
public void UpdateBarometer()
public IEnumerator MoveCircle()
public void ChangeValue(int amount)
```

## BuildingUI

Verwaltet Bauauswahl, segmentabhängiges Ressourcen-Prefab und Kostenanzeige; sendet Auswahl- und Rundenende-Events.

Quelle: [Assets/_Project/Scripts/UI/BuildingUI.cs](../../Assets/_Project/Scripts/UI/BuildingUI.cs) · Zeile 8 · MonoBehaviour

```csharp
public event Action<BuildingModel> OnBuildingSelected;
public event Action<bool> OnNextRoundPressed;
private Button buildingButton;
private GameObject buildingPanel;
private SegmentRotation segmentRotation;
private Button building1;
private Button building2;
private Button building3;
private Button nextRound;
private BuildingModel building1x1;
private BuildingModel building2x2;
private BuildingModel building3x3;
private TextMeshProUGUI text1x1Building;
private TextMeshProUGUI text2x2Building;
private TextMeshProUGUI text3x3Building;
private Image resourceImage1x1;
private Image resourceImage2x2;
private Image resourceImage3x3;
private BuildingModel wheedBuilding;
private BuildingModel woodBuilding;
private BuildingModel flachsBuilding;
public void Start()
public void OnBuildingButtonClicked()
public void BuildingFinished()
public void OnBuilding1Clicked()
public void OnBuilding2Clicked()
public void OnBuilding3Clicked()
public void OnEndRoundWasClicked()
public void ShowCostBuilding(BuildingData buildingData, TextMeshProUGUI text, Image image)
public void RefreshUiButtons()
```

## DemandEntryUI

Zeigt eine Forderung mit Restzeit; bindet die drei Entscheidungsbuttons an den Forderungsmanager.

Quelle: [Assets/_Project/Scripts/UI/DemandEntryUI.cs](../../Assets/_Project/Scripts/UI/DemandEntryUI.cs) · Zeile 6 · MonoBehaviour

```csharp
private TextMeshProUGUI demandResource;
private TextMeshProUGUI demandTitle;
private TextMeshProUGUI quanatityText;
private TextMeshProUGUI remainingRoundsLeft;
private Button giveAll;
private Button giveHalf;
private Button aufschub;
private DemendManger demandManager;
public void Setup(DemandData currentDemand, int remainingRounds, DemendManger manager)
private void Update()
```

## DemendUI

Zeigt das erste Forderungs-Popup und erzeugt/entfernt den dynamischen Übersichtseintrag anhand von Events.

Quelle: [Assets/_Project/Scripts/UI/DemendUI.cs](../../Assets/_Project/Scripts/UI/DemendUI.cs) · Zeile 5 · MonoBehaviour

```csharp
private DemendManger demendManger;
private GameObject demandPanel;
private TextMeshProUGUI headlineText;
private TextMeshProUGUI descriptionText;
private TextMeshProUGUI resourceName;
private TextMeshProUGUI demandQuantity;
private TextMeshProUGUI demandDeadline;
private Button closeButton;
private Button openDemandEntry;
private DemandEntryUI demandEntryUI;
private Transform content;
private GameObject redInfo;
private GameObject demandOverview;
private DemandData demandData;
private ResourceData resourceData;
private DemandEntryUI demandEntry;
private bool currentState = false;
private void OnEnable()
public void Start()
public void DemandPopUP(DemandData currentDemand, int currentRound)
public void ClosePopUp()
public void OpenDemandOverview()
private void RemoveDemandEntry()
private void OnDisable()
```

## ResourceDisplayUI

Setzt Ressourcen-Icon und Mengenanzeige für Produzent oder Wagen.

Quelle: [Assets/_Project/Scripts/UI/ResourceDisplayUI.cs](../../Assets/_Project/Scripts/UI/ResourceDisplayUI.cs) · Zeile 5 · MonoBehaviour

```csharp
private Image resourceIcon;
private TextMeshProUGUI quantity;
public void SetResource(ResourceData resource)
public void SetQuantity(int amount)
```

## SegmentStorageUI

Öffnet ein Lager per Tap und erlaubt die Wagenzuweisung über drei Slots.

Quelle: [Assets/_Project/Scripts/UI/SegmentStorageUI.cs](../../Assets/_Project/Scripts/UI/SegmentStorageUI.cs) · Zeile 6 · MonoBehaviour

```csharp
private GameObject segmentStorageUi;
private Image resourceImage;
private TextMeshProUGUI amountText;
private Button closeButton;
private CartManager cartManager;
private Vector3 panelOffset= new Vector3(80f,0f,0f);
private InputManager inputManager;
private Camera mainCamera;
private Button cartSlot1;
private Button cartSlot2;
private Button cartSlot3;
private Sprite slotEmpty;
private Sprite slotFull;
private SegmentStorage segmentStorage;
public void OnEnable()
public void Start()
public void OpenUI(Vector2 pos)
public void CloseUI()
private SegmentStorage GetStorageAtPosition(Vector2 pos)
private void ToggleCart(int slot)
private void RefreshCartButtons()
public void OnDisable()
```

## UIOverview

Liest globale Ressourcen, Rundenzahl und verfügbare Wagen für die Übersicht.

Quelle: [Assets/_Project/Scripts/UI/UIOverview.cs](../../Assets/_Project/Scripts/UI/UIOverview.cs) · Zeile 4 · MonoBehaviour

```csharp
private MainStorrage mainStorrage;
private RoundManager roundManager;
private CartManager cartManager;
private TextMeshProUGUI flachsText;
private TextMeshProUGUI wheedText;
private TextMeshProUGUI woodText;
private TextMeshProUGUI currentRound;
private TextMeshProUGUI availabelCarts;
public void Update()
```

## NewTestScript

Bestehender NUnit-Beispieltest für 2 + 2 = 4.

Quelle: [Assets/_Project/Tests/TestScripts/NewTestScript.cs](../../Assets/_Project/Tests/TestScripts/NewTestScript.cs) · Zeile 6 · C#-Klasse

```csharp
public void TwoPlusTwo()
```

