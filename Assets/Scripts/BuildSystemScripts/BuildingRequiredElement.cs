using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BuildingRequiredElement : MonoBehaviour{
	[SerializeField] private Image	slotImage;
	[SerializeField] private Image	itemImage;
	[SerializeField] private Text	itemCost;
	[SerializeField] private Color	greenColor;
	[SerializeField] private Color	redColor;

	[HideInInspector] public bool hasRessources = false;

	public void	Setup(ItemInInventory ressourceRequired){
		itemImage.sprite = ressourceRequired.itemData.visual;
		itemCost.text = ressourceRequired.count.ToString();

		ItemInInventory[]	itemInInventory = Inventory.instance.GetContent().Where(elem => elem.itemData == ressourceRequired.itemData).ToArray();
		int					totalItemQuantity = 0;

		for (int i = 0; i < itemInInventory.Length; i++){
			totalItemQuantity += itemInInventory[i].count;
		}
		if (totalItemQuantity >= ressourceRequired.count){
			hasRessources = true;
			slotImage.color = greenColor;
		} else {
			slotImage.color = redColor;
		}
	}
}
