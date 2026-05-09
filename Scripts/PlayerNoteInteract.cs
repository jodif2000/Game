using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNoteInteract : MonoBehaviour
{
    [Header("Поиск записки")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] float interactDistance = 3f;
    [SerializeField] LayerMask noteLayerMask;

    [Header("Подсказка")]
    [SerializeField] GameObject interactPrompt;

    [Header("Панель записки")]
    [SerializeField] GameObject notePanel;
    [SerializeField] GameObject idCardPanel;
    [SerializeField] GameObject map;

    [Header("Тексты записок")]
    [SerializeField] GameObject worldNote00;
    [SerializeField] GameObject worldNote01; // записка в сцене
    [SerializeField] GameObject worldNote02; // записка в сцене
    [SerializeField] GameObject mapNote;
    [SerializeField] GameObject worldNote021;
    [SerializeField] GameObject worldNote03; // записка в сцене

    [SerializeField] GameObject noteText01; // текст на панели
    [SerializeField] GameObject noteText02; // текст на панели
    [SerializeField] GameObject noteText021;
    [SerializeField] GameObject noteText03; // текст на панели

    [Header("Альтернативные тексты")]
    [SerializeField] GameObject noteText02_Unread;

    [Header("Смена внешнего вида 2-й записки")]
    [SerializeField] MeshRenderer note02MeshRenderer;
    [SerializeField] Material note02Material_Normal;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip noteOpenSound;

    [SerializeField] PlayerRotation rotationScript; // скрипт вращения игрока

    bool isLookingAtNote;
    bool isReading;
    bool hasReadFirstNote = false;

    GameObject currentNote; // текущая записка, на которую смотрит игрок

    private void Update()
    {
        if (isReading)
        {
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
            return;
        }

        ScanForNote();
    }

    private void ScanForNote()
    {
        bool wasLookingAtNote = isLookingAtNote;
        isLookingAtNote = false;
        currentNote = null;

        if (cameraTransform == null) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, noteLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Note"))
            {
                isLookingAtNote = true;
                currentNote = hit.collider.gameObject;
            }
        }

        if (!wasLookingAtNote && isLookingAtNote)
        {
            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }

        if (wasLookingAtNote && !isLookingAtNote)
        {
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }

    private void OnInteract(InputValue _)
    {
        if (isReading)
        {
            CloseNote();
            return;
        }

        if (isLookingAtNote)
        {
            OpenNote();
        }
    }

    private void OpenNote()
    {
        isReading = true;

        if (currentNote == worldNote00)
        {
            if (idCardPanel != null) idCardPanel.SetActive(true);
        }

        else if (currentNote == mapNote)
        {
            if (map != null) map.SetActive(true);
        }

        else
        {
            if (notePanel != null) notePanel.SetActive(true);
        }

        ShowCurrentNoteText();

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (audioSource != null && noteOpenSound != null)
            audioSource.PlayOneShot(noteOpenSound);

        if (rotationScript != null)
            rotationScript.SetRotationEnabled(false);

        Time.timeScale = 0f;
    }

    private void CloseNote()
    {
        isReading = false;

        if (notePanel != null) notePanel.SetActive(false);
        if (idCardPanel != null) idCardPanel.SetActive(false);
        if (map != null) map.SetActive(false);

        HideAllNoteTexts();

        if (audioSource != null && noteOpenSound != null)
            audioSource.PlayOneShot(noteOpenSound);

        if (rotationScript != null)
            rotationScript.SetRotationEnabled(true);

        Time.timeScale = 1f;
    }

    private void ShowCurrentNoteText()
    {
        HideAllNoteTexts();

        if (currentNote == worldNote01 && noteText01 != null) 
        {
            noteText01.SetActive(true);
            hasReadFirstNote = true;

            if (note02MeshRenderer != null && note02Material_Normal != null)
            {
                note02MeshRenderer.material = note02Material_Normal;
            }
        }

        if (currentNote == worldNote02)
        {
            if (hasReadFirstNote)
            {
                if (noteText02 != null) noteText02.SetActive(true);
            }
            else
            {
                if (noteText02_Unread != null) noteText02_Unread.SetActive(true);
            }
        }

        if (currentNote == worldNote021 && noteText021 != null)
            noteText021.SetActive(true);

        if (currentNote == worldNote03 && noteText03 != null)
            noteText03.SetActive(true);
    }

    private void HideAllNoteTexts()
    {
        if (noteText01 != null) noteText01.SetActive(false);
        if (noteText02 != null) noteText02.SetActive(false);
        if (noteText02_Unread != null) noteText02_Unread.SetActive(false);
        if (noteText021 != null) noteText021.SetActive(false);
        if (noteText03 != null) noteText03.SetActive(false);
    }

    private void OnDisable()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (notePanel != null)
            notePanel.SetActive(false);

        if (map != null)
            map.SetActive(false);

        HideAllNoteTexts();

        if (rotationScript != null)
            rotationScript.SetRotationEnabled(true);

        Time.timeScale = 1f;
    }
}