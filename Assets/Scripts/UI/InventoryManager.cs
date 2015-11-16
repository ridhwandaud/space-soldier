using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class InventoryManager : MonoBehaviour {
    public static float TileSideLength = 50; //tile is a square, so this is both length and width

    public GameObject GenericInventoryTile;
    public int MaxTiles;

    private Queue<InventoryTileInfo> toRender;
    private List<Transform> slotTransforms;
    private bool[] slots;
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
        // temporary code to pause game for testing
        Time.timeScale = 0;

        slots = new bool[MaxTiles];
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
        // sizeDelta == size (for rect transforms)
        tileSize = new Vector2(slotTransform.sizeDelta.x, slotTransform.sizeDelta.y);
    }

    void OnEnable ()
    {
        while(toRender.Count != 0)
        {
            InventoryTileInfo info = toRender.Dequeue();
            int indexOfNewTile = AddToInventorySlot();
            InstantiateNewTile(info, indexOfNewTile);
        }
    }

    public void EnqueueNewSkill(InventoryTileInfo info)
    {
        toRender.Enqueue(info);
    }

    private void InstantiateNewTile(InventoryTileInfo info, int tileIndex)
    {
        Transform slotTransform = slotTransforms[tileIndex];
        GameObject newTile = Instantiate(GenericInventoryTile) as GameObject;

        newTile.transform.SetParent(transform);
        newTile.transform.position = slotTransform.position;
        // Need to set this explicitly or else scale will automatically be set to the wrong value.
        newTile.transform.localScale = new Vector2(1, 1);

        newTile.GetComponent<RectTransform>().sizeDelta = tileSize;
        newTile.GetComponent<InventoryTile>().Init(slotTransform);
        // TODO: set the image and weapon.
    }

    private int AddToInventorySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == false)
            {
                slots[i] = true;
                return i;
            }
        }

        print("something went horribly wrong and your inventory filled up.");
        return -1;
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
