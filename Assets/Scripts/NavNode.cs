using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavNode : MonoBehaviour {

    [SerializeField] private Vector3 position;
    [SerializeField] private float weight = int.MaxValue;
    [SerializeField] private NavNode parentNode;
    [SerializeField] private bool parentSet;
    [SerializeField] private List<NavNode> neighbourNode;

    // Use this for initialization

    public NavNode(Vector3 position, GameObject prefab) {
        neighbourNode = new List<NavNode>();
        this.position = position;
        parentNode = null;
        parentSet = false;
    }


    //Set Methods
    public void AddNode(NavNode node) {
        neighbourNode.Add(node);
    }

    public void SetParentNode(NavNode node) {
        this.parentSet = true;
        this.parentNode = node;
    }


    public void SetWeight(float value) {
        this.weight = value;
    }

    //Get Methods
    public List<NavNode> GetNeighbourNode() {
        return neighbourNode;
    }

    public float GetWeight() {
        return weight;

    }

    public NavNode GetParentNode() {
        return parentNode;
    }

    public Vector3 GetPosition() {
        return position;
    }

    public bool GetParentSet() {
        return parentSet;
    }
}
