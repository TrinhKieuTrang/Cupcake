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
        bool useX = transform.parent.localRotation.y == 0;
        dir = useX ? Vector3.right : Vector3.forward;
        float size = transform.localScale.x;

        int totalCount = 0;
        foreach (var data in sweetList) totalCount += data.count;

        Vector3 startPos = transform.position + dir * (size / 2 - 0.05f);

        int createdIndex = 0;
        foreach (var data in sweetList)
        {
            for (int i = 0; i < data.count; i++)
            {
                Vector3 spawnPos = startPos;

                if (createdIndex == 0)
                {
                    spawnPos = startPos + Vector3.up * offset;
                }
                else if (createdIndex == 1)
                {
                    spawnPos = startPos + Vector3.up * offset + Vector3.right * offset;
                }
                else
                {
                    spawnPos = startPos - dir * offset * (createdIndex - 1);
                }

                GameObject go = Instantiate(sweetPrefab, spawnPos, Quaternion.identity, cakeContainer);
                Sweet sweet = go.GetComponent<Sweet>();
                if (sweet != null) sweet.color = data.color;

                sweets.Enqueue(sweet);
                createdIndex++;
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

    public void ShiftSweets()
    {
        float size = transform.localScale.x;
        Vector3 basePos = transform.position + dir * (size / 2 - 0.05f);

        int index = 0;
        foreach (Sweet s in sweets)
        {
            if (s == null) continue;

            Vector3 targetPos = basePos;

            if (index == 0)
            {
                targetPos = basePos + Vector3.up * offset;
            }
            else if (index == 1)
            {
                targetPos = basePos + Vector3.up * offset + Vector3.right * offset;
            }
            else
            {
                targetPos = basePos - dir * offset * (index - 1);
            }

            s.transform.DOMove(targetPos, 0.2f);
            index++;
        }
    }
}
