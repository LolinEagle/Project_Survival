using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour{
	[Header("References")]
	[SerializeField] private Transform		playerTransform;
	[SerializeField] private Equipment		equipment;
	[SerializeField] private PlayerStats	playerStats;
	[SerializeField] private BuildSystem	buildSystem;
	[SerializeField] private MainMenu		mainMenu;
	[SerializeField] private PauseMenu		pauseMenu;

	[Header("SaveSystem Settings")]
	[SerializeField] private bool			SaveSystemActive;

	private string	filePath;

	void	Start(){
		if (MainMenu.loadSavedData){
			LoadData();
		}
	}

	void	Update(){
		if (Input.GetKeyDown(KeyCode.F5)){
			SaveData();
		} else if (Input.GetKeyDown(KeyCode.F6)){
			LoadData();
		}
	}

	public void	SaveData(){
		if (!SaveSystemActive) return;

		// Save all the data
		SavedData	savedData = new SavedData{
			playerPosition = playerTransform.position,
			inventory = Inventory.instance.GetContent(),

			equipedHeadItem = equipment.equipedHeadItem,
			equipedChestItem = equipment.equipedChestItem,
			equipedHandsItem = equipment.equipedHandsItem,
			equipedLegsItem = equipment.equipedLegsItem,
			equipedFeetItem = equipment.equipedFeetItem,
			equipedWeaponItem = equipment.equipedWeaponItem,

			currentHealth = playerStats.currentHealth,
			currentHunger = playerStats.currentHunger,
			currentThirs = playerStats.currentThirs,

			placedStructures = buildSystem.placedStructures.ToArray(),
		};
		string		jsonData = JsonUtility.ToJson(savedData);
		
		// Write save data in to file
		filePath = Application.persistentDataPath + "/SavedData.json";
		System.IO.File.WriteAllText(filePath, jsonData);
		mainMenu.loadGameButton.interactable = true;
		mainMenu.clearSaveButton.interactable = true;
	}

	public void    LoadData(){
		if (!SaveSystemActive) return;

		// Recover all the data
		filePath = Application.persistentDataPath + "/SavedData.json";
		string		jsonData = System.IO.File.ReadAllText(filePath);
		SavedData	savedData = JsonUtility.FromJson<SavedData>(jsonData);

		// Load the saved data
		playerTransform.position = savedData.playerPosition;
		equipment.LoadEquipments(new ItemData[]{
			savedData.equipedHeadItem,
			savedData.equipedChestItem,
			savedData.equipedHandsItem,
			savedData.equipedLegsItem,
			savedData.equipedFeetItem,
			savedData.equipedWeaponItem
		});
		Inventory.instance.LoadData(savedData.inventory);
		playerStats.currentHealth = savedData.currentHealth;
		playerStats.currentHunger = savedData.currentHunger;
		playerStats.currentThirs = savedData.currentThirs;
		playerStats.UpdateHealthBarFill();
		buildSystem.LoadStructures(savedData.placedStructures);

		pauseMenu.ClosePauseMenu();
	}
}

public class SavedData{
	public Vector3					playerPosition;
	public List<ItemInInventory>	inventory;

	public ItemData					equipedHeadItem;
	public ItemData					equipedChestItem;
	public ItemData					equipedHandsItem;
	public ItemData					equipedLegsItem;
	public ItemData					equipedFeetItem;
	public ItemData					equipedWeaponItem;

	public float					currentHealth;
	public float					currentHunger;
	public float					currentThirs;

	public PlacedStructure[]		placedStructures;
}
