using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using System;
using DG.Tweening;

public class UI_Controller : MonoBehaviour
{
    #region Scene References

    [FoldoutGroup("Meta Menu")]
    [SerializeField] Image _topMetaWing = null;

    [FoldoutGroup("Meta Menu")]
    [SerializeField] Image _bottomMetaWing = null;


    [FoldoutGroup("Systems & Weapons")]
    [SerializeField] SystemIconDriver[] _systemIcons = null;

    [FoldoutGroup("Systems & Weapons")]
    [SerializeField] WeaponIconDriver _primaryWeaponIcon = null;

    [FoldoutGroup("Systems & Weapons")]
    [SerializeField] WeaponIconDriver[] _secondaryWeaponIcons = null;

    [FoldoutGroup("Systems & Weapons")]
    [SerializeField] Sprite _primaryFireHintSprite = null;

    [FoldoutGroup("Systems & Weapons")]
    [SerializeField] Sprite _secondaryFireHintSprite = null;

    [FoldoutGroup("SHEI")]
    [SerializeField] AdjustableImageBar _shieldBar = null;

    [FoldoutGroup("SHEI")]
    [SerializeField] AdjustableImageBar _hullBar = null;

    [FoldoutGroup("SHEI")]
    [SerializeField] AdjustableImageBar _energyBar = null;


    [SerializeField] RadarScreen _radarScreen = null;

    [FoldoutGroup("Scrap & Upgrade Points")]
    [SerializeField] Image _scrapBarFill = null;

    [FoldoutGroup("Scrap & Upgrade Points")]
    [SerializeField] TextMeshProUGUI _scrapAmountTMP = null;

    [FoldoutGroup("Scrap & Upgrade Points")]
    [SerializeField] TextMeshProUGUI _levelTMP = null;

    [FoldoutGroup("Scrap & Upgrade Points")]
    [SerializeField] TextMeshProUGUI _tabTMP = null;

    [FoldoutGroup("Upgrade Menu")]
    [SerializeField] Image _leftUpgradeWing = null;

    [FoldoutGroup("Upgrade Menu")]
    [SerializeField] Image _rightUpgradeWing = null;

    [FoldoutGroup("Upgrade Menu")]
    [SerializeField] SystemSelectorDriver[] _systemSelectors = null;

    [FoldoutGroup("Upgrade Menu")]
    [SerializeField] Image _selectionImage = null;

    [FoldoutGroup("Upgrade Menu")]
    [SerializeField] TextMeshProUGUI _selectionNameTMP = null;

    [FoldoutGroup("Upgrade Menu")]
    [SerializeField] TextMeshProUGUI _selectionDescTMP = null;

    [FoldoutGroup("Upgrade Menu")]
    [SerializeField] TextMeshProUGUI _selectionUpgradeDescTMP = null;

    [FoldoutGroup("Upgrade Menu")]
    [SerializeField] TextMeshProUGUI _selectionUpgradeCostTMP = null;

    [FoldoutGroup("Upgrade Menu")]
    [SerializeField] Button _selectionUpgradeButton = null;

    [FoldoutGroup("Upgrade Menu")]
    [SerializeField] Button _installButton = null;

    [FoldoutGroup("Upgrade Menu")]
    [SerializeField] TextMeshProUGUI _installTMP = null;

    [FoldoutGroup("Upgrade Menu")]
    [SerializeField] Button _scrapButton = null;

    [FoldoutGroup("Upgrade Menu")]
    [SerializeField] TextMeshProUGUI _scrapRefundTMP = null;


    [FoldoutGroup("Meta Menu")]
    [SerializeField] TextMeshProUGUI _shipChoiceName = null;

    [FoldoutGroup("Meta Menu")]
    [SerializeField] TextMeshProUGUI _shipChoiceDescription = null;

    [FoldoutGroup("Meta Menu")]
    [SerializeField] Image _shipChoiceImage = null;

    [FoldoutGroup("Meta Menu")]
    [SerializeField] Image[] _shipChoiceOptionImages = null;

    #endregion

    PlayerStateHandler _playerStateHandler;
    GameController _gameController;
    PlayerShipLibrary _playerShipLibrary;

    public enum Context { None, Start, Core, End };

    //settings
    [FoldoutGroup("Scrap & Upgrade Points")]
    [SerializeField] float _minScrapFactor = 0.13f;

    [FoldoutGroup("Scrap & Upgrade Points")]
    [SerializeField] float _maxScrapFactor = 0.87f;

    [FoldoutGroup("Upgrade Menu")]
    [SerializeField] float _upgradeMenuDeployTime;  //.7f is nice.

