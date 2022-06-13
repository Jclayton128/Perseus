using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystemHandler : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        SystemCrateHandler sch;
        if (collision.gameObject.TryGetComponent<SystemCrateHandler>(out sch))
        {
            GameObject go = Instantiate<GameObject>(sch.SystemChunk, this.transform);
            go.transform.localPosition = sch.LocalPosition;
            //SystemDisplayData[] sdds = sch.SystemChunk.GetDisplayData();
            //foreach (var sdd in sdds)
            //{
            //    CreateChildSprite(sdd);
            //}
            Destroy(collision.gameObject);
        }

    }
    private void CreateChildSprite(SystemDisplayData sdd)
    {
        GameObject go = new GameObject();
        go.transform.parent = this.transform;
        go.transform.rotation = this.transform.rotation;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sdd.PartSprite;
        sr.sortingLayerName = "Actors";
        sr.sortingOrder = sdd.PartSortingOrder;
        go.transform.localPosition = sdd.PartPosition;
    }
}
