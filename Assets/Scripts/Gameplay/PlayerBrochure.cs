using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBrochure : MonoBehaviour
{
    [SerializeField] Sprite _icon = null;
    [SerializeField] string _name = "default ship name";
    [SerializeField][Multiline(3)] string _description = "default ship description";

    public (Sprite, string, string) GetShipDetails()
    {
        (Sprite, string, string) newDetails;

        newDetails.Item1 = _icon;
        newDetails.Item2 = _name;
        newDetails.Item3 = _description;

        return newDetails;
    }


}