    [FoldoutGroup("Upgrade Menu")]
    [SerializeField] float _upgradeWingTraverseDistance;

    [FoldoutGroup("Meta Menu")]
    [SerializeField] float _metaMenuTraverseDistance;

    [FoldoutGroup("Meta Menu")]
    [SerializeField] float _metaMenuDeployTime = 1.2f;



    //state
    IUpgradeable _currentUpgradeableSelection;
    Image _currentActiveSecondary;
    Tween _upgradeWingsTween_left;
    Tween _upgradeWingsTween_right;
    Tween _topMetaTween;
    Tween _bottomMetaTween;

    #region Initialization
    private void Awake()
    {
        _gameController = GetComponent<GameController>();
        _playerShipLibrary = FindObjectOfType<PlayerShipLibrary>();
        _gameController.OnPlayerSpawned += ReactToPlayerSpawning;
        InitializeSystemWeaponIcons();
        InitializeShipSelection();
    }

    private void ReactToPlayerSpawning(GameObject player)
    {
        _playerStateHandler = player.GetComponent<PlayerStateHandler>();
        ClearSelection();
    }


    private void InitializeSystemWeaponIcons()
    {
        foreach (var sid in _systemIcons)
        {
            sid.Initialize();
        }

        _primaryWeaponIcon.Initialize();

        foreach (var wid in _secondaryWeaponIcons)
        {
            wid.Initialize(_primaryFireHintSprite, _secondaryFireHintSprite);
        }
    }

    private void InitializeShipSelection()
    {
        foreach (var image in _shipChoiceOptionImages)
        {
            image.GetComponentInParent<Button>().interactable = false;
            image.color = Color.clear;
        }

        _shipChoiceImage.color = Color.clear;
        _shipChoiceName.text = "";
        _shipChoiceDescription.text = "Select your ship from below.";
        

        if (_playerShipLibrary.GetPlayerShipCount() > _shipChoiceOptionImages.Length)
        {
            Debug.LogError("More player ship options than places to display them!");
        }

        for (int i = 0; i < _playerShipLibrary.GetPlayerShipCount(); i++)
        {
            Sprite sprite = _playerShipLibrary.GetPlayerShipDetails(i).Item1;
            if (sprite != null)
            {
                _shipChoiceOptionImages[i].sprite = sprite;
                _shipChoiceOptionImages[i].color = Color.white;
                _shipChoiceOptionImages[i].GetComponentInParent<Button>().interactable = true;
            }
        }

    }

    #endregion

    #region Meta Menu

    public void DeployMetaMenu()
    {
        _topMetaTween.Kill();
        _bottomMetaTween.Kill();

        _topMetaTween = _topMetaWing.rectTransform.DOAnchorPosY(-_metaMenuTraverseDistance,
            _metaMenuDeployTime).SetEase(Ease.InOutQuad).SetUpdate(true);
        _bottomMetaTween = _bottomMetaWing.rectTransform.DOAnchorPosY(_metaMenuTraverseDistance,
            _metaMenuDeployTime).SetEase(Ease.InOutQuad).SetUpdate(true);

    }

    public void RetractMetaMenu()
    {
        _shipChoiceImage.color = Color.clear;

        _topMetaTween.Kill();
        _bottomMetaTween.Kill();

        _topMetaTween = _topMetaWing.rectTransform.DOAnchorPosY(_metaMenuTraverseDistance,
            _metaMenuDeployTime).SetEase(Ease.InOutQuad).SetUpdate(true);
        _bottomMetaTween = _bottomMetaWing.rectTransform.DOAnchorPosY(-_metaMenuTraverseDistance,
            _metaMenuDeployTime).SetEase(Ease.InOutQuad).SetUpdate(true);

    }

    public void InstantDeployMetaMenu()
    {
        _topMetaWing.rectTransform.anchoredPosition = new Vector2(0, -_metaMenuTraverseDistance);
        _bottomMetaWing.rectTransform.anchoredPosition = new Vector2(0, _metaMenuTraverseDistance);
    }

    public void HandleStartNewGamePress()
    {
        _gameController.SetupNewGame();
    }

    public void HandleExitToMenuPress()
    {
        _gameController.EndGameOnPlayerChoice();
    }

    public void HandleSelectShip(int index)
    {
        (Sprite, string, string) details = _playerShipLibrary.GetPlayerShipDetails(index);
        _shipChoiceImage.sprite = details.Item1;
        _shipChoiceName.text = details.Item2;
        _shipChoiceDescription.text = details.Item3;

        _playerShipLibrary.UpdateSelectedPlayerShip(index);
    }

