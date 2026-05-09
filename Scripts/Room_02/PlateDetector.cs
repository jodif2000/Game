using UnityEngine;

public class PlateDetector : MonoBehaviour
{
    public int plateIndex;
    public BoxOrderPuzzle puzzle;

    void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Box")) return;

        BoxColor box = other.GetComponent<BoxColor>();

        if(box != null)
        {
            puzzle.BoxPlaced(box, plateIndex);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag("Box")) return;

        puzzle.BoxRemoved(plateIndex);
    }
}