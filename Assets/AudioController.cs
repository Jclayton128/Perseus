using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    
    AudioSource _playerAudioSource;

    private void Awake()
    {
        _playerAudioSource = Camera.main.GetComponent<AudioSource>();
    }

    public void PlayGameplayClipForPlayer(AudioClip clip)
    {
        if (GameController.IsPaused == false)
        {
            _playerAudioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.Log("Can't play gameplay clips while paused");
        }

    }


}
