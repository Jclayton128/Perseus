using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialController : MonoBehaviour
{
    UI_Controller _uiController;
    InputController _inputController;
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

    float _timeDelayCompletionCriteria = 8f; //Time to show time-based steps.

    //state
    Tween _backgroundTween;
    Tween _fontTween;
    int _currentStepIndex = -1;
    TutorialStep _currentTutorialStep;
    TutorialStep.CompletionCriteria _currentCompletionCriteria;
    float _timeForTimebasedCompletion = Mathf.Infinity;

    private void Awake()
    {
        _uiController = GetComponent<UI_Controller>();
        _inputController = GetComponent<InputController>();
        AttachInputToCompletionCriteriaCheckers();
    }

    private void AttachInputToCompletionCriteriaCheckers()
    {
        _inputController.OnAccelBegin += HandleCompletedAccel;
        _inputController.OnDecelBegin += HandleCompletedDecel;
        _inputController.OnTurnLeft += HandleCompletedTurn;
        _inputController.OnTurnRight += HandleCompletedTurn;
        _inputController.OnScroll += HandleCompletedScrollSecondary;
        _inputController.OnMouseDown += HandleCompletedFireWeapon;

        _uiController.ScrapLevelIncreased += HandleScrapLevelIncreased;
        _uiController.UpgradePointsIncreased += HandleUpgradePointsIncreased;
        _uiController.DetectedStrongSignal += HandleCompletedReceivedStrongSignal;
    }

    private void Start()
    {
        HideTutorialPanel(true);
    }
    public void BeginTutorialSequence()
    {
        _currentStepIndex = 0;
        _currentTutorialStep = _tutorialSteps[_currentStepIndex];
        _currentCompletionCriteria = _currentTutorialStep.GetCompletionCriteria();

        DisplayCurrentTutorialStep();
    }

    public void EndTutorial()
    {
        HideTutorialPanel(true);
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
        HideTutorialPanel(false);

        _currentStepIndex++;
        _currentTutorialStep = _tutorialSteps[_currentStepIndex];
        _currentCompletionCriteria = _currentTutorialStep.GetCompletionCriteria();
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.Timed)
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
        _fontTween = _tutorialTMP.DOColor(_tutorialFontColor, _tutorialPanelDeployTime);
    }

    #endregion

    #region Completion Criteria Handlers
    private void HandleCompletedAccel()
    {
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.Accelerate)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleCompletedDecel()
    {
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.Decelerate)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleCompletedTurn(bool throwaway)
    {
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.Turn)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleCompletedFireWeapon(int mouseButton)
    {
        if (mouseButton == 0 &&
            _currentCompletionCriteria== TutorialStep.CompletionCriteria.FirePrimary)
        {
            IncrementCurrentTutorialStep();
            return;
        }
        if (mouseButton == 1 &&
            _currentCompletionCriteria == TutorialStep.CompletionCriteria.FireSecondary)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleScrapLevelIncreased()
    {
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.GainScrap)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleCompletedScrollSecondary(int throwaway)
    {
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.ScrollSecondary)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleUpgradePointsIncreased()
    {
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.GainUpgradePoint)
        {
            IncrementCurrentTutorialStep();
        }
    }

    private void HandleCompletedReceivedStrongSignal()
    {
        if (_currentCompletionCriteria == TutorialStep.CompletionCriteria.OnDetectSignal)
        {
            IncrementCurrentTutorialStep();
        }
    }
    #endregion
}
