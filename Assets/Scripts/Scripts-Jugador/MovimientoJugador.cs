using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Mathematics;

public class MovimientoJugador : MonoBehaviour
{
    public float movimiento;
    UnityEngine.Vector2 inputMovimiento;

    public float fuerzaSalto;
    [SerializeField] bool inputSalto;

    public float pesoJugador = 1f;

    public Transform validarSuelo;
    public LayerMask suelo;
    public float rayLength;
    [SerializeField] bool enSuelo;

    [SerializeField] bool volteado;
    public bool flipped;
    public float flipSpeed;
    public float velocidadVoltear;

    // --- NUEVAS VARIABLES DE CONTROL ---
    public bool estaEnDialogo = false; 
    // ------------------------------------

    [Header("Sonido de Pasos")]
    public AudioSource fuentePasos;
    public AudioClip sonidoPasosPiedra;

    Quaternion voltearIzq = Quaternion.Euler(0, -180, 0);
    Quaternion voltearDerecha = Quaternion.Euler(0, 0, 0);
    Rigidbody theRB;
    Animator Animacion;

    void Start()
    {
        theRB = GetComponent<Rigidbody>();
        Animacion = GetComponent<Animator>();
    }

   void Update()
{
    // --- CONTROL DE DIÁLOGO (REFORZADO) ---
    if (estaEnDialogo)
    {
        inputMovimiento = UnityEngine.Vector2.zero;
        if (fuentePasos != null && fuentePasos.isPlaying) fuentePasos.Stop();
        Animacion.SetFloat("movimiento", 0f);
        return; 
    }
    // --------------------------------------

    movimiento = 2.7f;
    inputMovimiento.x = Input.GetAxisRaw("Horizontal");
    inputMovimiento.y = Input.GetAxisRaw("Vertical");

    // ... (resto del código igual)

        inputMovimiento = Vector2.ClampMagnitude(inputMovimiento, 1f);

        Animacion.SetFloat("movimiento", theRB.linearVelocity.magnitude);
        
        if (!flipped && inputMovimiento.x < 0)
        {
            flipped = true;
        }
        else if (flipped && inputMovimiento.x > 0)
        {
            flipped = false;
        }
        
        if (flipped) transform.rotation = Quaternion.Slerp(transform.rotation, voltearIzq, velocidadVoltear * Time.deltaTime);
        else if (!flipped) transform.rotation = Quaternion.Slerp(transform.rotation, voltearDerecha, velocidadVoltear * Time.deltaTime);
        
        if (inputMovimiento.y > 0) 
        {
            volteado = true;
        }
        else 
        {
            volteado = false; 
        }
        Animacion.SetBool("volteado", volteado);
        
        if (Input.GetKeyDown(KeyCode.Space) && enSuelo) inputSalto = true;
        Animacion.SetBool("enSuelo", enSuelo);

        // --- LÓGICA DE SONIDO DE PASOS ---
        bool seEstaMoviendo = inputMovimiento.magnitude > 0.1f;

        if (seEstaMoviendo && enSuelo)
        {
            if (fuentePasos != null && !fuentePasos.isPlaying)
            {
                fuentePasos.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                fuentePasos.clip = sonidoPasosPiedra;
                fuentePasos.Play();
            }
        }
        else
        {
            if (fuentePasos != null && fuentePasos.isPlaying)
            {
                fuentePasos.Stop();
            }
        }
    }

    private void FixedUpdate()
    {
        // Si estamos en diálogo, el Rigidbody no debería moverse horizontalmente
        if (estaEnDialogo)
        {
            theRB.linearVelocity = new UnityEngine.Vector3(0, theRB.linearVelocity.y, 0);
            return;
        }

        theRB.linearVelocity = new UnityEngine.Vector3(inputMovimiento.x * movimiento, theRB.linearVelocity.y, inputMovimiento.y * movimiento);

        RaycastHit hit;
        if (Physics.Raycast(validarSuelo.position, UnityEngine.Vector3.down, out hit, rayLength, suelo)) enSuelo = true;
        else enSuelo = false;

        Debug.DrawRay(validarSuelo.position, UnityEngine.Vector2.down, Color.red);

        if (inputSalto) Jump();
    }

    void Jump()
    {
        theRB.linearVelocity = new UnityEngine.Vector3(theRB.linearVelocity.x, fuerzaSalto, theRB.linearVelocity.z);
        inputSalto = false;
    }
}