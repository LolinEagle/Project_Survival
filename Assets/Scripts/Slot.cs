using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
	[SerializeField] private ItemActionSystem	itemActionSystem;

	public ItemData	item;
	public Image	itemVisual;
	public Text		countText;	

	public void	OnPointerEnter(PointerEventData eventData){
		if (item != null){
			TooltipSystem.instance.Show(item.name, item.description);
		}
	}

	public void	OnPointerExit(PointerEventData eventData){
		TooltipSystem.instance.Hide();
	}

    public void	ClickOnSlot(){
		itemActionSystem.OpenActionPanel(item, transform.position);
    }
}
