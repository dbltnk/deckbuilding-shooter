using System;
using System.Collections;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    // ========= MOVEMENT =================
    public float speed = 4;
    
    // ======== HEALTH ==========
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;
    public Transform respawnPosition;
    public ParticleSystem hitParticle;
    
    // ======== PROJECTILE ==========
    public GameObject projectilePrefab;

    // ======== AUDIO ==========
    public AudioClip hitSound;
    public AudioClip shootingSound;
    
    // ======== HEALTH ==========
    public int health
    {
        get { return currentHealth; }
    }
    
    // =========== MOVEMENT ==============
    Rigidbody2D rigidbody2d;
    
    // ======== HEALTH ==========
    int currentHealth;
    float invincibleTimer;
    bool isInvincible;
   
    // ==== ANIMATION =====
    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);
    
    // ================= SOUNDS =======================
    AudioSource audioSource;

    void Start()
    {
        // =========== MOVEMENT ==============
        rigidbody2d = GetComponent<Rigidbody2D>();
                
        // ======== HEALTH ==========
        invincibleTimer = -1.0f;
        currentHealth = maxHealth;
        
        // ==== ANIMATION =====
        animator = GetComponent<Animator>();
        
        // ==== AUDIO =====
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // ================= HEALTH ====================
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        // ============== MOVEMENT ======================
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
                
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        Vector2 position = rigidbody2d.position;
        
        position = position + move * speed * Time.deltaTime;
        
        rigidbody2d.MovePosition(position);


        // ============== ANIMATION =======================

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        // ======== DIALOGUE ==========
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, 1 << LayerMask.NameToLayer("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }  
            }
        }
 
    }
    
    // ===================== HEALTH ==================
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        { 
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
            
            animator.SetTrigger("Hit");
            audioSource.PlayOneShot(hitSound);

            Instantiate(hitParticle, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        if(currentHealth == 0)
            Respawn();
        
        UIHealthBar.Instance.SetValue(currentHealth / (float)maxHealth);
    }
    
    void Respawn()
    {
        ChangeHealth(maxHealth);
        transform.position = respawnPosition.position;
    }
    
    // =============== PROJECTICLE ========================
    public void LaunchProjectile(Vector2 worldPos, Gameplay.Card card)
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        projectileObject.transform.localScale = new Vector3(card.Ability.ProjectileSize, card.Ability.ProjectileSize, card.Ability.ProjectileSize);

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        Vector2 dir = worldPos - rigidbody2d.position;

        projectile.Launch(dir.normalized, card.Ability.ProjectileSpeed * 100f);
        
        animator.SetTrigger("Launch");
        audioSource.PlayOneShot(shootingSound);

        SpriteRenderer rend = projectileObject.GetComponent<SpriteRenderer>();
        rend.color = card.Ability.ProjectileColor;

        TrailRenderer trail = projectileObject.GetComponentInChildren<TrailRenderer>();
        trail.startColor = card.Ability.ProjectileColor;
        trail.endColor = Color.white;
        trail.startWidth = card.Ability.ProjectileSize;

        StartCoroutine(CoDestroy(projectileObject, card.Ability.ProjectileLifetime));
    }
    
    IEnumerator CoDestroy (GameObject go, float lifetime) {
        yield return new WaitForSeconds(lifetime);
        Destroy(go);
    }

    // =============== SOUND ==========================

    //Allow to play a sound on the player sound source. used by Collectible
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