    public void FlashShipSelectionDescription()
    {
        _shipChoiceDescription.rectTransform.DOLocalJump(Vector2.up, 14f, 1, 0.7f).SetUpdate(true);
    }

    #endregion

    #region Scrap and Upgrade Points

    public void ModifyScrapAmount(float scrapFillFactor, int totalScrapAmount)
    {
        if (scrapFillFactor < 0 || scrapFillFactor > 1f)
        {
            Debug.LogError("Invalid Scrap fill factor");
        }

        _scrapAmountTMP.text = totalScrapAmount.ToString();
        _scrapBarFill.fillAmount = Mathf.Lerp(_minScrapFactor, _maxScrapFactor, scrapFillFactor);
    }

    public void ModifyUpgradePointsAvailable(int newLevel)
    {
        _levelTMP.text = newLevel.ToString();
    }

    public void ShowHideTAB(bool shouldShow)
    {
        _tabTMP.text = (shouldShow) ? "TAB" : "";
    }

    #endregion

    #region Upgrade Menu

    public void DeployUpgradeMenu()
    {
        _currentUpgradeableSelection = null;
        _upgradeWingsTween_left.Kill();
        _upgradeWingsTween_right.Kill();

        _upgradeWingsTween_left = _leftUpgradeWing.rectTransform.DOAnchorPosX(_upgradeWingTraverseDistance,
            _upgradeMenuDeployTime).SetEase(Ease.InOutQuad).SetUpdate(true);
        _upgradeWingsTween_right = _rightUpgradeWing.rectTransform.DOAnchorPosX(-_upgradeWingTraverseDistance,
            _upgradeMenuDeployTime).SetEase(Ease.InOutQuad).SetUpdate(true);

        _selectionUpgradeButton.interactable = false;
        _installButton.interactable = false;
        _installTMP.text = "-";
        _scrapButton.interactable = false;
        _scrapRefundTMP.text = "-";

        DeploySelectors();
    }


    public void RetractUpgradeMenu()
    {
        _upgradeWingsTween_left.Kill();
        _upgradeWingsTween_right.Kill();

        _leftUpgradeWing.rectTransform.DOAnchorPosX(-_upgradeWingTraverseDistance,
            _upgradeMenuDeployTime).SetEase(Ease.InOutQuad).SetUpdate(true);
        _rightUpgradeWing.rectTransform.DOAnchorPosX(_upgradeWingTraverseDistance,
            _upgradeMenuDeployTime).SetEase(Ease.InOutQuad).SetUpdate(true);

        RetractSelectors();
    }

    private void DeploySelectors()
    {
        foreach (SystemSelectorDriver driver in _systemSelectors)
        {
            driver.DeploySelector();
        }
    }

    private void RetractSelectors()
    {
        foreach (SystemSelectorDriver driver in _systemSelectors)
        {
            driver.RetractSelector();
        }
    }

    public void ClearSelection()
    {
        _currentUpgradeableSelection = null;
        _selectionUpgradeButton.interactable = false;
        _selectionImage.color = Color.clear;
        _selectionImage.sprite = null;
        _selectionNameTMP.text = "-";
        _selectionDescTMP.text = null;
        _selectionUpgradeDescTMP.text = null;
        _selectionUpgradeCostTMP.text = "-";
    }

    public void UpdateSelection(IUpgradeable upgradeableThing)
    {
        _currentUpgradeableSelection = upgradeableThing;
        (Sprite, string, string, string, int) selectionInfo = upgradeableThing.GetUpgradeDetails();

        _selectionUpgradeButton.interactable = CheckIfUpgradeButtonShouldBeInteractable(_currentUpgradeableSelection);

        _selectionImage.color = Color.white;
        _selectionImage.sprite = selectionInfo.Item1;
        _selectionNameTMP.text = selectionInfo.Item2;
        _selectionDescTMP.text = selectionInfo.Item3;
        _selectionUpgradeDescTMP.text = selectionInfo.Item4;
        if (selectionInfo.Item5 < 0)
        {
            _selectionUpgradeCostTMP.text = "-";
        }
        else
        {
            _selectionUpgradeCostTMP.text = selectionInfo.Item5.ToString();
        }

        bool isInstalled = _currentUpgradeableSelection.CheckIfInstalled();
        bool isScrappable = _currentUpgradeableSelection.CheckIfScrappable();

        _installButton.interactable = !isInstalled;
        _installTMP.text = (isInstalled) ? "-" : "1";

        _scrapButton.interactable = isScrappable;
        string amount = _currentUpgradeableSelection.GetScrapRefundAmount().ToString();
        _scrapRefundTMP.text = (isScrappable) ? amount : "-";

    }

