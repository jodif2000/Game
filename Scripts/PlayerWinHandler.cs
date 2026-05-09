using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerWinHandler : MonoBehaviour
{
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject dot;
    [SerializeField] GameObject ring;
    [SerializeField] CursorLock cursorLock;
    
    [SerializeField] MonoBehaviour playerMovement;
    [SerializeField] MonoBehaviour playerRotation;

    bool isWin = false;

    public void OnWin()
    {
        if (isWin) return;

        isWin = true;

        if (winPanel != null)
            winPanel.SetActive(true);

        if (dot != null)
            dot.SetActive(false);

        if (ring != null)
            ring.SetActive(false);

        if (cursorLock != null)
            cursorLock.UnlockCursor();
        
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerRotation != null)
            playerRotation.enabled = false;

        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}