using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/New item")]
public class ItemData : ScriptableObject{
	[Header("Data")]
	public string			itemName;
	public string			description;
	public Sprite			visual;
	public GameObject		prefab;
	public bool				stackable;
	public int				maxStack;

	[Header("Effects")]
	public float	healthEffect;
	public float	hungerEffect;
	public float	thirsEffect;

	[Header("Stats")]
	public float armorPoint;
	public float attackPoints;

	[Header("Types")]
	public ItemType			itemType;
	public EquipmentType	equipmentType;
}

public enum ItemType{
	Ressource,
	Equipment,
	Consumable
}

public enum EquipmentType{
	None,
	Head,
	Chest,
	Gloves,
	Legs,
	Feet,
	Weapon
}
