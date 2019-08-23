using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float enemyHealth = 100;
    [SerializeField] int scoreValue = 150;

    [Header("Shooting")]
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimBetweenShots = 3f;

    [Header("ShootingSFX")]
    [SerializeField] AudioClip shootSound;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.25f;

    [Header("Projectile")]
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed = 10f;

    [Header("DestroyVFX")]
    [SerializeField] GameObject deathVFX;
    [SerializeField] float durationOfExplosion = 1f;

    [Header("DestroySFX")]
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0, 1)]float deathSFXVolume = 0.75f;


    // Start is called before the first frame update
    void Start()
    {
        shotCounter = Random.Range(maxTimBetweenShots, maxTimBetweenShots);
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        float random = Random.Range(1, 5);
        shotCounter = shotCounter - (Time.deltaTime * random);
        if(shotCounter <= 0f)
        {
            Fire();
            shotCounter = Random.Range(maxTimBetweenShots, maxTimBetweenShots);
        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileSpeed);
        AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.tag == "PlayerLaser")
        //{
            DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
            if(!damageDealer)
            {
                return;
            }
            ProcessHit(damageDealer);
        //}
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        enemyHealth = enemyHealth - damageDealer.GetDamage();
        damageDealer.Hit();
        if(enemyHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        FindObjectOfType<GameSession>().AddToScore(scoreValue);
        Destroy(gameObject);
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        Destroy(explosion, durationOfExplosion);
        AudioSource.PlayClipAtPoint(deathSFX,Camera.main.transform.position,deathSFXVolume);
    }

}
