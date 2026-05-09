using UnityEngine;

public class NoteInteraction : MonoBehaviour
{
    public GameObject noteText;

    public void ShowNote()
    {
        noteText.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HideNote()
    {
        noteText.SetActive(false);
        Time.timeScale = 1f;
    }
}