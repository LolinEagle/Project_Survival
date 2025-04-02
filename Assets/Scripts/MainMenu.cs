using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour{
	[SerializeField] private Button		loadGameButton;
	[SerializeField] private Button		clearSaveButton;
	[SerializeField] private Dropdown	resolutionsDropdown;
	[SerializeField] private AudioMixer audioMixer;
	[SerializeField] private Slider		soundSlider;
	[SerializeField] private GameObject optionsPanel;

	public static bool	loadSavedData;

	void		Start(){
		// Volume
		audioMixer.GetFloat("Volume", out float soundValue);
		soundSlider.value = soundValue;

		// Load
		bool saveFileExists = System.IO.File.Exists(Application.persistentDataPath + "/SavedData.json");
		loadGameButton.interactable = saveFileExists;
		clearSaveButton.interactable = saveFileExists;

		// Resolutions
		Resolution[]	resolutions = Screen.resolutions;
		List<string>	resolutionOptrion = new List<string>();
		int				currentResolutionIndex = 0;

		resolutionsDropdown.ClearOptions();
		for (int i = 0; i < resolutions.Length ; i++){
			string	option = resolutions[i].width + "x" + resolutions[i].height;

			resolutionOptrion.Add(option);
			if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height){
				currentResolutionIndex = i;
			}
		}
		resolutionsDropdown.AddOptions(resolutionOptrion);
		resolutionsDropdown.value = currentResolutionIndex;
		resolutionsDropdown.RefreshShownValue();
	}

	public void	LoadGame(){
		loadSavedData = true;
		SceneManager.LoadScene("Scene");
	}

	public void	NewGame(){
		loadSavedData = false;
		SceneManager.LoadScene("Scene");
	}

	public void	QuitGame(){
		Application.Quit();
	}

	public void	SetResolution(int resolutionIndex){
		Resolution	resolution = Screen.resolutions[resolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
	}

	public void	SetFullScreen(bool isFullScreen){
		Screen.fullScreen = isFullScreen;
	}

	public void	SetVolume(float volume){
		audioMixer.SetFloat("Volume", volume);
	}

	public void	ClearSave(){
		System.IO.File.Delete(Application.persistentDataPath + "/SavedData.json");
		loadGameButton.interactable = false;
		clearSaveButton.interactable = false;
	}

	public void	EnableDisableOptionsPanel(){
		optionsPanel.SetActive(!optionsPanel.activeSelf);
	}
}
