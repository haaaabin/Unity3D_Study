using UnityEngine;

public class NoticeBoard : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            InGameUI.Instance().ToggleNoticeBoardButton();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            InGameUI.Instance().ToggleNoticeBoardButton();
        }
    }
}