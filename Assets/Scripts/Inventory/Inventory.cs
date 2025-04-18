using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour{
	[SerializeField] private Equipment			equipment;
	[SerializeField] private ItemActionSystem	itemActionSystem;
	[SerializeField] private CraftingSystem		craftingSystem;
	[SerializeField] private BuildSystem		buildSystem;

	[Header("Inventory Panel References")]
	[SerializeField] private List<ItemInInventory>	content = new List<ItemInInventory>();
	[SerializeField] private GameObject				inventoryPanel;
	[SerializeField] private Transform				inventorySlotParent;

	private const uint		InventorySize = 40;
	private bool			isOpen = false;
	public static Inventory	instance;
	public Sprite			emptySlotVisual;

	private void	Awake(){
		instance = this;
	}

	public void		Start(){
		CloseInventory();
		RefreshContent();
	}

	public void		Update(){
		if (Input.GetKeyDown(KeyCode.I)){
			if (isOpen) {
				CloseInventory();
			} else {
				OpenInventory();
			}
		}
	}

	public void		AddItem(ItemData item){
		ItemInInventory[]	itemInInventory = content.Where(elem => elem.itemData == item).ToArray();
		bool				itemAdded = false;

		if (itemInInventory.Length > 0 && item.stackable){
			for (int i = 0; i < itemInInventory.Length; i++){
				if (itemInInventory[i].count < item.maxStack){
					itemAdded = true;
					itemInInventory[i].count++;
					break ;
				}
			}
			if (!itemAdded){
				content.Add(new ItemInInventory{itemData = item, count = 1});
			}
		} else {
			content.Add(new ItemInInventory{itemData = item, count = 1});
		}
		RefreshContent();
	}

	public void		RemoveItem(ItemData item){
		ItemInInventory	itemInInventory = content.Where(elem => elem.itemData == item).FirstOrDefault();
		if (itemInInventory != null && itemInInventory.count > 1){
			itemInInventory.count--;
		} else {
			content.Remove(itemInInventory);
		}
		RefreshContent();
	}

	public List<ItemInInventory>	GetContent(){
		return (content);
	}

	private void	OpenInventory(){
		inventoryPanel.SetActive(true);
		isOpen = true;
	}

	public void		CloseInventory(){
		inventoryPanel.SetActive(false);
		itemActionSystem.actionPanel.SetActive(false);
		TooltipSystem.instance.Hide();
		isOpen = false;
	}

	public void		RefreshContent(){
		for (int i = 0; i < inventorySlotParent.childCount; i++){
			Slot	currentSlot = inventorySlotParent.GetChild(i).GetComponent<Slot>();
			currentSlot.item = null;
			currentSlot.itemVisual.sprite = emptySlotVisual;
			currentSlot.countText.enabled = false;
		}

		for (int i = 0; i < content.Count; i++){
			Slot	currentSlot = inventorySlotParent.GetChild(i).GetComponent<Slot>();
			currentSlot.item = content[i].itemData;
			currentSlot.itemVisual.sprite = content[i].itemData.visual;
			if (currentSlot.item.stackable){
				currentSlot.countText.enabled = true;
				currentSlot.countText.text = content[i].count.ToString();
			}
		}

		equipment.UpdateEquipmentsDesequipButtons();
		craftingSystem.UpdateDisplayedRecipes();
		buildSystem.UpdateDisplayCosts();
	}

	public bool		IsFull(){
		return (InventorySize <= content.Count);
	}

	public void		LoadData(List<ItemInInventory> savedData){
		content = savedData;
		RefreshContent();
	}

	public void		ClearInventory(){
		content.Clear();
	}
}

[System.Serializable]
public class ItemInInventory{
	public ItemData itemData;
	public int		count;
}
