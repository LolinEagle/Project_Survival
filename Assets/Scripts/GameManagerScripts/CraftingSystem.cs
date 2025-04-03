using UnityEngine;

public class CraftingSystem : MonoBehaviour{
	[SerializeField] private RecipeData[]	availableRecipes;
	[SerializeField] private GameObject		recipeUiPrefab;
	[SerializeField] private Transform		recipeParent;
	[SerializeField] private KeyCode		openCraftPanelInput;
	[SerializeField] private GameObject		craftingPanel;

	void Start(){
		UpdateDisplayedRecipes();
	}

	private void Update(){
		if (Input.GetKeyDown(openCraftPanelInput)){
			craftingPanel.SetActive(!craftingPanel.activeSelf);
			UpdateDisplayedRecipes();
		}
	}

	public void UpdateDisplayedRecipes(){
		foreach(Transform child in recipeParent){
			Destroy(child.gameObject);
		}

		for (int i = 0; i < availableRecipes.Length; i++){
			GameObject	recipe = Instantiate(recipeUiPrefab, recipeParent);
			recipe.GetComponent<Recipe>().Configure(availableRecipes[i]);
		}
	}
}
