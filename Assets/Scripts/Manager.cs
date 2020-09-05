using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private GameObject targetPrefab; 

    private int numberOfAgents;
    private bool simulationActive;

    private Transform waterTable;
    private KdTree<Transform> targets;

    private Transform trailsGO;
    private Transform targetsGO;

    [SerializeField] private GameObject navMeshBuilder;
    [SerializeField] private HeightMapController heightMapController;
    [SerializeField] private Texture2D hMap;

    private float drainRate = -0.001f;

    void Start() {
        
        navMeshBuilder.SetActive(false);
        numberOfAgents = 100;
        simulationActive = false;

        waterTable = GameObject.Find("WaterTable").transform;
        trailsGO = transform.Find("Trails");
        targetsGO = transform.Find("Targets");

        targets = new KdTree<Transform>();
        BuildTerrain();
        ApplySoil();
        ApplyVegetation();
    }


    void Update() {
       Drain();
    }

    void Drain() {
        if (simulationActive && waterTable.position.y > 0.5f) {
            waterTable.Translate(Vector3.up * drainRate);
        }
    }
    //Provide new drop location for agents
    public Vector3 WarpDrop() {
        return new Vector3(Random.Range(0, 990), 100, Random.Range(0, 990));
    }

    public void TogglePaused() {
        
        if(targets.Count == 0) {
            return;
        }

        simulationActive = !simulationActive;

        if (simulationActive) {
            Time.timeScale = 1f;
        } else {
            Time.timeScale = 0f;
        }
    }

    public void BeginSimulation(int agents, float waterHeight, float terrainHeight) {
        numberOfAgents = agents;

        UpdateWaterLevel(waterHeight);
        simulationActive = true;
        navMeshBuilder.SetActive(true);
        CreateTargets();
        CreateAgents();
    }

    //WaterTable
    public void UpdateWaterLevel(float newWaterLevel) {
        waterTable.position = new Vector3(512, newWaterLevel, 512);
    }
    public void IncreaseWaterTable(int waterContent) {
        waterTable.Translate(Vector3.up * (waterContent * 0.001f));
    }

    public float GetWaterTable() {
        return waterTable.position.y;
    }

    //Terrain
    public void UpdateTerrainHeight(float newHeight) {
        heightMapController.UpdateTerrainHeight(newHeight);
    }

    public void BuildTerrain() {
        BuildTerrain(HeightMapController.defaultHeight);
    }

    public void BuildTerrain(float terrainHeight) {
        if (MainMenuController.selectedHmap == "Custom") {
            heightMapController.BuildTerrain(MainMenuController.customHeightmap, terrainHeight);
        } else {
            hMap = Resources.Load<Texture2D>("heightmaps/" + MainMenuController.selectedHmap);
            heightMapController.BuildTerrain(hMap, terrainHeight);
        }
    }

    public void ApplySoil() {
        if (MainMenuController.selectedSoilMap == "Custom") {
            heightMapController.SoilMapLoad(MainMenuController.customSoilMap);
            Debug.Log("custom map");
        } else {
            Texture2D soilMap = Resources.Load<Texture2D>("soilmaps/" + MainMenuController.selectedSoilMap);
            heightMapController.SoilMapLoad(soilMap);
        }
    }

    public void ApplyVegetation() {
        if (MainMenuController.selectedVegetationMap == "Custom") {
            heightMapController.VegetationMapLoad(MainMenuController.customVegetationMap);
        } else {
            Texture2D vMap = Resources.Load<Texture2D>("vegetationMaps/" + MainMenuController.selectedVegetationMap);
            heightMapController.VegetationMapLoad(vMap);
        }
    }


    //Agents
    public void CreateAgents() {
        for (int i = 0; i < numberOfAgents; i++) {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
        }
    }

    public void StoreTrail(GameObject trail) {
        trail.transform.SetParent(trailsGO, true);
    }

    private void CreateTargets() {
        for (int x = 0; x < HeightMapController.defaultWidth; x = x + 25) {
            for (int z = 0; z < HeightMapController.defaultLength; z = z + 25) {
                Vector3 start = new Vector3(x, 1500, z);
                RaycastHit hit;
                if (Physics.Raycast(start, -Vector3.up, out hit)) {
                    Debug.DrawLine(start, hit.point, Color.red, 3f);
                    if (hit.collider.gameObject.name == waterTable.name) {
                        GameObject target = Instantiate(targetPrefab, hit.point, Quaternion.identity);
                        target.transform.parent = targetsGO.transform;
                        targets.Add(target.transform);
                    }
                }
            }
        }
    }

    //Accessors
    public Transform GetWaterTableObject() {
        return waterTable;
    }

    public HeightMapController GetHeightMapController() {
        return heightMapController;
    }

    public KdTree<Transform> GetTargets() {
        return targets;
    }

    public bool IsSimulationActive() {
        return simulationActive;
    }

    public int GetNumberOfAgents() {
        return numberOfAgents;
    }
}
