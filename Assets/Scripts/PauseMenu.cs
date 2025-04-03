using UnityEngine;

public class PauseMenu : MonoBehaviour{
	[SerializeField] private GameObject					pauseMenu;
	[SerializeField] private GameObject					optionMenu;
	[SerializeField] private ThirdPersonOrbitCamBasic	cameraScript;

	private bool	isMenuOpen = false;

	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)){
			OpenClosePauseMenu();
		}
	}

	public void OpenClosePauseMenu(){
		isMenuOpen = !isMenuOpen;
		pauseMenu.SetActive(isMenuOpen);
		optionMenu.SetActive(false);
		Time.timeScale = isMenuOpen ? 0 : 1;
		cameraScript.enabled = !isMenuOpen;
	}

	public void ClosePauseMenu(){
		isMenuOpen = false;
		pauseMenu.SetActive(false);
		optionMenu.SetActive(false);
		Time.timeScale = 1;
		cameraScript.enabled = true;
	}
}
