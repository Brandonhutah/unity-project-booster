﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 125f;
    [SerializeField] float sceneTransitionSeconds = 1f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;
    [SerializeField] ParticleSystem engineSystem;
    [SerializeField] ParticleSystem deathSystem;
    [SerializeField] ParticleSystem successSystem;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    Rigidbody rigidBody;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    void ProcessInput()
    {
        if (state == State.Alive)
        {
            CheckThrustInput();
            CheckRotateInput();
            CheckDebugInput();
        }
    }

    private void CheckDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartFinishSequence();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            rigidBody.detectCollisions = !rigidBody.detectCollisions;
        }
    }

    private void CheckThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            if (engineSystem.isPlaying)
            {
                engineSystem.Stop();
            }
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * (mainThrust * Time.deltaTime));
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        if (!engineSystem.isPlaying)
        {
            engineSystem.Play();
        }
    }

    private void CheckRotateInput()
    {
        rigidBody.angularVelocity = Vector3.zero;

        if (!(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)))
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(Vector3.forward * (rcsThrust * Time.deltaTime));
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(Vector3.back * (rcsThrust * Time.deltaTime));
            }
        }
    }

    private void LoadNextScene()
    {
        switch(state)
        {
            case State.Transcending:
                int nextScene = SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1 ? SceneManager.GetActiveScene().buildIndex + 1 : 0;
                SceneManager.LoadScene(nextScene);
                break;
            case State.Dying:
                SceneManager.LoadScene(0);
                break;
        }

        state = State.Alive;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartFinishSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartFinishSequence()
    {
        state = State.Transcending;
        engineSystem.Stop();
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successSystem.Play();
        Invoke("LoadNextScene", sceneTransitionSeconds);
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        engineSystem.Stop();
        audioSource.Stop();
        audioSource.PlayOneShot(death, .5f);
        deathSystem.Play();
        Invoke("LoadNextScene", sceneTransitionSeconds);
    }
}
