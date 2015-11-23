using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class InventoryManager : MonoBehaviour {
    public static InventoryManager Instance;
    public static float TileSideLength = 50; //tile is a square, so this is both length and width

    public GameObject GenericInventoryTile;
    public Weapon weapon; // temporary

    private List<RectTransform> slotRects;
    private Queue<InventoryTileInfo> toRender;
    private List<Transform> slotTransforms;
    private Vector2 tileSize;

    private string UnequippedPanelName = "Unequipped";

    void Update ()
    {
        if (Input.GetButtonDown("Jump"))
        {
            EnqueueNewSkill(new InventoryTileInfo());
        }

        if (Input.GetButtonDown("Submit"))
        {
            OnEnable();
        }
    }

    void Awake ()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Init()
    {
        // temporary code to pause game for testing
        //Time.timeScale = 0;

        slotRects = new List<RectTransform>();
        GameObject[] slotObjects = GameObject.FindGameObjectsWithTag("Slot Tile");
        for (int i = 0; i < slotObjects.Length; i++)
        {
            slotRects.Add(slotObjects[i].GetComponent<RectTransform>());
        }

        toRender = new Queue<InventoryTileInfo>();
        slotTransforms = new List<Transform>();
        Transform[] transforms = GameObject.Find(UnequippedPanelName).GetComponentsInChildren<Transform>();
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i].name != UnequippedPanelName)
            {
                slotTransforms.Add(transforms[i]);
            }
        }
        StartCoroutine("ConfigureSize");
    }

    IEnumerator ConfigureSize()
    {
        yield return new WaitForEndOfFrame();

        RectTransform slotTransform = slotTransforms[0].GetComponent<RectTransform>();
        tileSize = new Vector2(slotTransform.sizeDelta.x, slotTransform.sizeDelta.y);
    }

    void OnEnable ()
    {
        while(toRender.Count != 0)
        {
            InventoryTileInfo info = toRender.Dequeue();
            InstantiateNewTile(info);
        }
    }

    public void EnqueueNewSkill(InventoryTileInfo info)
    {
        toRender.Enqueue(info);
    }

    private void InstantiateNewTile(InventoryTileInfo info)
    {
        Transform slotTransform = null;

        for (int i = 0; i < slotTransforms.Count; i++)
        {
            InventorySlot inventorySlot = slotTransforms[i].GetComponent<InventorySlot>();
            if (!inventorySlot.Occupied)
            {
                inventorySlot.Occupied = true;
                slotTransform = slotTransforms[i];
                break;
            }
        }

        if (slotTransform == null)
        {
            print("inventory full.");
            return;
        }

        GameObject newTile = Instantiate(GenericInventoryTile) as GameObject;

        newTile.transform.SetParent(transform);
        newTile.transform.position = slotTransform.position;
        // Need to set this explicitly or else scale will automatically be set to the wrong value.
        newTile.transform.localScale = new Vector2(1, 1);

        newTile.GetComponent<RectTransform>().sizeDelta = tileSize;
        newTile.GetComponent<InventoryTile>().Init(slotTransform, slotRects, weapon);
        // TODO: set the image and weapon.
    }

    public struct InventoryTileInfo
    {
        public Image Image;
        public Weapon Weapon;

        public InventoryTileInfo(Image image, Weapon weapon)
        {
            Image = image;
            Weapon = weapon;
        }
    }
}
