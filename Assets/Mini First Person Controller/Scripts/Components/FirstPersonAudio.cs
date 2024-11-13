﻿using System.Linq;
using UnityEngine;

public class FirstPersonAudio : MonoBehaviour
{
    public FirstPersonMovement character;
    public GroundCheck groundCheck;

    [Tooltip("Minimum velocity for moving audio to play")]
    /// <summary> "Minimum velocity for moving audio to play" </summary>
    public float velocityThreshold = .01f;
    Vector2 lastCharacterPosition;
    Vector2 CurrentCharacterPosition => new Vector2(character.transform.position.x, character.transform.position.z);

    [SerializeField] private Jump jump;
    [SerializeField] private Crouch crouch;

    [SerializeField] private AK.Wwise.Event stepsStartEvent;
    [SerializeField] private AK.Wwise.Event stepsStopEvent;
    [SerializeField] private AK.Wwise.Event crouchEvent;
    [SerializeField] private AK.Wwise.Event jumpEvent;
    [SerializeField] private AK.Wwise.Event landEvent;


    private bool _stepsPlaying;

    void Reset()
    {
        // Setup stuff.
        character = GetComponentInParent<FirstPersonMovement>();
        groundCheck = (transform.parent ?? transform).GetComponentInChildren<GroundCheck>();

        // Setup jump audio.
        jump = GetComponentInParent<Jump>();

        // Setup crouch audio.
        crouch = GetComponentInParent<Crouch>();
    }

    void OnEnable() => SubscribeToEvents();

    void OnDisable() => UnsubscribeToEvents();

    void FixedUpdate()
    {
        // Play moving audio if the character is moving and on the ground.
        float velocity = Vector3.Distance(CurrentCharacterPosition, lastCharacterPosition);
        if (velocity >= velocityThreshold && groundCheck && groundCheck.isGrounded)
        {
            if (crouch && crouch.IsCrouched)
            {
                crouchEvent.Post(gameObject);
            }
            else
            {
                if (!_stepsPlaying)
                {
                    stepsStartEvent.Post(gameObject);
                    _stepsPlaying = true;
                }
            }
        }
        else
        {
            if (_stepsPlaying)
            {
                stepsStopEvent.Post(gameObject);
                _stepsPlaying = false;
            }
        }

        lastCharacterPosition = CurrentCharacterPosition;
    }


    #region Play instant-related audios.

    void PlayLandingAudio() => landEvent.Post(gameObject);
    void PlayJumpAudio() => jumpEvent.Post(gameObject);
    void PlayCrouchStartAudio() => crouchEvent.Post(gameObject);

    #endregion

    #region Subscribe/unsubscribe to events.

    void SubscribeToEvents()
    {
        // PlayLandingAudio when Grounded.
        groundCheck.Grounded += PlayLandingAudio;

        // PlayJumpAudio when Jumped.
        if (jump)
        {
            jump.Jumped += PlayJumpAudio;
        }

        // Play crouch audio on crouch start/end.
        if (crouch)
        {
            crouch.CrouchStart += PlayCrouchStartAudio;
        }
    }

    void UnsubscribeToEvents()
    {
        // Undo PlayLandingAudio when Grounded.
        groundCheck.Grounded -= PlayLandingAudio;

        // Undo PlayJumpAudio when Jumped.
        if (jump)
        {
            jump.Jumped -= PlayJumpAudio;
        }

        // Undo play crouch audio on crouch start/end.
        if (crouch)
        {
            crouch.CrouchStart -= PlayCrouchStartAudio;
        }
    }

    #endregion

    #region Utility.

    /// <summary>
    /// Get an existing AudioSource from a name or create one if it was not found.
    /// </summary>
    /// <param name="name">Name of the AudioSource to search for.</param>
    /// <returns>The created AudioSource.</returns>
    AudioSource GetOrCreateAudioSource(string name)
    {
        // Try to get the audiosource.
        AudioSource result = System.Array.Find(GetComponentsInChildren<AudioSource>(), a => a.name == name);
        if (result)
            return result;

        // Audiosource does not exist, create it.
        result = new GameObject(name).AddComponent<AudioSource>();
        result.spatialBlend = 1;
        result.playOnAwake = false;
        result.transform.SetParent(transform, false);
        return result;
    }

    static void PlayRandomClip(AudioSource audio, AudioClip[] clips)
    {
        if (!audio || clips.Length <= 0)
            return;

        // Get a random clip. If possible, make sure that it's not the same as the clip that is already on the audiosource.
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clips.Length > 1)
            while (clip == audio.clip)
                clip = clips[Random.Range(0, clips.Length)];

        // Play the clip.
        audio.clip = clip;
        audio.Play();
    }

    #endregion
}