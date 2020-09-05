
using UnityEngine;
using UnityEngine.UI;

public class HotBarController : MonoBehaviour {

    public GameObject prefab;
    public Toggle toggle;

    public Toggle GetToggle() {
        return toggle;
    }

    public GameObject GetPrefab() {
        return prefab;
    }

}
