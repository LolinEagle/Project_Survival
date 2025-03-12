using UnityEngine;

public class ItemActionSystem : MonoBehaviour{
	[SerializeField] private Equipment		equipment;
	[SerializeField] private PlayerStats	playerStats;

	[Header("Action Panel References")]
	[SerializeField] private Transform	dropPoint;
	[SerializeField] private GameObject	useItemButton;
	[SerializeField] private GameObject	equipItemButton;
	[SerializeField] private GameObject	dropItemButton;
	[SerializeField] private GameObject	destroyItemButton;

	[HideInInspector] public ItemData	itemCurrentlySelected;
	public GameObject	actionPanel;

	public void	OpenActionPanel(ItemData item, Vector3 slotPosition){
		itemCurrentlySelected = item;

		if (item == null){
			actionPanel.SetActive(false);
			return ;
		}

		switch (item.itemType){
			case ItemType.Ressource:
				useItemButton.SetActive(false);
				equipItemButton.SetActive(false);
				break;
			case ItemType.Equipment:
				useItemButton.SetActive(false);
				equipItemButton.SetActive(true);
				break;
			case ItemType.Consumable:
				useItemButton.SetActive(true);
				equipItemButton.SetActive(false);
				break;
		}

		actionPanel.transform.position = slotPosition;
		actionPanel.SetActive(true);
	}

	public void	CloseActionPanel(){
		actionPanel.SetActive(false);
		itemCurrentlySelected = null;
	}

	public void	UseActionButton(){
		playerStats.Consumeitem(itemCurrentlySelected.healthEffect, itemCurrentlySelected.hungerEffect, itemCurrentlySelected.thirsEffect);
		Inventory.instance.RemoveItem(itemCurrentlySelected);
		CloseActionPanel();
	}

	public void EquipActionButton(){
		equipment.EquipAction();
	}

	public void	DropActionButton(){
		GameObject	instantiatedItem = Instantiate(itemCurrentlySelected.prefab);
		instantiatedItem.transform.position = dropPoint.position;
		DestroyActionButton();
	}

	public void	DestroyActionButton(){
		Inventory.instance.RemoveItem(itemCurrentlySelected);
		Inventory.instance.RefreshContent();
		CloseActionPanel();
	}
}
