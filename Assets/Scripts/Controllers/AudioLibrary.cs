using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class AudioLibrary : SerializedMonoBehaviour
{
    public enum ClipID { ButtonClickDown, ButtonClickUp, ButtonClickNegative, MetaPanelSlide, PanelCollide,
    ScannerPickup, ScannerDrop, WeaponToggle, InstallSystem, UpgradeSystem, ScrapSystem, GainScrap,
    GainUpgradePoint, UpgradePanelSlide, SelectSystem }
    public Dictionary<ClipID, AudioClip> _clipsByID = new Dictionary<ClipID, AudioClip>();

    public AudioClip GetClip(ClipID clipID)
    {
        if (_clipsByID.ContainsKey(clipID))
        {
            return _clipsByID[clipID];
        }

        else return null;
    }
}

