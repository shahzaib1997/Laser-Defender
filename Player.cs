using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    // configuration parametrs
    [Header("PlayerMovement")]
    [SerializeField] float moveSspeed = 10f;
    [SerializeField] float xPadding = 0.7f, yPadding = 1f;

    [Header("Health")]
    [SerializeField] int playerHealth = 1500;

    [Header("DestroySFX")]
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0, 1)] float deathSFXVolume = 0.75f;

    [Header("ShootingSFX")]
    [SerializeField] AudioClip shootSound;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.25f;

    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 20f;
    [SerializeField] float projectileFiringPeriod = 0.05f;

    Coroutine firingCoroutine;
    float xMin, xMax, yMin, yMax;

    // cached component refrences
    Level level;


    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
        level = FindObjectOfType<Level>();
        //StartCoroutine(PrintAndWait());
        /*Debug.Log(transform.rotation);
        transform.rotation = Quaternion.identity;
        Debug.Log(transform.rotation);*/
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

   private void OnTriggerEnter2D(Collider2D collision)
   {
        //if(collision.gameObject.tag == "EnemyLaser")
        //{
            DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
            GameObject gameobject = collision.gameObject;
            if (!damageDealer)
            {
              return;
            }
            ProcessHit(damageDealer, gameobject);
        //}
   }

    private void ProcessHit(DamageDealer damageDealer, GameObject gameobject)
    {
        playerHealth = playerHealth - damageDealer.GetDamage();
        if (gameobject.tag != "Enemy")
        {
            damageDealer.Hit();
        }
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        level.LoadGameOver();
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSFXVolume);
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + xPadding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - xPadding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + yPadding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - yPadding;
    }


    /*IEnumerator PrintAndWait()
    {
        Debug.Log("My first");
        yield return new WaitForSeconds(3);
        Debug.Log("Coroutine");
    }*/

    private void Fire()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(StartFiring());
        }
        if(Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
            //StopAllCoroutines();
        }
    }

    IEnumerator StartFiring()
    {
        while(true)
        {
            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSspeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSspeed;
        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector3(newXPos, newYPos, transform.position.z);


        /*Rigidbody2D myRigidbody2D = GetComponent<Rigidbody2D>();
        float deltaX = CrossPlatformInputManager.GetAxis("Horizontal");
        float deltaY = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 PlayerVelocity = new Vector2(deltaX * moveSspeed, deltaY * moveSspeed);
        myRigidbody2D.velocity = PlayerVelocity;*/
    }

    public int GetHealth()
    {
        return playerHealth;
    }
}
