using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "SystemChunk")]
public class SystemChunk : ScriptableObject
{
    [SerializeField] Sprite[] PartSprites = null;
    [SerializeField] Vector3[] PartPositions = null;
    [SerializeField] int[] PartSortingOrder = null;

    public SystemDisplayData[] GetDisplayData()
    {
        SystemDisplayData[] sdds = new SystemDisplayData[PartSprites.Length];

        for (int i = 0; i < PartSprites.Length; i++)
        {
            sdds[i].PartSprite = PartSprites[i];
            sdds[i].PartPosition = PartPositions[i];
            sdds[i].PartSortingOrder = PartSortingOrder[i];
        }

        return sdds;
    }

}

public struct SystemDisplayData
{
    public Sprite PartSprite;
    public Vector3 PartPosition;
    public int PartSortingOrder;
}
