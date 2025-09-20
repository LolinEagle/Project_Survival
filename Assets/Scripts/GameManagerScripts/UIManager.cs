using UnityEngine;
using System.Linq;

public class UIManager : MonoBehaviour{
	[SerializeField] private GameObject[]				UIPanels;
	[SerializeField] private ThirdPersonOrbitCamBasic	playerCameraScript;

	private float	defaultHorizontalAimingSpeed;
	private float	defaultVerticalAimingSpeed;

	[HideInInspector] public bool	isAPanelOpened;

	void Start(){
		defaultHorizontalAimingSpeed = playerCameraScript.horizontalAimingSpeed;
		defaultVerticalAimingSpeed = playerCameraScript.verticalAimingSpeed;
	}

	void Update(){
		isAPanelOpened = UIPanels.Any((panel) => panel == panel.activeSelf);
		if (isAPanelOpened){
			playerCameraScript.horizontalAimingSpeed = 0;
			playerCameraScript.verticalAimingSpeed = 0;
		} else {
			playerCameraScript.horizontalAimingSpeed = defaultHorizontalAimingSpeed;
			playerCameraScript.verticalAimingSpeed = defaultVerticalAimingSpeed;
		}
	}
}
