using UnityEngine;

public class Box : MonoBehaviour
{
    private void Start()
    {
        // Tạo 1 Bounds bao quanh toàn bộ mesh con
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds); // mở rộng bounds bao quanh từng mesh con
        }

        Vector3 size = bounds.size;

        Debug.Log($"Kích thước thực tế (World Space): {size}");
        Debug.Log($"Chiều dài (X): {size.x}");
        Debug.Log($"Chiều cao (Y): {size.y}");
        Debug.Log($"Chiều rộng (Z): {size.z}");
    }
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
