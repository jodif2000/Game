using UnityEngine;

public class RoomHint : MonoBehaviour
{
    [SerializeField] GameObject hintText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (hintText != null)
                hintText.SetActive(true);
        }
    }
}