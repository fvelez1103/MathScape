using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float moveSpeed = 5f;
    
    [Header("Efectos de Sonido")]
    public AudioSource fuenteSonido;
    public AudioClip sonidoCaminar;
    [Range(0f, 1f)] public float volumenCaminar = 1f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator anim;
    private SpriteRenderer sprite;

    public bool estaMuerto = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        
        estaMuerto = false; 
    }

    void Update()
    {
        if (estaMuerto) return; 

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement != Vector2.zero)
        {
            movement = movement.normalized;

            anim.SetFloat("MoveX", Mathf.Abs(movement.x)); 
            anim.SetFloat("MoveY", movement.y);
            anim.SetBool("estaCaminando", true);

            if (movement.x < 0) sprite.flipX = true;
            else if (movement.x > 0) sprite.flipX = false;

            if (fuenteSonido != null && sonidoCaminar != null && !fuenteSonido.isPlaying)
            {
                fuenteSonido.pitch = Random.Range(0.9f, 1.1f); 
                fuenteSonido.volume = volumenCaminar;
                fuenteSonido.clip = sonidoCaminar;
                fuenteSonido.Play();
            }
        }
        else
        {
            anim.SetBool("estaCaminando", false);

            if (fuenteSonido != null && fuenteSonido.isPlaying && fuenteSonido.clip == sonidoCaminar)
            {
                fuenteSonido.Stop();
            }
        }
    }

    void FixedUpdate()
    {
        if (estaMuerto) return;
        
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("NPC") && !estaMuerto) 
        {
            Muerte();
        }
    }
    
    public void Muerte()
    {
        if (!estaMuerto)
        {
            estaMuerto = true;
            movement = Vector2.zero; 

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; 
                rb.constraints = RigidbodyConstraints2D.FreezeAll; 
            }
            
            if (fuenteSonido != null)
            {
                fuenteSonido.Stop(); 
            }
            
            if (anim != null) 
            {
                anim.SetBool("estaCaminando", false);
                anim.SetFloat("MoveX", 0);
                anim.SetFloat("MoveY", 0);
                anim.SetTrigger("Morir");
            }

            this.enabled = false;
        }
    }
}