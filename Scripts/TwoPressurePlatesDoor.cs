using UnityEngine;

public class TwoPressurePlatesDoor : MonoBehaviour
{
    [SerializeField] DoorAnimationScript door; // ссылка на дверь

    bool plateA;
    bool plateB;

    public void SetPlateA(bool pressed)
    {
        plateA = pressed;
        UpdateDoor();
    }

    public void SetPlateB(bool pressed)
    {
        plateB = pressed;
        UpdateDoor();
    }

    void UpdateDoor()
    {
        if (door == null) return;

        if (plateA && plateB)
            door.unlockGate();
        else
            door.lockGate();
    }
}