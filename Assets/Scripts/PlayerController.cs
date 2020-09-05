using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    [SerializeField] private Manager manager;
    [SerializeField] private GameObject treePrefab;

    //UI Elements
    [SerializeField] private GameObject setupPanel;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private InputField agentNumber;
    [SerializeField] private InputField waterTable;
    [SerializeField] private InputField terrainHeight;

    [SerializeField] private GameObject errorPanel;
    [SerializeField] private Text errorText;

    [SerializeField] private Text waterLevelText;
    [SerializeField] private Text rainFallText;
    [SerializeField] private Text ticksText;
    [SerializeField] private Text timeElapsedText;

    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private GameObject selectedPrefab;

    private HeightMapController heightMapController;
    private int ticks = 0;
    private float startTime = 0;
    private float pauseTime = 0;


    void Start() {
        terrainHeight.text = HeightMapController.defaultHeight.ToString();
        heightMapController = manager.GetHeightMapController();
        //Hide pause and error panels
        pauseMenu.SetActive(false);
        errorPanel.SetActive(false);
        //Disable VSYNC
        QualitySettings.vSyncCount = 0;
    }

    void Update() {

        //Handle Pause
        if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.Escape)) {
            TogglePaused();
        }

        //Update Simulation Information
        waterLevelText.text = "Water Level: " + manager.GetWaterTable().ToString("0.000") + "m";
        if (manager.IsSimulationActive()) {
            ticks++;
            ticksText.text = "Ticks: " + ticks.ToString();
            timeElapsedText.text = "Time Elapsed: " + (Time.time - startTime).ToString("0.0");
        } else {
            pauseTime += Time.deltaTime;
        }
       
        //Handle Clicks
        if (Input.GetMouseButtonDown(0) && selectedPrefab != null) {

            if (EventSystem.current.IsPointerOverGameObject()) {
                return;
            }

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {

                Instantiate(selectedPrefab, hit.point, selectedPrefab.transform.rotation);

                int width = selectedPrefab.GetComponent<WaterFeature>().width / 2;
                int length = selectedPrefab.GetComponent<WaterFeature>().length / 2;

                heightMapController.Flatten((int)hit.point.z - length, (int)hit.point.z + length, (int)hit.point.x - width, (int)hit.point.x + width);
            }
        }
    }


    //Begin Simulation Button
    public void ButtonHandleBeginSimulation() {

        float water = float.Parse(waterTable.text);
        float terrH = float.Parse(terrainHeight.text);
        int agentN = int.Parse(agentNumber.text);

        if (agentN < 1 || terrH < 1 || water < 0) {
            errorPanel.SetActive(true);
            errorText.text = "Simulation parameters should be greater than 0";
            return;
        }

        manager.BeginSimulation(agentN, water, terrH);
        ticks = 0;
        setupPanel.SetActive(false);

        startTime = Time.time;
        rainFallText.text = "Rainfall(agents): " + manager.GetNumberOfAgents().ToString();
    }

    //HotBar Items
    public void ButtonHandleSelectItem(HotBarController hotBarItem) {
        Toggle change = hotBarItem.GetToggle();

        if (change.isOn) {
            change.GetComponent<Image>().color = Color.white;
            selectedPrefab = hotBarItem.GetPrefab();
        } else {
            change.GetComponent<Image>().color = Color.black;
            selectedPrefab = null;
        }

    }

    //Update Terrain Height
    public void HandleTerrainHeightChange(Text newValue) {
        manager.UpdateTerrainHeight(float.Parse(newValue.text));
    }

    //Update Water Height
    public void HandleWaterHeightChange(Text newValue) {
        manager.UpdateWaterLevel(float.Parse(newValue.text));
    }

    //Handle Pausing
    public void TogglePaused() {
        bool paused = pauseMenu.activeSelf;
        paused = !paused;
        pauseMenu.SetActive(paused);
    }

    public void HideErrorPanel() {
        errorPanel.SetActive(false);
    }

    //Quit Simulation
    public void Quit() {
        SceneManager.LoadScene(0);
    }

}
