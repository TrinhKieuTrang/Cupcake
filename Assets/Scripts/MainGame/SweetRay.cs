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
    [SerializeField] private MeshRenderer ovenRender;
    public List<SweetData> sweetList = new List<SweetData>();
    private Queue<Sweet> sweets = new Queue<Sweet>();

    private Vector3 dir;
    private float offset = 0.06f;

    void Start()
    {
        InstantiateSweets();
    }

    void InstantiateSweets()
    {
        float yRot = transform.parent.localRotation.eulerAngles.y;
        transform.parent.localRotation = Quaternion.Euler(0, 0, 0);
        dir = Vector3.right;

        int totalCount = 0;
        foreach (var data in sweetList) totalCount += data.count;

        Vector3 startPos = transform.GetChild(0).position;

        int createdIndex = 0;
        foreach (var data in sweetList)
        {
            for (int i = 0; i < data.count; i++)
            {
                Vector3 spawnPos = startPos;

                if (createdIndex < 2)
                {
                    spawnPos = startPos - dir * offset * createdIndex;
                }

                else
                {
                    spawnPos = startPos - dir * offset * (createdIndex - 1) - new Vector3(0, 0, offset);
                }

                GameObject go = Instantiate(sweetPrefab, spawnPos, Quaternion.identity, cakeContainer);
                MeshRenderer rend = go.GetComponentInChildren<MeshRenderer>();

                Material[] mats = rend.materials; 
                mats[0] = GameManager.Instance.GetMaterialByColor((int)data.color); 
                rend.materials = mats; 

                Sweet sweet = go.GetComponent<Sweet>();
                if (sweet != null) sweet.color = data.color;

                sweets.Enqueue(sweet);
                createdIndex++;
            }
        }

        transform.parent.localRotation = Quaternion.Euler(0, yRot, 0);

        ChangeColorOven();
    }

    private void ChangeColorOven()
    {
        if (sweets.Count == 0) return;
        Sweet lastSweet = sweets.ToArray()[sweets.Count - 1];
        Material lastMat = GameManager.Instance.GetMaterialByColor((int)lastSweet.color);

        Material[] mats = ovenRender.materials;
        mats[0] = lastMat;
        ovenRender.materials = mats;
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
                ChangeColorOven();
            }
        }
    }

    public void ShiftSweets()
    {
        float size = transform.localScale.x;
        Vector3 basePos = transform.GetChild(0).position;

        int index = 0;
        foreach (Sweet s in sweets)
        {
            if (s == null) continue;

            Vector3 targetPos = basePos;
            if (index < 2)
            {
                targetPos = basePos - dir * offset * index;
            }

            else
            {
                targetPos = basePos - dir * offset * (index - 1) - new Vector3(0, 0, offset);
            }

            s.transform.DOMove(targetPos, 0.15f);
            index++;
        }
    }
}
