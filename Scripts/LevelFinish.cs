using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinish : MonoBehaviour
{
    [SerializeField] GameObject winPanel;  // панель завершения
    [SerializeField] float restartDelay = 2f; // задержка перед перезапуском

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (winPanel != null)
                winPanel.SetActive(true);

            Invoke(nameof(RestartLevel), restartDelay);
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}