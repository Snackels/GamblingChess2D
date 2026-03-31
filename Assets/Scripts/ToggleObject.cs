using UnityEngine;

public class ToggleObject : MonoBehaviour {
    [SerializeField] private GameObject objectToToggle;

    public void ToggleActiveState() {
        if (objectToToggle != null) {
            bool isActive = objectToToggle.activeSelf;
            objectToToggle.SetActive(!isActive);
        }
    }
}
