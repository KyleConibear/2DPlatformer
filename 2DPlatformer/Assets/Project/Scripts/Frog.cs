using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Enemy // This is inheriting from Enemy, Frog is now the derived class
{
    [SerializeField] private AudioClip[] audioClips = new AudioClip[0];
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private Rigidbody2D rigidbody2D = null;

    private enum SFX
    {
        CROAK = 0,
    }

    public void PlayCroak()
    {
        this.audioSource.clip = this.audioClips[(int)SFX.CROAK];
        this.audioSource.Play();
    }

    protected override void Move()
    {
        //base.Move(); // Enemy move
        // this.Move(); // Frog move
    }

    protected override void ArrivedAtDestination() // Declare in base class and is required to complie
    {
        Debug.Log("Arrived at destination");
    }
}