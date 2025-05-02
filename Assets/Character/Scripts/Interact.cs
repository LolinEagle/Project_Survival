using UnityEngine;

public class Interact : MonoBehaviour{
	[SerializeField] private float		interactRange;
	[SerializeField] private GameObject	interactText;
	[SerializeField] private LayerMask	layerMask;

	public InteractBehaviour	playerInteractBehaviour;

	void Update(){
		RaycastHit	hit;

		if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange, layerMask)){
			interactText.SetActive(true);
			if (Input.GetKeyDown(KeyCode.E)){
				if (hit.transform.CompareTag("Item")){
					playerInteractBehaviour.DoPickup(hit.transform.gameObject.GetComponent<Item>());
				}
				if (hit.transform.CompareTag("Harvestable")){
					playerInteractBehaviour.DoHarvest(hit.transform.gameObject.GetComponent<Harvestable>());
				}
			}
		} else {
			interactText.SetActive(false);
		}
	}
}
