using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class SweetRay : MonoBehaviour
{
    public Transform pickupPoint;
    [SerializeField] private GameObject sweetPrefab;
    [SerializeField] private Transform cakeContainer;

    [System.Serializable]
    public struct SweetData
    {
        public SweetColor color;
        public int count;
    }

    public List<SweetData> sweetList = new List<SweetData>();
    private Queue<Sweet> sweets = new Queue<Sweet>();

    private Vector3 dir;      
    private float offset = 0.1f; 

    void Awake()
    {
        InstantiateSweets();
    }

    void InstantiateSweets()
    {
        bool useX = transform.localRotation.y == 0;
        dir = useX ? Vector3.right : Vector3.forward;
        float size = useX ? transform.localScale.x : transform.localScale.z;

        int totalCount = 0;
        foreach (var data in sweetList) totalCount += data.count;

        Vector3 startPos = transform.position + dir * (size / 2 - 0.05f);

        foreach (var data in sweetList)
        {
            for (int i = 0; i < data.count; i++)
            {
                GameObject go = Instantiate(sweetPrefab, startPos, Quaternion.identity, cakeContainer);
                Sweet sweet = go.GetComponent<Sweet>();
                if (sweet != null) sweet.color = data.color;

                sweets.Enqueue(sweet);

                startPos -= dir * offset;
            }
        }
    }

    public Sweet PeekSweet()
    {
        if (sweets.Count > 0) return sweets.Peek();
        return null;
    }

    public Sweet TakeSweet()
    {
        if (sweets.Count > 0) return sweets.Dequeue();
        return null;
    }

    public void PickSweet(Tray tray)
    {
        if (sweets.Count == 0 || tray.IsDone || tray.isMoving) return;

        Sweet nextSweet = sweets.ToArray()[sweets.Count - 1];

        if (nextSweet.color == tray.trayColor)
        {
            if (tray.IsEmpty() != null)
            {
                nextSweet = TakeSweet();
                tray.TryAddSweet(nextSweet);
                ShiftSweets(); 
            }
        }
    }

    private void ShiftSweets()
    {
        Vector3 curPos = transform.position + dir * ((transform.localScale.x >= transform.localScale.z ? transform.localScale.x : transform.localScale.z) / 2 - 0.05f);

        foreach (Sweet s in sweets)
        {
            if (s == null) continue;
            s.transform.DOMove(curPos, 0.2f);
            curPos -= dir * offset;
        }
    }
}
