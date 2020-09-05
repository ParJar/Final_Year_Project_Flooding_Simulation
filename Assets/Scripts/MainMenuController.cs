using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SimpleFileBrowser;

public class MainMenuController : MonoBehaviour {

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject simulationSetup;
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private Text errorText;

    [SerializeField] private Toggle padstowToggle;
    [SerializeField] private Toggle woodBridgeToggle;
    [SerializeField] private Toggle middlesboroughToggle;
    [SerializeField] private Toggle dartmouthToggle;
    [SerializeField] private Toggle customToggle;

    public static int numberOfAgents;
    public static string selectedHmap;
    public static string selectedSoilMap;
    public static string selectedVegetationMap;

    public static Texture2D customHeightmap;
    public static Texture2D customSoilMap;
    public static Texture2D customVegetationMap;

    [SerializeField] private GameObject selectMap;
    [SerializeField] private GameObject selectSoil;
    [SerializeField] private GameObject selectVegetation;

    // Use this for initialization

    enum Selection {
        HeightSelect,
        SoilSelect,
        VegetationSelect
    }

    void Start () {

        mainMenu.SetActive(true);
        simulationSetup.SetActive(false);
        errorPanel.SetActive(false);
        selectMap.SetActive(false);
        selectSoil.SetActive(false);
        selectVegetation.SetActive(false);

        selectedHmap = "Padstow";
        selectedSoilMap = "Quadrants";
        selectedVegetationMap = "blank";

        //padstowToggle.onValueChanged.AddListener(delegate {
        //    ToggleValueChanged(padstowToggle);
        //});

        //woodBridgeToggle.onValueChanged.AddListener(delegate {
        //    ToggleValueChanged(woodBridgeToggle);
        //});

        //middlesboroughToggle.onValueChanged.AddListener(delegate {
        //    ToggleValueChanged(middlesboroughToggle);
        //});

        //dartmouthToggle.onValueChanged.AddListener(delegate {
        //    ToggleValueChanged(dartmouthToggle);
        //});

        //customToggle.onValueChanged.AddListener(delegate {
        //    ToggleValueChanged(customToggle);
        //});

    }
	
	// Update is called once per frame
	void Update () {
	}

    //buttons
    public void ButtonHandleSetupSimulation() {
        mainMenu.SetActive(false);
        simulationSetup.SetActive(true);
    }

    public void ButtonHandleReturnMainMenu() {
        mainMenu.SetActive(true);
        simulationSetup.SetActive(false);
    }

    public void ButtonHandleQuit() {
        Application.Quit();
    }

    public void ButtonHandleStartSimulation() {
        SceneManager.LoadScene(1);
    }

    //toggles
    public void ToggleValueChanged(Toggle change) {

        if (change.isOn) {
            change.GetComponent<Image>().color = Color.white;
        } else {
            change.GetComponent<Image>().color = Color.black;
        }
    }

    public void SetSelection(Toggle change) {
        if (change.isOn) {

            if (change.group.name == Selection.HeightSelect.ToString()) {
                selectedHmap = change.name;
            } else if (change.group.name == Selection.SoilSelect.ToString()) {
                selectedSoilMap = change.name;
            } else if (change.group.name == Selection.VegetationSelect.ToString()) {
                selectedVegetationMap = change.name;
            }
            Debug.Log(selectedHmap);
        }
    }

    public void SwitchConfigScreen(GameObject newScreen) {

        selectMap.SetActive(false);
        selectSoil.SetActive(false);
        selectVegetation.SetActive(false);

        newScreen.SetActive(true);
    }

    //custom items
    public void ButtonHandleCustomMap(Toggle toggle) {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".png"));
        FileBrowser.SetDefaultFilter(".png");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        StartCoroutine(ShowLoadDialogCoroutine(toggle));

    }

    IEnumerator ShowLoadDialogCoroutine(Toggle toggle) {
        yield return FileBrowser.WaitForLoadDialog(false, null, "Load File", "Load");
        Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);

        if (FileBrowser.Success) {
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result);
            Texture2D tex = new Texture2D(1024, 1024);
            tex.LoadImage(bytes);

            if (tex.height > 1024 || tex.height < 1024 || tex.width > 1024 || tex.width < 1024) {
                DisplayError("Invalid Map Height or Length, dimenstion must be 1024x1024");
            } 

            if (toggle.group.name == Selection.HeightSelect.ToString()) {
                customHeightmap = tex;
            } else if (toggle.group.name == Selection.SoilSelect.ToString()) {
                customSoilMap = tex;
            } else if (toggle.group.name == Selection.VegetationSelect.ToString()) {
                customVegetationMap = tex;
            }
        }
    }

    //Error Dialog

    public void DisplayError(string message) {
        errorPanel.SetActive(true);
        errorText.text = message;
    }

    public void HideErrorDialog() {
        errorPanel.SetActive(false);
    }
}
