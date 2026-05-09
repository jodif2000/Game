using UnityEngine;
using UnityEngine.Playables;

public class StartScreenController : MonoBehaviour
{
    [SerializeField] GameObject startPanel;

    [SerializeField] MonoBehaviour playerMovement;
    [SerializeField] MonoBehaviour playerRotation;
    [SerializeField] MonoBehaviour cursorLockScript;
    
    [SerializeField] GameObject dot;
    [SerializeField] GameObject ring;

    [SerializeField] Transform mainCameraTransform;
    [SerializeField] GameObject cutsceneCamera;
    [SerializeField] PlayableDirector startingCutsceneDirector;

    private void Start()
    {
        if (startPanel != null)
            startPanel.SetActive(true);

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerRotation != null)
            playerRotation.enabled = false;

        if (cursorLockScript != null)
            cursorLockScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //Time.timeScale = 1f;
        
        dot.SetActive(false);
        ring.SetActive(false);
    }

    public void StartGame()
    {
        if (startPanel != null)
            startPanel.SetActive(false);

        if (cutsceneCamera != null)
            cutsceneCamera.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerRotation != null)
            playerRotation.enabled = true;

        if (cursorLockScript != null)
            cursorLockScript.enabled = true;

        if (startingCutsceneDirector != null)
            startingCutsceneDirector.Stop();

        if (mainCameraTransform != null)
        {
            mainCameraTransform.localRotation = Quaternion.Euler(0, 0, 0);
            mainCameraTransform.localPosition = new Vector3(0.02192566f, -0.5711587f, 0.4648577f);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        startPanel.SetActive(false);

        dot.SetActive(true);
        ring.SetActive(true);
    }
}