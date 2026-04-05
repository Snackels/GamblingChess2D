using UnityEngine;
using UnityEngine.UI;

public class Clickable : MonoBehaviour {

    public float alphaThreshold = 0.1f;
    void Start() {
        GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;
    }

}
