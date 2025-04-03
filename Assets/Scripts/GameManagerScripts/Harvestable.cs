using UnityEngine;

public class Harvestable : MonoBehaviour{
	public Ressource[]	harvestableItem;
	public Tool			tool;
	public bool			disableKinematicOnHarvest;
	public float		destroyDelay;
}

[System.Serializable]
public class Ressource{
	public ItemData				itemData;
	[Range(0, 100)] public int	dropChance;
}

public enum Tool{
	Pickaxe,
	Axe
}
