using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using UnityEngine.UI;

public class AnalyticsController : MonoBehaviour
{
	GameController _gameController;
	LevelController _levelController;
	RunController _runController;

	[SerializeField] Toggle _analyticsOptinToggle = null;
	

	async void Start()
	{
		await UnityServices.InitializeAsync();

		_gameController = FindObjectOfType<GameController>();
		_levelController = FindObjectOfType<LevelController>();
		_runController = FindObjectOfType<RunController>();

		_gameController.PlayerSpawned += HandlePlayerSpawned;
		_gameController.PlayerDespawning += HandlePlayerDespawned;
	}

	void CheckForConsent()
	{
		Debug.Log("checking for consent");
		if (_analyticsOptinToggle.isOn)
        {
			ConsentGiven();
		}
	}

	void ConsentGiven()
	{
		AnalyticsService.Instance.StartDataCollection();
		Debug.Log("analytics ready to collect");
	}

	void HandlePlayerSpawned(GameObject throwaway)
    {
		Debug.Log("handle player spawned");
		CheckForConsent();

		_gameController.Player.GetComponentInChildren<HealthHandler>().Dying += FireEvent_PlayerDeath;

	}

	void HandlePlayerDespawned()
    {
		FireEvent_PlayerDeath();
    }

	private void FireEvent_PlayerDeath()
    {
		Debug.Log("transmitting analytics on player death");
		PlayerStateHandler psh = _gameController.Player.GetComponentInChildren<PlayerStateHandler>();


		Dictionary<string, object> parameters = new Dictionary<string, object>()
		{
			{ "ShipLevel", psh.ShipLevel },
			{"CurrentSectorCount", _runController.CurrentSectorCount}
		};

		AnalyticsService.Instance.CustomData("PlayerDeath", parameters);



	}
}

