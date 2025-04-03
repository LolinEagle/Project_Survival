using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Recipe : MonoBehaviour{
	private RecipeData	currentRecipe;

	[SerializeField] private Image		craftableItemImage;
	[SerializeField] private GameObject	elementRequiredPrefab;
	[SerializeField] private Transform	elementRequiredParent;
	[SerializeField] private Button		craftButton;
	[SerializeField] private Sprite		canbuildIcon;
	[SerializeField] private Sprite		cantbuildIcon;
	[SerializeField] private Color		missingColor;
	[SerializeField] private Color		availableColor;

	public void Configure(RecipeData recipe){
		bool	canCraft = true;

		currentRecipe = recipe;
		craftableItemImage.sprite = recipe.craftableItem.visual;
		craftableItemImage.transform.parent.GetComponent<Slot>().item = recipe.craftableItem;
		for (int i = 0; i < recipe.requiredItems.Length; i++) {
			GameObject	requiredItemGo = Instantiate(elementRequiredPrefab, elementRequiredParent);
			Image		requiredItemImage = requiredItemGo.GetComponent<Image>();
			ItemData	requiredItem = recipe.requiredItems[i].itemData;

			ElementRequired		elementRequired = requiredItemGo.GetComponent<ElementRequired>();
			ItemInInventory[]	itemInInventory = Inventory.instance.GetContent().Where(elem => elem.itemData == requiredItem).ToArray();
			int					totalRequiredItem = 0;

			for (int j = 0; j < itemInInventory.Length; j++){
				totalRequiredItem += itemInInventory[j].count;
			}
			requiredItemGo.GetComponent<Slot>().item = requiredItem;
			if (totalRequiredItem >= recipe.requiredItems[i].count){
				requiredItemImage.color = availableColor;
			} else {
				requiredItemImage.color = missingColor;
				canCraft = false;
			}
			elementRequired.elementImage.sprite = recipe.requiredItems[i].itemData.visual;
			elementRequired.elementCountText.text = recipe.requiredItems[i].count.ToString();
		}
		craftButton.image.sprite = canCraft ? canbuildIcon : cantbuildIcon;
		craftButton.enabled = canCraft;
		ResizeElmentsRequiredParent();
	}

	private void ResizeElmentsRequiredParent(){
		Canvas.ForceUpdateCanvases();
		elementRequiredParent.GetComponent<ContentSizeFitter>().enabled = false;
		elementRequiredParent.GetComponent<ContentSizeFitter>().enabled = true;
	}

	public void	CraftItem(){
		for (int i = 0; i < currentRecipe.requiredItems.Length; i++){
			for (int j = 0; j < currentRecipe.requiredItems[i].count; j++){
				Inventory.instance.RemoveItem(currentRecipe.requiredItems[i].itemData);
			}
		}
		Inventory.instance.AddItem(currentRecipe.craftableItem);
	}
}
