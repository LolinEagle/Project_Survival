using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class InteractBehaviour : MonoBehaviour{
	[Header("References")]
	[SerializeField] private MoveBehaviour		playerMoveBehaviour;
	[SerializeField] private Animator			playerAnimator;
	[SerializeField] private Inventory			inventory;
	[SerializeField] private Equipment			equipment;
	[SerializeField] private EquipmentLibary	equipmentLibary;
	[SerializeField] private AudioSource		audioSource;

	[Header("Tools Configuration")]
	[SerializeField] private GameObject		pickaxeVisual;
	[SerializeField] private GameObject		axeVisual;
	[SerializeField] private AudioClip		pickaxeSound;
	[SerializeField] private AudioClip		axeSound;

	[Header("Other")]
	[SerializeField] private AudioClip		pickupSound;

	[HideInInspector] public bool isBusy = false;

	private Item		currentItem;
	private Harvestable currentHarvestable;
	private Tool		currentTool;
	private Vector3		spawnItemOffset = new Vector3(0, 0.5f, 0);

	public void		DoPickup(Item item){
		if (inventory.IsFull() || isBusy){
			return ;
		}

		isBusy = true;
		currentItem = item;
		playerAnimator.SetTrigger("Pickup");
		playerMoveBehaviour.canMove = false;
	}

	public void		DoHarvest(Harvestable harvestable){
		if (isBusy){
			return ;
		}

		isBusy = true;
		currentTool = harvestable.tool;
		EnableToolVisual(currentTool);
		currentHarvestable = harvestable;
		playerAnimator.SetTrigger("Harvest");
		playerMoveBehaviour.canMove = false;
	}

	IEnumerator		BreakHarvestable(){
		Harvestable	harvestable = currentHarvestable;

		harvestable.gameObject.layer = LayerMask.NameToLayer("Default");

		if (harvestable.disableKinematicOnHarvest){
			Rigidbody	rigidbody = harvestable.gameObject.GetComponent<Rigidbody>();
			rigidbody.isKinematic = false;
			rigidbody.AddForce(transform.forward * 800, ForceMode.Impulse);
		}

		yield return new WaitForSeconds(harvestable.destroyDelay);

		for (int i = 0; i < harvestable.harvestableItem.Length; i++){
			Ressource	ressource = harvestable.harvestableItem[i];
			if (Random.Range(1, 101) <= ressource.dropChance){
				GameObject	instantiated = Instantiate(ressource.itemData.prefab);
				instantiated.transform.position = harvestable.transform.position + spawnItemOffset;
			}
		}
		Destroy(harvestable.gameObject);
	}

	public void		AddItemToInventory(){
		inventory.AddItem(currentItem.itemData);
		audioSource.PlayOneShot(pickupSound);
		Destroy(currentItem.gameObject);
	}

	public void		ReEnablePlayerMovement(){
		EnableToolVisual(currentTool, false);
		playerMoveBehaviour.canMove = true;
		isBusy = false;
	}

	private void	EnableToolVisual(Tool toolType, bool enable = true){
		EquipmentLibraryItem	equipmentLibraryItem = equipmentLibary.content.Where(elem => elem.itemData == equipment.equipedWeaponItem).FirstOrDefault();

		if (equipmentLibraryItem != null){
			for (int i = 0; i < equipmentLibraryItem.elementsToDisable.Length; i++){
				equipmentLibraryItem.elementsToDisable[i].SetActive(enable);
			}
			equipmentLibraryItem.itemPrefab.SetActive(!enable);
		}

		switch (toolType){
			case Tool.Pickaxe:
				pickaxeVisual.SetActive(enable);
				audioSource.clip = pickaxeSound;
				break;
			case Tool.Axe:
				axeVisual.SetActive(enable);
				audioSource.clip = axeSound;
				break;
		}
	}

	public void		playHarvestingSound(){
		audioSource.Play();
	}
}
