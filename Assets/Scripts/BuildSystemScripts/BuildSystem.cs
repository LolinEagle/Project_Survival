using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class BuildSystem : MonoBehaviour{
	[Header("Configuration")]
	[SerializeField] private BuildingGrid	grid;
	[SerializeField] private Transform		placedStructuresParent;
	[SerializeField] private Structure[]	structures;
	[SerializeField] private Material		blueMaterial;
	[SerializeField] private Material		redMaterial;
	[SerializeField] private Transform		rotationRef;
	[SerializeField] private AudioSource	audioSource;
	[SerializeField] private AudioClip		buildingSound;

	[Header("UI References")]
	[SerializeField] private Transform		buildingSystemUIPanel;
	[SerializeField] private GameObject		BuildingRequiredElement;

	private Structure	currentStructure;
	private bool		canBuild;
	private bool		inPlace;
	private Vector3		finalPosition;
	private bool		systemEnabled = false;

	public List<PlacedStructure>	placedStructures;

	void				Awake(){
		currentStructure = structures.First();
		DisableSystem();
	}

	void				ChangeStructureType(Structure newStructure){
		buildingSystemUIPanel.gameObject.SetActive(true);
		systemEnabled = true;
		currentStructure = newStructure;
		foreach (var structure in structures){
			structure.placementPrefab.SetActive(structure.structureType == currentStructure.structureType);
		}

		UpdateDisplayCosts();
	}

	private GameObject	GetPlacementPrefab(){
		return (structures.Where(elem => elem.structureType == currentStructure.structureType).FirstOrDefault().placementPrefab);
	}

	private Structure	GetStructureByType(StructureType structureType){
		return (structures.Where(elem => elem.structureType == structureType).FirstOrDefault());
	}

	void				SetPosition(Vector3 targetPosition){
		Transform	prefabTransform = GetPlacementPrefab().transform;
		Vector3		positionVelocity = Vector3.zero;

		if (Vector3.Distance(prefabTransform.position, targetPosition) > 10){
			prefabTransform.position = targetPosition;
		} else {
			prefabTransform.position = Vector3.SmoothDamp(prefabTransform.position, targetPosition, ref positionVelocity, 0, 15000);
		}
	}

	void				CheckPosition(){
		inPlace = GetPlacementPrefab().transform.position == finalPosition;
		if (!inPlace){
			SetPosition(finalPosition);
		}
	}

	void				UpdateStructureMaterial(){
		MeshRenderer	meshRenderer = GetPlacementPrefab().GetComponentInChildren<CollisionDetectionEdge>().meshRenderer;

		if (inPlace && canBuild && HasAllRessources()){
			meshRenderer.material = blueMaterial;
		} else {
			meshRenderer.material = redMaterial;
		}
	}

	private void		FixedUpdate(){
		if (!systemEnabled){
			return ;
		}
		canBuild = GetPlacementPrefab().GetComponentInChildren<CollisionDetectionEdge>().CheckConnection();
		finalPosition = grid.GetNearestPointOnGrid(transform.position);
		CheckPosition();
		RoundPlacementStructureRotation();
		UpdateStructureMaterial();
	}

	void				RoundPlacementStructureRotation(){
		float	Yangle = rotationRef.localEulerAngles.y;
		int		roundedRotation = 0;

		if (Yangle > -45 && Yangle <= 45){
			roundedRotation = 0;
		} else if (Yangle > 45 && Yangle <= 135){
			roundedRotation = 90;
		} else if (Yangle > 135 && Yangle <= 225){
			roundedRotation = 180;
		} else if (Yangle > 225 && Yangle <= 315){
			roundedRotation = 270;
		}

		GetPlacementPrefab().transform.rotation = Quaternion.Euler(0, roundedRotation, 0);
	}

	void				RotateStructure(){
		if (currentStructure.structureType == StructureType.Stairs){
			GetPlacementPrefab().transform.GetChild(0).transform.Rotate(0, 90, 0);
		}
	}

	void				DisableSystem(){
		systemEnabled = false;
		buildingSystemUIPanel.gameObject.SetActive(false);
		currentStructure.placementPrefab.SetActive(false);
	}
	
	bool				HasAllRessources(){
		BuildingRequiredElement[]	requiredElements = GameObject.FindObjectsByType<BuildingRequiredElement>(FindObjectsSortMode.None);

		return (requiredElements.All(requiredElements => requiredElements.hasRessources));
	}

	void				BuildStructure(){
		Instantiate(
			GetStructureByType(currentStructure.structureType).instantiatedPrefab,
			GetPlacementPrefab().transform.position,
			GetPlacementPrefab().transform.GetChild(0).transform.rotation,
			placedStructuresParent
		);
		placedStructures.Add(new PlacedStructure {
			placementPrefab = currentStructure.instantiatedPrefab,
			pos = GetPlacementPrefab().transform.position,
			rot = GetPlacementPrefab().transform.GetChild(0).transform.rotation.eulerAngles
		});
		audioSource.PlayOneShot(buildingSound);
		for (int i = 0; i < currentStructure.ressourcesCost.Length; i++){
			for (int j = 0; j < currentStructure.ressourcesCost[i].count; j++){
				Inventory.instance.RemoveItem(currentStructure.ressourcesCost[i].itemData);
			}
		}
	}

	private void		Update(){
		if (Input.GetKeyDown(KeyCode.Alpha1)){
			if (currentStructure.structureType == StructureType.Stairs && systemEnabled){
				DisableSystem();
			} else {
				ChangeStructureType(GetStructureByType(StructureType.Stairs));
			}
			
		}

		if (Input.GetKeyDown(KeyCode.Alpha2)){
			if (currentStructure.structureType == StructureType.Wall && systemEnabled) {
				DisableSystem();
			} else {
				ChangeStructureType(GetStructureByType(StructureType.Wall));
			}
		}

		if (Input.GetKeyDown(KeyCode.Alpha3)){
			if (currentStructure.structureType == StructureType.Floor && systemEnabled) {
				DisableSystem();
			} else {
				ChangeStructureType(GetStructureByType(StructureType.Floor));
			}
		}

		if (Input.GetKeyDown(KeyCode.Mouse0) && canBuild && inPlace && systemEnabled && HasAllRessources()){
			BuildStructure();
		}

		if (Input.GetKeyDown(KeyCode.R)){
			RotateStructure();
		}
	}

	public void			UpdateDisplayCosts(){
		foreach (Transform child in buildingSystemUIPanel){
			Destroy(child.gameObject);
		}

		foreach (ItemInInventory requiredRessource in currentStructure.ressourcesCost){
			GameObject requiredElement = Instantiate(BuildingRequiredElement, buildingSystemUIPanel);
			requiredElement.GetComponent<BuildingRequiredElement>().Setup(requiredRessource);
		}
	}

	public void			LoadStructures(PlacedStructure[] structureToLoad){
		foreach (PlacedStructure structure in structureToLoad){
			placedStructures.Add(structure);
			GameObject	newStructure = Instantiate(structure.placementPrefab, placedStructuresParent);
			newStructure.transform.position = structure.pos;
			newStructure.transform.rotation = Quaternion.Euler(structure.rot);
		}
	}
}

[System.Serializable]
public class Structure{
	public GameObject			placementPrefab;
	public GameObject			instantiatedPrefab;
	public StructureType		structureType;
	public ItemInInventory[]	ressourcesCost;
}

public enum StructureType{
	Stairs,
	Wall,
	Floor
}

[System.Serializable]
public class PlacedStructure{
	public GameObject	placementPrefab;
	public Vector3		pos;
	public Vector3		rot;
}
