using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOrderPuzzle : MonoBehaviour
{
    public DoorAnimationScript door;
    public GameObject poisonGas;

    [SerializeField] float exitCheckDelay = 0.05f;

    public BoxColor.ColorType[] correctOrder =
    {
        BoxColor.ColorType.Sun,
        BoxColor.ColorType.Tree,
        BoxColor.ColorType.Eye
    };

    readonly Dictionary<int, BoxColor.ColorType?> currentState = new Dictionary<int, BoxColor.ColorType?>();

    bool isSolved = false;
    Coroutine exitRoutine;

    PoisonGasDeath poisonGasDeath;

    private void Start()
    {
        for (int i = 0; i < correctOrder.Length; i++)
            currentState[i] = null;

        if (poisonGas != null)
        {
            poisonGas.SetActive(false);
            poisonGasDeath = poisonGas.GetComponent<PoisonGasDeath>();
        }
    }

    public void BoxPlaced(BoxColor box, int plateIndex)
    {
        currentState[plateIndex] = box.boxColor;

        if (exitRoutine != null)
        {
            StopCoroutine(exitRoutine);
            exitRoutine = null;
        }

        CheckPuzzleState();
    }

    public void BoxRemoved(int plateIndex)
    {
        currentState[plateIndex] = null;

        if (exitRoutine != null)
            StopCoroutine(exitRoutine);

        exitRoutine = StartCoroutine(CheckPuzzleExitDelayed());
    }

    private IEnumerator CheckPuzzleExitDelayed()
    {
        yield return new WaitForSeconds(exitCheckDelay);
        CheckPuzzleState();
        exitRoutine = null;
    }

    private void CheckPuzzleState()
    {
        bool allPressed = true;

        for (int i = 0; i < correctOrder.Length; i++)
        {
            if (currentState[i] == null)
            {
                allPressed = false;
                break;
            }
        }

        if (!allPressed)
        {
            DisableGas();

            if (isSolved)
            {
                isSolved = false;
                if (door != null)
                    door.lockGate();
            }

            return;
        }

        if (IsCorrect())
        {
            DisableGas();

            if (!isSolved)
            {
                isSolved = true;
                if (door != null)
                    door.unlockGate();
            }
        }
        else
        {
            if (isSolved)
            {
                isSolved = false;
                if (door != null)
                    door.lockGate();
            }

            EnableGas();
        }
    }

    private bool IsCorrect()
    {
        for (int i = 0; i < correctOrder.Length; i++)
        {
            if (currentState[i] != correctOrder[i])
                return false;
        }

        return true;
    }

    void EnableGas()
    {
        if (poisonGas == null) return;

        if (!poisonGas.activeSelf)
            poisonGas.SetActive(true);

        if (poisonGasDeath != null)
            poisonGasDeath.StartDeathTimer();
    }

    void DisableGas()
    {
        if (poisonGasDeath != null)
            poisonGasDeath.StopDeathTimer();

        if (poisonGas != null && poisonGas.activeSelf)
            poisonGas.SetActive(false);
    }

    public void ResetPuzzle()
    {
        if (exitRoutine != null)
        {
            StopCoroutine(exitRoutine);
            exitRoutine = null;
        }

        isSolved = false;

        currentState.Clear();

        for (int i = 0; i < correctOrder.Length; i++)
            currentState[i] = null;

        DisableGas();

        if (door != null)
            door.lockGate();
    }
}