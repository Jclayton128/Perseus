using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicController : MonoBehaviour
{
    public static MusicController Instance { get; private set; }

    [SerializeField] AudioSource _musicAudioSource = null;
    [SerializeField] AudioSource _warpAudioSource = null;
    [SerializeField] AudioClip _warpClip = null;
    [SerializeField] AudioClip _startClip = null;
    [SerializeField] AudioClip _startBong = null;

    //settings
    [SerializeField][Range(0f, 1f)] float _musicVolume_max = 0.5f;
    Tween _musicAusoTween;
    Tween _warpAusoTween;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _musicAudioSource.PlayOneShot(_startBong);
        _musicAudioSource.PlayOneShot(_startClip);
    }

    public void PlayNewMusic(AudioClip musicClip)
    {
        _musicAudioSource.volume = _musicVolume_max;
        _musicAudioSource.clip = musicClip;
        _musicAudioSource.Play();

    }

    public void BeginWarpInMusic()
    {
        _musicAusoTween.Kill();
        _musicAusoTween = _musicAudioSource.DOFade(0, _warpClip.length / 2f);
        _warpAusoTween.Kill();
        _warpAudioSource.volume = 1;
        _warpAudioSource.clip = _warpClip;
        _warpAudioSource.Play();
    }

    public void CeaseWarp()
    {
        _warpAusoTween.Kill();
        _warpAusoTween = _warpAudioSource.DOFade(0, 1f);

        _musicAusoTween.Kill();
        _musicAusoTween = _musicAudioSource.DOFade(_musicVolume_max, 1f);
    }

}
