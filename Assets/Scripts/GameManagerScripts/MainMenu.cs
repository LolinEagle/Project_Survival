using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour{
	public Button						loadGameButton;
	public Button						clearSaveButton;
	[SerializeField] private Dropdown	resolutionsDropdown;
	[SerializeField] private Dropdown	qualitysDropdown;
	[SerializeField] private AudioMixer	audioMixer;
	[SerializeField] private Slider		soundSlider;
	[SerializeField] private GameObject	optionsPanel;
	[SerializeField] private Toggle		fullScreenToggle;

	[Header("heads-up display")]
	[SerializeField] private GameObject	hud;
	[SerializeField] private GameObject	inventoryPanel;
	[SerializeField] private GameObject	craftingPanel;
	[SerializeField] private GameObject	tooltip;
	[SerializeField] private GameObject	itemActionPanel;

	public static bool	loadSavedData;
	private List<int>	resolutionIndex = new List<int>();

	void		Start(){
		// Volume
		audioMixer.GetFloat("Volume", out float soundValue);
		soundSlider.value = soundValue;

		// Load
		bool	saveFileExists = System.IO.File.Exists(Application.persistentDataPath + "/SavedData.json");
		loadGameButton.interactable = saveFileExists;
		clearSaveButton.interactable = saveFileExists;

		// Quality
		string[]		qualities = QualitySettings.names;
		List<string>	qualityOptrion = new List<string>();
		int				currentQualityIndex = 0;

		qualitysDropdown.ClearOptions();
		for (int i = 0; i < qualities.Length; i++){
			qualityOptrion.Add(qualities[i]);
			if (i == QualitySettings.GetQualityLevel()){
				currentQualityIndex = i;
			}
		}
		qualitysDropdown.AddOptions(qualityOptrion);
		qualitysDropdown.value = currentQualityIndex;
		qualitysDropdown.RefreshShownValue();

		// Resolutions
		Resolution[]	res = Screen.resolutions;
		List<string>	resolutionOptrion = new List<string>();
		int				currentResolutionIndex = 0;

		resolutionsDropdown.ClearOptions();
		for (int i = 0; i < res.Length ; i++){
			if (res[i].refreshRateRatio.value != Screen.currentResolution.refreshRateRatio.value || res[i].width < 1366 || res[i].height < 768)
				continue;

			string	option = res[i].width + " x " + res[i].height;// + " @ " + Mathf.RoundToInt((float)res[i].refreshRateRatio.value) + "Hz";

			resolutionOptrion.Add(option);
			resolutionIndex.Add(i);
			if (res[i].width == Screen.width && res[i].height == Screen.height){
				currentResolutionIndex = i;
			}
		}
		resolutionsDropdown.AddOptions(resolutionOptrion);
		resolutionsDropdown.value = currentResolutionIndex;
		resolutionsDropdown.RefreshShownValue();

		fullScreenToggle.isOn = Screen.fullScreen;
	}

	void		OnEnable(){
		if (hud != null) hud.SetActive(false);
		if (inventoryPanel != null) inventoryPanel.SetActive(false);
		if (craftingPanel != null) craftingPanel.SetActive(false);
		if (tooltip != null) tooltip.SetActive(false);
		if (itemActionPanel != null) itemActionPanel.SetActive(false);
	}

	void		OnDisable(){
		if (hud != null) hud.SetActive(true);
	}

	public void	LoadGame(){
		loadSavedData = true;
		Time.timeScale = 1;
		SceneManager.LoadScene("Scene");
	}

	public void	NewGame(){
		loadSavedData = false;
		Time.timeScale = 1;
		SceneManager.LoadScene("Scene");
	}

	public void	QuitGame(){
		Application.Quit();
	}

	public void LoadMainMenu(){
		SceneManager.LoadScene("MainMenu");
	}

	public void	SetQuality(int qualityIndex){
		QualitySettings.SetQualityLevel(qualityIndex);
	}

	public void	SetResolution(int i){
		Resolution	resolution = Screen.resolutions[resolutionIndex[i]];

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
