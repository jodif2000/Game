using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] GameObject losePanel;

    [SerializeField] GameObject dot;
    [SerializeField] GameObject ring;

    [SerializeField] CursorLock cursorLock;

    [SerializeField] MonoBehaviour playerMovement;
    [SerializeField] MonoBehaviour playerRotation;
    [SerializeField] MonoBehaviour playerPushBox;

    bool isDead = false;

    // смерть от пазлов / газа
    public void OnDeath()
    {
        if (isDead) return;

        isDead = true;

        if (losePanel != null)
            losePanel.SetActive(true);

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

        if (playerPushBox != null)
            playerPushBox.enabled = false;

        Time.timeScale = 0f;
    }

    // смерть от киллера
    public void KilledByKiller()
    {
        OnDeath();
    }

    // кнопка ЗАНОВО (перезапуск комнаты)
    public void RestartLevel()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.RespawnPlayer(gameObject, this);
    }

    public void ResetAfterDeath()
    {
        isDead = false;

        if (losePanel != null)
            losePanel.SetActive(false);

        if (dot != null)
            dot.SetActive(true);

        if (ring != null)
            ring.SetActive(true);

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerRotation != null)
            playerRotation.enabled = true;

        if (playerPushBox != null)
            playerPushBox.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}