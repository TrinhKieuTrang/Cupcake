using UnityEngine;

public class Box : MonoBehaviour
{
    private void OnMouseDown()
    {
        transform.parent.GetComponent<Tray>().Down();
    }

    private void OnMouseDrag()
    {
        transform.parent.GetComponent<Tray>().Drag();
    }

    private void OnMouseUp()
    {
        transform.parent.GetComponent<Tray>().Up();
    }
}
