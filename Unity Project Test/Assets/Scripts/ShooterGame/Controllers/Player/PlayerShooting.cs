using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public int damagePerShot = 20;
    public float timeBetweenBullets = 0.15f;
    public float range = 100f;


    float timer;
    Ray shootRay = new Ray();
    RaycastHit shootHit;
    int shootableMask;
    ParticleSystem gunParticles;
    LineRenderer gunLine;
    AudioSource gunAudio;
    Light gunLight;
    float effectsDisplayTime = 0.2f;
    private bool _isFiring;


    void Start ()
    {
        shootableMask = LayerMask.GetMask ("Shootable");
        gunParticles = gameObject.GetComponent<ParticleSystem>();
        gunLine = gameObject.GetComponent<LineRenderer>();
        gunAudio = gameObject.GetComponent<AudioSource> ();
        gunLight =  gameObject.GetComponent<Light> ();
    }


    void Update ()
    {
        timer += Time.deltaTime;

		if(_isFiring && timer >= timeBetweenBullets && Time.timeScale != 0)
        {
            Shoot ();
        }

        if(timer >= timeBetweenBullets * effectsDisplayTime)
        {
            DisableEffects ();
        }
    }

    public void SetFiring(bool isFiring)
    {
        _isFiring = isFiring;
    }

    public bool GetFiring()
    {
        return _isFiring;
    }


    public void DisableEffects ()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;
    }


    void Shoot ()
    {
        timer = 0f;

        gunAudio.Play ();

        gunLight.enabled = true;

        gunParticles.Stop ();
        gunParticles.Play ();

        gunLine.enabled = true;
        gunLine.SetPosition (0, transform.position);

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
        {
            PlayerHealth enemyHealth = shootHit.collider.GetComponent <PlayerHealth> ();
            if(enemyHealth != null)
            {
                enemyHealth.TakeDamage (damagePerShot);
            }
            gunLine.SetPosition (1, shootHit.point);
        }
        else
        {
            gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
        }
    }
}
