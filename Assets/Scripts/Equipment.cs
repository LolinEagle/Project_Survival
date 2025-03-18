using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour{
	[SerializeField] private ItemActionSystem	itemActionSystem;
	[SerializeField] private PlayerStats		playerStats;
	[SerializeField] private EquipmentLibary	equipmentLibrary;
	[SerializeField] private AudioSource		audioSource;
	[SerializeField] private AudioClip			equipSound;

	[Header("Equipment Image References")]
	[SerializeField] private Image	headSlotImage;
	[SerializeField] private Image	chestSlotImage;
	[SerializeField] private Image	handsSlotImage;
	[SerializeField] private Image	legsSlotImage;
	[SerializeField] private Image	feetSlotImage;
	[SerializeField] private Image	weaponSlotImage;

	[Header("Equipment Button References")]
	[SerializeField] private Button	headSlotDesequipButton;
	[SerializeField] private Button	chestSlotDesequipButton;
	[SerializeField] private Button	handsSlotDesequipButton;
	[SerializeField] private Button	legsSlotDesequipButton;
	[SerializeField] private Button	feetSlotDesequipButton;
	[SerializeField] private Button	weaponSlotDesequipButton;

	private ItemData	equipedHeadItem;
	private ItemData	equipedChestItem;
	private ItemData	equipedHandsItem;
	private ItemData	equipedLegsItem;
	private ItemData	equipedFeetItem;
	[HideInInspector] public ItemData	equipedWeaponItem;

	public void		EquipAction(){
		EquipmentLibraryItem	equipmentLibraryItem = equipmentLibrary.content.Where(elem => elem.itemData == itemActionSystem.itemCurrentlySelected).First();
		if (equipmentLibraryItem != null){
			switch (itemActionSystem.itemCurrentlySelected.equipmentType){
				case EquipmentType.Head:
					DisablePreviousEquipedEquipment(equipedHeadItem);
					headSlotImage.sprite = itemActionSystem.itemCurrentlySelected.visual;
					equipedHeadItem = itemActionSystem.itemCurrentlySelected;
					break;
				case EquipmentType.Chest:
					DisablePreviousEquipedEquipment(equipedChestItem);
					chestSlotImage.sprite = itemActionSystem.itemCurrentlySelected.visual;
					equipedChestItem = itemActionSystem.itemCurrentlySelected;
					break;
				case EquipmentType.Gloves:
					DisablePreviousEquipedEquipment(equipedHandsItem);
					handsSlotImage.sprite = itemActionSystem.itemCurrentlySelected.visual;
					equipedHandsItem = itemActionSystem.itemCurrentlySelected;
					break;
				case EquipmentType.Legs:
					DisablePreviousEquipedEquipment(equipedLegsItem);
					legsSlotImage.sprite = itemActionSystem.itemCurrentlySelected.visual;
					equipedLegsItem = itemActionSystem.itemCurrentlySelected;
					break;
				case EquipmentType.Feet:
					DisablePreviousEquipedEquipment(equipedFeetItem);
					feetSlotImage.sprite = itemActionSystem.itemCurrentlySelected.visual;
					equipedFeetItem = itemActionSystem.itemCurrentlySelected;
					break;
				case EquipmentType.Weapon:
					DisablePreviousEquipedEquipment(equipedWeaponItem);
					weaponSlotImage.sprite = itemActionSystem.itemCurrentlySelected.visual;
					equipedWeaponItem = itemActionSystem.itemCurrentlySelected;
					break;
			}
			
			for (int i = 0; i < equipmentLibraryItem.elementsToDisable.Length; i++){
				equipmentLibraryItem.elementsToDisable[i].SetActive(false);
			}
			equipmentLibraryItem.itemPrefab.SetActive(true);
			playerStats.currentArmorPoints += itemActionSystem.itemCurrentlySelected.armorPoint;
			Inventory.instance.RemoveItem(itemActionSystem.itemCurrentlySelected);
			audioSource.PlayOneShot(equipSound);
		} else {
			Debug.LogError("Equipment : " + itemActionSystem.itemCurrentlySelected.name + " non-existent");
		}
		itemActionSystem.CloseActionPanel();
	}

	public void		UpdateEquipmentsDesequipButtons(){
		headSlotDesequipButton.onClick.RemoveAllListeners();
		chestSlotDesequipButton.onClick.RemoveAllListeners();
		handsSlotDesequipButton.onClick.RemoveAllListeners();
		legsSlotDesequipButton.onClick.RemoveAllListeners();
		feetSlotDesequipButton.onClick.RemoveAllListeners();
		weaponSlotDesequipButton.onClick.RemoveAllListeners();

		headSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Head); });
		chestSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Chest); });
		handsSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Gloves); });
		legsSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Legs); });
		feetSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Feet); });
		weaponSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Weapon); });

		headSlotDesequipButton.gameObject.SetActive(equipedHeadItem);
		chestSlotDesequipButton.gameObject.SetActive(equipedChestItem);
		handsSlotDesequipButton.gameObject.SetActive(equipedHandsItem);
		legsSlotDesequipButton.gameObject.SetActive(equipedLegsItem);
		feetSlotDesequipButton.gameObject.SetActive(equipedFeetItem);
		weaponSlotDesequipButton.gameObject.SetActive(equipedWeaponItem);
	}

	public void		DesequipEquipment(EquipmentType equipmentType){
		if (Inventory.instance.IsFull()){
			return;
		}

		ItemData	currentItem = null;

		switch (equipmentType){
			case EquipmentType.Head:
				currentItem = equipedHeadItem;
				equipedHeadItem = null;
				headSlotImage.sprite = Inventory.instance.emptySlotVisual;
				break;
			case EquipmentType.Chest:
				currentItem = equipedChestItem;
				equipedChestItem = null;
				chestSlotImage.sprite = Inventory.instance.emptySlotVisual;
				break;
			case EquipmentType.Gloves:
				currentItem = equipedHandsItem;
				equipedHandsItem = null;
				handsSlotImage.sprite = Inventory.instance.emptySlotVisual;
				break;
			case EquipmentType.Legs:
				currentItem = equipedLegsItem;
				equipedLegsItem = null;
				legsSlotImage.sprite = Inventory.instance.emptySlotVisual;
				break;
			case EquipmentType.Feet:
				currentItem = equipedFeetItem;
				equipedFeetItem = null;
				feetSlotImage.sprite = Inventory.instance.emptySlotVisual;
				break;
			case EquipmentType.Weapon:
				currentItem = equipedWeaponItem;
				equipedWeaponItem = null;
				weaponSlotImage.sprite = Inventory.instance.emptySlotVisual;
				break;
		}

		EquipmentLibraryItem	equipmentLibraryItem = equipmentLibrary.content.Where(elem => elem.itemData == currentItem).First();
		if (equipmentLibraryItem != null){
			for (int i = 0; i < equipmentLibraryItem.elementsToDisable.Length; i++){
				equipmentLibraryItem.elementsToDisable[i].SetActive(true);
			}
			equipmentLibraryItem.itemPrefab.SetActive(false);
		}

		playerStats.currentArmorPoints -= currentItem.armorPoint;
		Inventory.instance.AddItem(currentItem);
		Inventory.instance.RefreshContent();
	}

	private void	DisablePreviousEquipedEquipment(ItemData itemToDiable){
		if (itemToDiable == null){
			return;
		}

		EquipmentLibraryItem	equipmentLibraryItem = equipmentLibrary.content.Where(elem => elem.itemData == itemToDiable).First();
		if (equipmentLibraryItem != null){
			for (int i = 0; i < equipmentLibraryItem.elementsToDisable.Length; i++){
				equipmentLibraryItem.elementsToDisable[i].SetActive(true);
			}
			equipmentLibraryItem.itemPrefab.SetActive(false);
		}

		playerStats.currentArmorPoints -= itemToDiable.armorPoint;
		Inventory.instance.AddItem(itemToDiable);
	}
}
