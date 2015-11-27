using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class InventoryManager : MonoBehaviour {
    public static InventoryManager Instance = null;

    public static float TileSideLength = 50; //tile is a square, so this is both length and width

    [SerializeField]
    private GameObject genericInventoryTile;
    [SerializeField]
    private List<Transform> slotTransforms;

    private List<RectTransform> slotRects;
    private Vector2 tileSize = new Vector2(50, 50);
    private bool sizeHasBeenConfigured = false;

    void Awake ()
    {
        if (!Instance)
        {
            Instance = this;
        }

        slotRects = new List<RectTransform>();
        GameObject[] slotObjects = GameObject.FindGameObjectsWithTag("Slot Tile");
        for (int i = 0; i < slotObjects.Length; i++)
        {
            slotRects.Add(slotObjects[i].GetComponent<RectTransform>());
        }
    }

    public void InstantiateNewTile(InventoryTileInfo info)
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
            return;
        }

        GameObject newTile = Instantiate(genericInventoryTile) as GameObject;

        newTile.transform.SetParent(transform);
        newTile.transform.position = slotTransform.position;
        // Need to set this explicitly or else scale will automatically be set to the wrong value.
        newTile.transform.localScale = new Vector2(1, 1);

        newTile.GetComponent<RectTransform>().sizeDelta = tileSize;
        newTile.GetComponent<InventoryTile>().Init(slotTransform, slotRects, info.Weapon);
        // TODO: set the image.
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
