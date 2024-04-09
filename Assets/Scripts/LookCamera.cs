using UnityEngine;

public class LookCamera : MonoBehaviour
{
    public GameObject cam;
    void Update() {
        transform.rotation = cam.transform.rotation;
    }
}
