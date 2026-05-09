using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorTriggerScript : MonoBehaviour
{
    public UnityEvent OnBoxEnter;
    public UnityEvent OnBoxExit;

    [SerializeField] float exitCheckDelay = 0.05f;

    readonly HashSet<Collider> boxColliders = new HashSet<Collider>();
    bool isPressed = false;
    Coroutine exitRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Box") && !other.CompareTag("Player")) return;

        boxColliders.Add(other);

        if (exitRoutine != null)
        {
            StopCoroutine(exitRoutine);
            exitRoutine = null;
        }

        if (!isPressed)
        {
            isPressed = true;
            OnBoxEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Box") && !other.CompareTag("Player")) return;

        boxColliders.Remove(other);

        if (exitRoutine != null)
            StopCoroutine(exitRoutine);

        exitRoutine = StartCoroutine(CheckBoxExitDelayed());
    }

    private IEnumerator CheckBoxExitDelayed()
    {
        yield return new WaitForSeconds(exitCheckDelay);

        boxColliders.RemoveWhere(c => c == null);

        if (boxColliders.Count == 0 && isPressed)
        {
            isPressed = false;
            OnBoxExit?.Invoke();
        }

        exitRoutine = null;
    }
}