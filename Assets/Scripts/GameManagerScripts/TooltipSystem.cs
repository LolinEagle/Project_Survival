using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
	public static TooltipSystem instance;

	[SerializeField]
	private Tooltip             tooltip;

	private void Awake()
	{
		instance = this;
	}

	public void Show(string header, string content = "")
	{
		tooltip.SetText(header, content);
		tooltip.gameObject.SetActive(true);
	}

	public void Hide()
	{
		tooltip.gameObject.SetActive(false);
	}
}
