using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioController : MonoBehaviour
{
    AudioLibrary _audioLibrary;
    [SerializeField] AudioSource _cameraAudioSource = null;


    private void Awake()
    {
        //_cameraAudioSource = Camera.main.GetComponent<AudioSource>();
        _audioLibrary = FindObjectOfType<AudioLibrary>();
    }

    public void PlayUIClip(AudioLibrary.ClipID clipID)
    {
        AudioClip clip = _audioLibrary.GetClip(clipID);
        _cameraAudioSource.PlayOneShot(clip);
    }

    public void PlayClipAtPlayer(AudioClip clip)
    {
        if (GameController.IsPaused == false)
        {
            _cameraAudioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.Log("Can't play gameplay clips while paused");
        }

    }

    public void PlayClipAtPlayer(AudioClip clip, float volumeScale)
    {
        if (GameController.IsPaused == false)
        {
            _cameraAudioSource.PlayOneShot(clip, volumeScale);
        }
        else
        {
            Debug.Log("Can't play gameplay clips while paused");
        }
    }


}
