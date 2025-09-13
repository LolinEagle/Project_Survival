using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Objectives : MonoBehaviour{
	private int					objectives = 0;
	private int					totalItemQuantity;
	private ItemInInventory[]	itemInInventory;

	[SerializeField] private Text				objectiveText;
	[SerializeField] private Equipment			equipment;
	[SerializeField] private AttackBehaviour	attackBehaviour;

	[SerializeField] private ItemData	woodLog;
	[SerializeField] private ItemData	stone;
	[SerializeField] private ItemData	ironSword;

	void	Start(){
		objectiveText.text = "Objective: Collect a wood log";
	}

	bool	HaveItem(ItemData itemData, int quantity){
		itemInInventory = Inventory.instance.GetContent().Where(elem => elem.itemData == itemData).ToArray();
		totalItemQuantity = 0;
		for (int i = 0; i < itemInInventory.Length; i++){
			totalItemQuantity += itemInInventory[i].count;
		}
		if(totalItemQuantity >= quantity){
			objectives++;
			return true;
		}
		return false;
	}

	void	Update(){
		switch(objectives){
			case 0:
				if (HaveItem(woodLog, 1)) objectiveText.text = "Objective: Collect 2 stones";
				break;
			case 1:
				if (HaveItem(stone, 2)) objectiveText.text = "Objective: Craft and equip a sword";
				break;
			case 2:
				if (equipment.equipedWeaponItem == ironSword){
					objectiveText.text = "Objective: Kill a bear";
					objectives++;
				}
				break;
			case 3:
				if (attackBehaviour.numBeersKilled >= 1){
					objectives++;
				}
				break;
			case 4:
				objectiveText.text = "You have killed " + attackBehaviour.numBeersKilled + " bears";
				break;
		}
	}
}
