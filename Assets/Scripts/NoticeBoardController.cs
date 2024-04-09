using UnityEngine;

public class NoticeBoardController : MonoBehaviour
{
    public GameObject btn_noticeboard;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            btn_noticeboard.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            btn_noticeboard.SetActive(false);
        }
    }
}
