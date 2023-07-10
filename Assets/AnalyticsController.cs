using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Services.Core.Environments;

public class AnalyticsController : MonoBehaviour
{
	GameController _gameController;
	LevelController _levelController;
	RunController _runController;

	[SerializeField] Toggle _analyticsOptinToggle = null;
	

	async void Start()
	{

		var options = new InitializationOptions();
		options.SetEnvironmentName("production");

		await UnityServices.InitializeAsync(options);

		_gameController = FindObjectOfType<GameController>();
		_levelController = FindObjectOfType<LevelController>();
		_runController = FindObjectOfType<RunController>();

		_gameController.PlayerSpawned += HandlePlayerSpawned;
		_gameController.PlayerDespawning += HandlePlayerDespawned;
	}

	void CheckForConsent()
	{
		if (_analyticsOptinToggle.isOn)
        {
			ConsentGiven();
		}
	}

	void ConsentGiven()
	{
		AnalyticsService.Instance.StartDataCollection();
	}

	void HandlePlayerSpawned(GameObject throwaway)
    {
		CheckForConsent();

		_gameController.Player.GetComponentInChildren<HealthHandler>().Dying += FireEvent_PlayerDeath;

		PlayerSystemHandler psh = _gameController.Player.GetComponentInChildren<PlayerSystemHandler>();
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
			{"CurrentSectorCount", _runController.CurrentSectorCount},
			{"CurrentSectorName", _levelController.CurrentLevel.LevelName }
		};

		AnalyticsService.Instance.CustomData("PlayerDeath", parameters);
	}

	private void FireEvent_UpgradeSystem()
    {

    }

	private void FireEvent_UpgradeWeapon()
    {

    }

	private void FireEvent_InstallWeapon()
    {

    }

	private void FireEvent_InstallSystem()
    {

    }
}

