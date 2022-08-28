using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] AudioSource _playerAudioSource = null;

    public void PlayPlayerSound(AudioClip playerClip)
    {
        _playerAudioSource.PlayOneShot(playerClip);
    }

    public void PlayRemoteSound(AudioClip remoteClip, Vector2 soundPosition)
    {
        AudioSource.PlayClipAtPoint(remoteClip, soundPosition);
    }
}
