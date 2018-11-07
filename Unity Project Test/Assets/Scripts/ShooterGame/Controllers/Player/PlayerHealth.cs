using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ShooterGame.Controllers;
using UnityEngine.SceneManagement;


public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Image damageImage;
    public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

    private Action<int, int, int> _healthWatcher;
    private Animator _anim;
    private AudioSource _playerAudio;
    private PlayerMovement _playerMovement;
    private PlayerShooting _playerShooting;
    private PlayerController _playerController;
    private bool _isDead;
    private bool _damaged;


    void Awake ()
    {
        _anim = GetComponent <Animator> ();
        _playerAudio = GetComponent <AudioSource> ();
        _playerMovement = GetComponent <PlayerMovement> ();
        _playerShooting = GetComponentInChildren <PlayerShooting> ();
        _playerController = GetComponent <PlayerController> ();
        currentHealth = startingHealth;
    }


    void Update ()
    {
        if(_damaged)
        {
            //damageImage.color = flashColour;
        }
        else
        {
            //damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        _damaged = false;
    }

    public void SetHealthWatcher(Action<int, int, int> healthWatcher)
    {
        _healthWatcher = healthWatcher;
    }

    public void TakeDamage (int amount)
    {
        _damaged = true;
        currentHealth  = currentHealth - amount < 0 ? 0 : currentHealth - amount;
        //healthSlider.value = currentHealth;
        _playerAudio.Play ();
        if (_healthWatcher != null)
        {
            _healthWatcher(currentHealth, _playerController.ObjectId, _playerController.ClientId);
        }
        
        if(currentHealth <= 0 && !_isDead)
        {
            Death ();
        }
    }

    public void UpdateHealth(int health)
    {
        if (health < currentHealth)
        {
            TakeDamage(currentHealth - health);
        }
    }

    private void Death ()
    {
        _isDead = true;
        _playerShooting.DisableEffects ();
        _anim.SetTrigger ("Die");
        
        _playerMovement.enabled = false;
        _playerShooting.enabled = false;
        
        Destroy (gameObject, 5f);
    }

    public void PlayDeathSong()
    {
        _playerAudio.clip = deathClip;
        _playerAudio.Play ();
    }
}
