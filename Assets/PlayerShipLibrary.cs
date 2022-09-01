using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipLibrary : MonoBehaviour
{
    [SerializeField] PlayerBrochure[] _playerShips = null;

    //state
    PlayerBrochure _currentlySelectedPlayerShip;
    public int GetPlayerShipCount()
    {
        return _playerShips.Length;
    }

    public (Sprite, string, string) GetPlayerShipDetails(int index)
    {
        if (index >= _playerShips.Length)
        {
            Debug.LogError("Asking for a player ship that isn't in the library");
            (Sprite, string, string) nu = (null, "", "");
            return nu;
        }

        return _playerShips[index].GetShipDetails();
    }

    public GameObject GetPlayerShip(int index)
    {
        if (index >= _playerShips.Length)
        {
            Debug.LogError("Asking for a player ship that isn't in the library");
            return null;
        }
        return _playerShips[index].gameObject;
    }

    public void UpdateSelectedPlayerShip(int index)
    {
        if (index >= _playerShips.Length)
        {
            Debug.LogError("Asking for a player ship that isn't in the library");
            return;
        }
        _currentlySelectedPlayerShip = _playerShips[index];
    }

    public GameObject GetSelectedPlayerShipPrefab()
    {
        return _currentlySelectedPlayerShip.gameObject;
    }
}
