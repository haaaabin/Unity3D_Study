using UnityEngine;

public class LookCamera : MonoBehaviour
{
    public GameObject cam;
    private void Start()
    {
        cam = GameObject.Find("CamerController");
    }
    void Update() {
        transform.rotation = cam.transform.rotation;
    }
}
