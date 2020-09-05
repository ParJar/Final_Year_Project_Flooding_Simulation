using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAI : MonoBehaviour {

    private Transform destination;
    private NavMeshAgent navMeshAgent;
    private Collider thisCollider;
    private Collider waterCollider;
    private Manager manager;

    private bool hasPath;
    [SerializeField] private GameObject trailPrefab;
    private GameObject trail;

    private int speed = 100;
    private int acceleration = 4000;
    private int waterContent = 10;
    private float trailLifetime = 30f;
    private bool collision;

    private float[,,] splatMap;

    // Use this for initialization

    enum SoilTexture {
        Clay = 0,
        Sand = 1,
        Rock = 2,
        Urban = 3
    }

    void Start() {

        thisCollider = gameObject.GetComponent<Collider>();
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        waterCollider = manager.GetWaterTableObject().GetComponent<Collider>();

        navMeshAgent.speed = speed;
        navMeshAgent.angularSpeed = acceleration;
        navMeshAgent.acceleration = acceleration;

        PrepareRun();

        splatMap = manager.GetHeightMapController().splats;
    }

    // Update is called once per frame
    void Update() {

        //Update agent speed based on terrain texture
        if (!collision) {
            if (splatMap[(int)transform.position.z, (int)transform.position.x, 0] == 1f) {
                speed = 50;
                navMeshAgent.speed = speed;
            } else if (splatMap[(int)transform.position.z, (int)transform.position.x, 1] == 1f) {
                speed = 35;
                navMeshAgent.speed = speed;
            } else if (splatMap[(int)transform.position.z, (int)transform.position.x, 2] == 1f) {
                speed = 100;
                navMeshAgent.speed = speed;
            } else if (splatMap[(int)transform.position.z, (int)transform.position.x, 3] == 1f) {
                speed = 200;
                navMeshAgent.speed = speed;
            }
        }


        //Force agents to find a path and restart run if 5 units from target
        if (!navMeshAgent.hasPath) {
            SetDestination();
        } else if (navMeshAgent.remainingDistance < 5) {
            ContributeWater();
        }

        //If in water table restart run
        if (thisCollider.bounds.Intersects(waterCollider.bounds)) {
            ContributeWater();
        }
    }

    //If in water table restart run
    void OnCollisionEnter(Collision collide) {
        if (collide.gameObject.name == waterCollider.name) {
            ContributeWater();
        }
    }

    private void OnTriggerEnter(Collider other) {
        //change speed based on drag coefficient of trigger
        if (other.CompareTag("GroundWaterDrag")) {
            navMeshAgent.speed = navMeshAgent.speed / other.GetComponent<WaterFeature>().waterDragCoefficient;
            collision = true;
        }
        //restart run if entered water basin
        if (other.CompareTag("WaterFeature")) {
            if (other.GetComponent<WaterFeature>().AddWater(waterContent)) {
                PrepareRun();
            }
        }

    }

    //return to normal speed when leaving triggers
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("GroundWaterDrag")) {
            navMeshAgent.speed = speed;
        }
        collision = false;
    }

    //Contribute water and restart run
    private void ContributeWater() {
        manager.IncreaseWaterTable(waterContent);
        PrepareRun();
    }

    private void PrepareRun() {
        if (trail) {
            manager.StoreTrail(trail);
        }
        FindRunStart();
        FindNewTarget();
        SetDestination();
        ProduceNewTrail();
    }

    private void FindRunStart() {
        navMeshAgent.Warp(manager.WarpDrop());
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit)) {
            navMeshAgent.Warp(hit.point);
        }
    }

    private void FindNewTarget() {
        destination = manager.GetTargets().FindClosest(gameObject.transform.position);
    }

    private void SetDestination() {
        navMeshAgent.SetDestination(destination.position);
    }

    private void ProduceNewTrail() {
        trail = Instantiate(trailPrefab, transform.position, Quaternion.identity);
        trail.transform.SetParent(gameObject.transform);
        trail.transform.GetComponent<TrailRenderer>().time = trailLifetime;
    }
}

