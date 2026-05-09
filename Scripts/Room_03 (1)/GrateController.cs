using UnityEngine;

public class GrateController : MonoBehaviour
{
    [Header("Аниматор решётки")]
    [SerializeField] private Animator grateAnimator;

    [Header("Параметр открытия")]
    [SerializeField] private string openParameter = "isOpen";

    private bool isOpen;

    private void Awake()
    {
        if (grateAnimator == null)
        {
            grateAnimator = GetComponent<Animator>();
        }
    }

    public void OpenGrate()
    {
        if (isOpen) return;

        isOpen = true;

        if (grateAnimator != null)
        {
            grateAnimator.SetBool(openParameter, true);
        }

        SoundManager.PlaySound(SoundType.Grate_Open, 1f);
    }

    public void CloseGrate()
    {
        isOpen = false;

        if (grateAnimator != null)
        {
            grateAnimator.SetBool(openParameter, false);
        }
    }
}