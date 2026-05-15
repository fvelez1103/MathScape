using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Mathematics;

public class MovimientoJugador2d : MonoBehaviour
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

    Quaternion voltearIzq = Quaternion.Euler(0, -180, 0);
    Quaternion voltearDerecha = Quaternion.Euler(0, 0, 0);
    Rigidbody2D theRB;
    Animator Animacion;
    void Start()
    {
        theRB = GetComponent<Rigidbody2D>();
        Animacion = GetComponent<Animator>();
    }


    void Update()
    {
        inputMovimiento.x = Input.GetAxis("Horizontal");
        inputMovimiento.y = Input.GetAxis("Vertical");

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

        if (!volteado && inputMovimiento.y > 0) volteado = true;
        else if (volteado && inputMovimiento.y < 0) volteado = false;
        Animacion.SetBool("volteado", volteado);
        if (Input.GetKeyDown(KeyCode.Space) && enSuelo) inputSalto = true;
        Animacion.SetBool("enSuelo", enSuelo);

    }

    private void FixedUpdate()
    {
        theRB.linearVelocity = new UnityEngine.Vector3(inputMovimiento.x * movimiento, theRB.linearVelocity.y, inputMovimiento.y * movimiento);

        RaycastHit hit;
        if (Physics.Raycast(validarSuelo.position, UnityEngine.Vector3.down, out hit, rayLength, suelo)) enSuelo = true;
        else enSuelo = false;

        Debug.DrawRay(validarSuelo.position, UnityEngine.Vector2.down, Color.red);

        if (inputSalto) Jump();
    }

    void Jump()
    {
        theRB.linearVelocity = new UnityEngine.Vector3(0f, fuerzaSalto, 0f);
        inputSalto = false;
    }
}
