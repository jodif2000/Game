using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] Transform player; // ссылка на объект игрока
    [SerializeField] Transform visor; // ссылка на объект, отображающий глаза/голову игрока

    [SerializeField] [Range(0.01f, 10f)] float xSens = 0.1f; // чувствительность курсора по горизонтали
    [SerializeField] [Range(0.01f, 10f)] float ySens = 0.1f; // чувствительность курсора по вертикали

    Quaternion center; // центр экрана
    bool canRotate = true; // можно ли вращать игрока

    private void Start() => center = visor.localRotation;

    private void OnLook(InputValue lookValue) // метод, вызываемый при перемещении курсора
    {
        if (!canRotate) return;

        Vector2 rotation = lookValue.Get<Vector2>(); // получение смещения курсора
        float mouseY = rotation.y * ySens; // получение смещения курсора

        // расчёт поворота вокруг оси X
        Quaternion yRotation = visor.localRotation * Quaternion.AngleAxis(mouseY, -Vector3.right);

        if (Quaternion.Angle(center, yRotation) < 90)
            visor.localRotation = yRotation;

        // расчёт поворота вокруг оси Y
        float mouseX = rotation.x * xSens;
        Quaternion xRotation = player.localRotation * Quaternion.AngleAxis(mouseX, Vector3.up);

        player.localRotation = xRotation;
    }

    public void SetRotationEnabled(bool enabled)
    {
        canRotate = enabled;
    }

    public void ResetVerticalLook()
    {
        if (visor == null) return;
        visor.localRotation = center;
    }
}