using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SegmentStorageUI : MonoBehaviour
{
    [SerializeField] private GameObject segmentStorageUi;
    [SerializeField] private Image resourceImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Button closeButton;
    [SerializeField] private CartManager cartManager;
    [SerializeField] private Vector3 panelOffset= new Vector3(80f,0f,0f);
    

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Camera mainCamera;


    [SerializeField] private Button cartSlot1;
    [SerializeField] private Button cartSlot2;
    [SerializeField] private Button cartSlot3;

    [SerializeField] private Sprite slotEmpty;
    [SerializeField] private Sprite slotFull;

    private SegmentStorage segmentStorage;

    public void OnEnable()
    {
        inputManager.OnTap += OpenUI;
    }

    public void Start()
    {
        closeButton.onClick.AddListener(CloseUI);
        cartSlot1.onClick.AddListener(() => ToggleCart(1));
        cartSlot2.onClick.AddListener(() => ToggleCart(2));
        cartSlot3.onClick.AddListener(() => ToggleCart(3));
    }

    public void OpenUI(Vector2 pos) //Öffnet die UI und Instanziert die Werte
    {
        SegmentStorage storage = GetStorageAtPosition(pos);

        if (storage == null)
        {
            return;
        }

        segmentStorage = storage;


        resourceImage.sprite = segmentStorage.storedResource.icon;
        amountText.text = segmentStorage.currentAmount + " / " + segmentStorage.maxCapacity;

        Vector3 position = mainCamera.WorldToScreenPoint(segmentStorage.transform.position);
        float canvasScale = segmentStorageUi.GetComponentInParent<Canvas>().rootCanvas.scaleFactor;
        segmentStorageUi.transform.position = position + new Vector3(panelOffset.x * canvasScale, panelOffset.y * canvasScale, -position.z);

        RefreshCartButtons();
        segmentStorageUi.SetActive(true);
        PositionPanel();
    }

    public void CloseUI() //Schliest die UI wieder
    {
        segmentStorageUi.SetActive(false);
    }

    private SegmentStorage GetStorageAtPosition(Vector2 pos) //prüft ob ein Segmentlager getroffen wurde
    {
        Ray ray = mainCamera.ScreenPointToRay(pos);

        if(Physics.Raycast(ray, out RaycastHit hitInfo) == true)
        {
            SegmentStorage storage = hitInfo.collider.GetComponentInParent<SegmentStorage>();
            return storage;
        }
        return null;
    }

    private void ToggleCart(int slot) 
    {
        if (slot <= segmentStorage.assignedCarts)
        {
            cartManager.RemoveCart(segmentStorage);
        }
        else
        {
            cartManager.AssignCart(segmentStorage);
        }

        RefreshCartButtons();
    }

    private void RefreshCartButtons()
    {
        int count = segmentStorage.assignedCarts;

        cartSlot1.GetComponentInChildren<Image>().sprite = count >= 1 ? slotFull : slotEmpty;
        cartSlot2.GetComponentInChildren<Image>().sprite = count >= 2 ? slotFull : slotEmpty;
        cartSlot3.GetComponentInChildren<Image>().sprite = count >= 3 ? slotFull: slotEmpty;

    }

    private void PositionPanel()
    {
        RectTransform panel = (RectTransform)segmentStorageUi.transform;
        float canvasScale = panel.GetComponentInParent<Canvas>().rootCanvas.scaleFactor;

        Vector2 buildingPosition = mainCamera.WorldToScreenPoint(segmentStorage.transform.position);
        Vector2 halfSize = new Vector2(
            panel.rect.width * panel.lossyScale.x / 2f,
            panel.rect.height * panel.lossyScale.y / 2f);

        float gap = 0f * canvasScale;

        Rect area = Screen.safeArea;
        area.xMin += 10f;
        area.xMax -= 10f;
        area.yMin += 10f;
        area.yMax -= 10f;

        Vector2[] positions =
        {
        buildingPosition + new Vector2(halfSize.x + gap, 0f),
        buildingPosition - new Vector2(halfSize.x + gap, 0f),
        buildingPosition + new Vector2(0f, halfSize.y + gap),
        buildingPosition - new Vector2(0f, halfSize.y + gap)
    };

        foreach (Vector2 position in positions)
        {
            if (area.Contains(position - halfSize) &&
                area.Contains(position + halfSize))
            {
                panel.position = new Vector3(position.x, position.y, 0f);
                return;
            }
        }

        Vector2 fallback = positions[0];
        fallback.x = Mathf.Clamp(fallback.x, area.xMin + halfSize.x, area.xMax - halfSize.x);
        fallback.y = Mathf.Clamp(fallback.y, area.yMin + halfSize.y, area.yMax - halfSize.y);

        panel.position = new Vector3(fallback.x, fallback.y, 0f);
    }
    public void OnDisable()
    {
        inputManager.OnTap -= OpenUI;
    }


}
