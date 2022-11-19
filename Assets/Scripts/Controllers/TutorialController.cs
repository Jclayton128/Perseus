using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    UI_Controller _uiController;
    InputController _inputController;
    LevelController _levelController;
    [SerializeField] RectTransform _tutorialPanel = null;

    [Tooltip("TopLeft, TopMiddle, TopRight, BottomLeft, BottomMiddle, BottomRight")]
    [SerializeField] Vector2[] _tutorialPositions = null;

    [SerializeField] TutorialStep[] _tutorialSteps = null;

    [SerializeField] Image _tutorialPanelBackground = null;
    [SerializeField] TextMeshProUGUI _tutorialTMP = null;

    //settings
    [SerializeField] Color _tutorialFontColor = Color.white;
    [SerializeField] Color _tutorialPanelColor = Color.white;
    [SerializeField] float _tutorialPanelDeployTime = 1.0f;

    [SerializeField] float _timeDelayCompletionCriteria = 5f; //Time to show time-based steps.

    //state
    bool _isInTutorial = false;
    public bool IsInTutorial => _isInTutorial;
    Tween _backgroundTween;
    Tween _fontTween;
    [SerializeField] int _currentStepIndex = -1;
    [SerializeField] TutorialStep _currentTutorialStep;
    [SerializeField] TutorialStep.CompletionCriteria _currentCompletionCriteria;
    float _timeForTimebasedCompletion = Mathf.Infinity;
    Dictionary<TutorialStep.CompletionCriteria, bool> _completionState =
        new Dictionary<TutorialStep.CompletionCriteria, bool>();
    TutorialStep.Outcome _currentOutcome;

    private void Awake()
    {
        _uiController = GetComponent<UI_Controller>();
        _levelController = GetComponent<LevelController>();
        _inputController = GetComponent<InputController>();
        AttachInputToCompletionCriteriaCheckers();
    }

    private void InitializeCompletionStateDictionary()
    {
        _completionState.Clear();
        foreach (var tutStep in _tutorialSteps)
        {
            if (!_completionState.ContainsKey(tutStep.GetCompletionCriteria()))
            {
                _completionState.Add(tutStep.GetCompletionCriteria(), false);
            }
        }
    }

    private void AttachInputToCompletionCriteriaCheckers()
    {
        _inputController.AccelStarted += HandleCompletedAccel;
        _inputController.DecelStarted += HandleCompletedDecel;
        _inputController.TurnLeftChanged += HandleCompletedTurn;
        _inputController.TurnRightChanged += HandleCompletedTurn;
        _inputController.ScrollWheelChanged += HandleCompletedScrollSecondary;
        _inputController.LeftMouseChanged += HandleCompletedFirePrimary;
        _inputController.RightMouseChanged += HandleCompletedFireSecondary;

        _levelController.EnemyLevelCountChanged += HandleEnemyCountChanged;
        _levelController.WarpingIntoNewLevel += HandleWarpIntoNewLevel;
        _uiController.ScrapLevelIncreased += HandleScrapLevelIncreased;
        _uiController.UpgradePointsIncreased += HandleUpgradePointsIncreased;
        _uiController.DetectedStrongSignal += HandleCompletedReceivedStrongSignal;
        _uiController.ScannerScannedSomething += HandleScannerScanned;
        _uiController.ScannerScannedCrate += HandleScannedCrate;
        _uiController.UpgradeMenuOpened += HandleUpgradeMenuOpened;
    }

   
    private void DeattachInput()
    {
        _inputController.AccelStarted -= HandleCompletedAccel;
        _inputController.DecelStarted -= HandleCompletedDecel;
        _inputController.TurnLeftChanged -= HandleCompletedTurn;
        _inputController.TurnRightChanged -= HandleCompletedTurn;
        _inputController.ScrollWheelChanged -= HandleCompletedScrollSecondary;
        _inputController.LeftMouseChanged -= HandleCompletedFirePrimary;
        _inputController.RightMouseChanged -= HandleCompletedFireSecondary;

        _levelController.EnemyLevelCountChanged -= HandleEnemyCountChanged;
        _levelController.WarpingIntoNewLevel -= HandleWarpIntoNewLevel;
        _uiController.ScrapLevelIncreased -= HandleScrapLevelIncreased;
        _uiController.UpgradePointsIncreased -= HandleUpgradePointsIncreased;
        _uiController.DetectedStrongSignal -= HandleCompletedReceivedStrongSignal;
        _uiController.ScannerScannedSomething -= HandleScannerScanned;
        _uiController.ScannerScannedCrate -= HandleScannedCrate;
        _uiController.UpgradeMenuOpened -= HandleUpgradeMenuOpened;

    }

    private void Start()
    {
        HideTutorialPanel(true);
    }

    public void BeginTutorialSequence()
    {
        InitializeCompletionStateDictionary();
        _currentStepIndex = 0;
        _currentTutorialStep = _tutorialSteps[_currentStepIndex];
        _currentOutcome = _currentTutorialStep.GetOutcome();
        _currentCompletionCriteria = _currentTutorialStep.GetCompletionCriteria();
        _isInTutorial = true;
        DisplayCurrentTutorialStep();
    }

    public void EndTutorial()
    {
        DeattachInput();
        HideTutorialPanel(true);
        _isInTutorial = false;
    }

    #region Flow
    private void Update()
    {
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.Timed &&
            Time.time >= _timeForTimebasedCompletion)
        {
            IncrementCurrentTutorialStep();
            _timeForTimebasedCompletion = Time.time + _timeDelayCompletionCriteria;
        }
    }
    private void IncrementCurrentTutorialStep()
    {
        if (_currentStepIndex == _tutorialSteps.Length - 1)
        {
            EndTutorial();
            return;
        }

        ExecuteOutcome(_currentOutcome);
        HideTutorialPanel(false);

        _currentStepIndex++;
        _currentTutorialStep = _tutorialSteps[_currentStepIndex];
        _currentCompletionCriteria = _currentTutorialStep.GetCompletionCriteria();
        _currentOutcome = _currentTutorialStep.GetOutcome();

        if (_completionState[_currentCompletionCriteria] == true)
        {
            //Debug.Log($"Skipping tutStep with completion of {_currentCompletionCriteria} due to already doing it");
            IncrementCurrentTutorialStep();
        }
        else if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.Timed)
        {
            _timeForTimebasedCompletion = Time.time + _timeDelayCompletionCriteria;
        }
        
        Invoke(nameof(DisplayCurrentTutorialStep), _tutorialPanelDeployTime * 2f);
    }

    #endregion

    #region Panel Visuals

    private void HideTutorialPanel(bool withInstantTransition)
    {
        if (withInstantTransition)
        {
            _backgroundTween.Kill();
            _tutorialPanelBackground.color = Color.clear;
            _fontTween.Kill();
            _tutorialTMP.color = Color.clear;
        }
        else
        {
            _backgroundTween.Kill();
            _backgroundTween = _tutorialPanelBackground.DOColor(Color.clear, _tutorialPanelDeployTime);
            _fontTween.Kill();
            _fontTween = _tutorialTMP.DOColor(Color.clear, _tutorialPanelDeployTime);
        }

    }

    private void DisplayCurrentTutorialStep()
    {
        int locationAsInt = (int)_currentTutorialStep.GetLocation();
        // LocationAsInt of 6 means None, which means the panel should remain undisplayed
        if (locationAsInt != 6)
        {
            _tutorialPanel.anchoredPosition = _tutorialPositions[locationAsInt];
            _backgroundTween.Kill();
            _backgroundTween = _tutorialPanelBackground.DOColor(_tutorialPanelColor, _tutorialPanelDeployTime);
            SetAndDisplayTutorialPanelText(_currentTutorialStep.GetText());
        }
    }


    private void SetAndDisplayTutorialPanelText(string text)
    {
        _tutorialTMP.text = text;
        _fontTween = _tutorialTMP.DOColor(_tutorialFontColor, _tutorialPanelDeployTime).SetUpdate(true);
    }

    #endregion

    #region Outcomes

    private void ExecuteOutcome(TutorialStep.Outcome outcome)
    {
        switch (outcome)
        {
            case TutorialStep.Outcome.Nothing:
                break;

            case TutorialStep.Outcome.UnlockWormhole:
                _levelController.UnlockTutorialWormhole();
                break;

            case TutorialStep.Outcome.WeakenEnemy:
                _levelController.WeakenTutorialEnemy();
                break;
        }
    }
    #endregion

    #region Completion Criteria Handlers

    private void HandleEnemyCountChanged(int newCount)
    {
        if (!_isInTutorial || newCount > 0) return;
        _completionState[TutorialStep.CompletionCriteria.OnKillEnemy] = true;
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.OnKillEnemy)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleCompletedAccel()
    {
        if (!_isInTutorial) return;
        _completionState[TutorialStep.CompletionCriteria.Accelerate] = true;
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.Accelerate)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleCompletedDecel()
    {
        if (!_isInTutorial) return;
        _completionState[TutorialStep.CompletionCriteria.Decelerate] = true;
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.Decelerate)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleCompletedTurn(bool throwaway)
    {
        if (!_isInTutorial) return;
        _completionState[TutorialStep.CompletionCriteria.Turn] = true;
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.Turn)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleCompletedFirePrimary(bool throwaway)
    {
        if (!_isInTutorial) return;
        _completionState[TutorialStep.CompletionCriteria.FirePrimary] = true;
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.FirePrimary)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleCompletedFireSecondary(bool throwaway)
    {
        if (!_isInTutorial) return;
        _completionState[TutorialStep.CompletionCriteria.FireSecondary] = true;
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.FireSecondary)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleScrapLevelIncreased()
    {
        if (!_isInTutorial) return;
        _completionState[TutorialStep.CompletionCriteria.GainScrap] = true;
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.GainScrap)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleCompletedScrollSecondary(int throwaway)
    {
        if (!_isInTutorial) return;
        _completionState[TutorialStep.CompletionCriteria.ScrollSecondary] = true;
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.ScrollSecondary)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleUpgradePointsIncreased()
    {
        if (!_isInTutorial) return;
        _completionState[TutorialStep.CompletionCriteria.GainUpgradePoint] = true;
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.GainUpgradePoint)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleCompletedReceivedStrongSignal()
    {
        if (!_isInTutorial) return;
        _completionState[TutorialStep.CompletionCriteria.OnDetectSignal] = true;
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.OnDetectSignal)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleUpgradeMenuOpened()
    {
        if (!_isInTutorial) return;
        // Don't skip this; Used multiple times.
        //_completionState[TutorialStep.CompletionCriteria.OpenUpgradeMenu] = true;
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.OpenUpgradeMenu)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleScannerScanned()
    {
        if (!_isInTutorial) return;
        _completionState[TutorialStep.CompletionCriteria.OnScan] = true;
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.OnScan)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleWarpIntoNewLevel()
    {
        if (!_isInTutorial) return;
        _completionState[TutorialStep.CompletionCriteria.OnWarpIntoNewLevel] = true;
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.OnWarpIntoNewLevel)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleScannedCrate()
    {
        if (!_isInTutorial) return;
        // Don't skip this one; force it.
        //_completionState[TutorialStep.CompletionCriteria.CrateSeen] = true;
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.CrateSeen)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleIonized()
    {
        if (!_isInTutorial) return;
        _completionState[TutorialStep.CompletionCriteria.Ionized] = true;
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.Ionized)
        {
            IncrementCurrentTutorialStep();
        }
    }


    #endregion
}
