using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] AudioSource _playerAudioSource = null;
    [SerializeField] AudioSource _remoteAudioSource = null;

    public void PlayPlayerSound(AudioClip playerClip)
    {
        _playerAudioSource.PlayOneShot(playerClip);
    }

    public void PlayRemoteSound(AudioClip remoteClip, Vector2 soundPosition)
    {
        _remoteAudioSource.transform.position = soundPosition;
        _remoteAudioSource.PlayOneShot(remoteClip);
    }
}
