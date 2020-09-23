using System;
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
        processInput();
    }

    void processInput()
    {
        if (state == State.Alive)
        {
            CheckThrustInput();
            CheckRotateInput();
        }
    }

    private void CheckThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            applyThrust();
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void applyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * (mainThrust * Time.deltaTime));
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
    }

    private void CheckRotateInput()
    {
        rigidBody.freezeRotation = true;

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

        rigidBody.freezeRotation = false;
    }

    private void loadNextScene()
    {
        switch(state)
        {
            case State.Transcending:
                SceneManager.LoadScene(1);
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
        audioSource.PlayOneShot(success);
        Invoke("loadNextScene", sceneTransitionSeconds);
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death, .5f);
        Invoke("loadNextScene", sceneTransitionSeconds);
    }
}