    private bool CheckIfUpgradeButtonShouldBeInteractable(IUpgradeable currentUpgradeableSelection)
    {
        if (currentUpgradeableSelection == null) return false;

        bool hasMoreUpgradesAvailable = _currentUpgradeableSelection.CheckIfHasRemainingUpgrades();
        bool canAffordToUpgrade = _playerStateHandler.CheckUpgradePoints(_currentUpgradeableSelection.GetUpgradeCost());

        if ( hasMoreUpgradesAvailable && canAffordToUpgrade )
        {
            return true;
        }
        else return false;
    }


    public void HandleSelectedUpgrade()
    {
        int currentSelectionUpgradeCost = _currentUpgradeableSelection.GetUpgradeCost();
        _playerStateHandler.SpendUpgradePoints(currentSelectionUpgradeCost);
        _currentUpgradeableSelection.Upgrade();
        UpdateSelection(_currentUpgradeableSelection);
        
    }
    

    #endregion

    #region System Icons
    public SystemIconDriver IntegrateNewSystem(SystemHandler sh)
    {
        //if (index < 0 || index >= _systemIcons.Length)
        //{
        //    Debug.Log("invalid system integration index");
        //    return;
        //}
        bool foundOpenUISlot = false;
        SystemIconDriver sid = null;
        for (int i = 0; i < _systemIcons.Length; i++)
        {
            if (_systemIcons[i].IsOccupied) continue;
            else
            {
                _systemIcons[i].DisplayNewSystem(sh);
                sid = _systemIcons[i];
                foundOpenUISlot = true;
                break;
            }
        }
        if (foundOpenUISlot == false)
        {
            Debug.Log("did not find an open System slot on UI");
        }
        return sid;
    }

    public void ClearAllSystemSlots()
    {
        foreach (var slot in _systemIcons)
        {
            slot.ClearUIIcon();
        }
    }

    public void ClearSystemSlot(SystemWeaponLibrary.SystemType systemToRemove)
    {
        Debug.Log($"trying to clear {systemToRemove}");
        foreach (var systemIcon in _systemIcons)
        {
            if (systemIcon.System == systemToRemove)
            {
                systemIcon.ClearUIIcon();
                return;
            }
        }
    }

    #endregion

    #region Weapon Icons

    public void HighlightNewSecondaryWeapon(int index)
    {
        foreach (var sid in _secondaryWeaponIcons)
        {
            sid.DehighlightAsActive();
        }
        _secondaryWeaponIcons[index].HighlightAsActive();
    }
    public WeaponIconDriver IntegrateNewWeapon(WeaponHandler wh)
    {
        if (wh == null)
        {
            Debug.LogError("WeaponHandler passed is null!");
            return null;
        }
        if (wh.IsSecondary)
        {
            for (int i = 0; i < _secondaryWeaponIcons.Length; i++)
            {
                if (_secondaryWeaponIcons[i].IsOccupied) continue;
                else
                {
                    _secondaryWeaponIcons[i].DisplayNewWeapon(wh);
                    return _secondaryWeaponIcons[i];
                }
            }
            Debug.Log("did not find an open Secondary Weapon slot on UI");
            return null;

        }
        else
        {
            if (_primaryWeaponIcon.IsOccupied)
            {
                Debug.Log("Primary weapon slot was occupied, but is now overwritten");
            }
            _primaryWeaponIcon.DisplayNewWeapon(wh);
            _primaryWeaponIcon.HighlightAsActive();
            return _primaryWeaponIcon;
        }
        

    }

    public void ClearPrimaryWeaponSlot()
    {
        _primaryWeaponIcon.ClearUIIcon();
        _primaryWeaponIcon.DehighlightAsActive();
    }

    public void ClearAllSecondaryWeaponSlots()
    {
        foreach (var icon in _secondaryWeaponIcons)
        {
            icon.ClearUIIcon();
        }
    }

    #endregion

    #region Public Gets
    public int GetMaxSystems()
    {
        return _systemIcons.Length;
    }

    public int GetMaxWeapons()
    {
        return _secondaryWeaponIcons.Length;
    }

    public RadarScreen GetRadarScreen()
    {
        return _radarScreen;
    }

    public AdjustableImageBar GetShieldBar()
    {
        return _shieldBar;
    }

    public AdjustableImageBar GetHullBar()
    {
        return _hullBar;
    }

    public AdjustableImageBar GetEnergyBar()
    {
        return _energyBar;
    }

    #endregion
}
