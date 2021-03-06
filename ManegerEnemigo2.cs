using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ManegerEnemigo2 : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator animador;
    public LayerMask QueEsSuelo, QueEsJugador;
    public float Vida;
    public GameObject Indicador;
    public GameObject Indicador2;
    public Transform ojosPlayer;
    public float VelocidadGiro;
    public float TiempoAlerta;
    public Transform EnemigoTransform;
    private float TiempoDeBusqueda;
    public static float valorAlturaAgachado = 3f;
    //Patrullaje
    public Vector3 TargetCaminado;
    public bool TargetCaminadoSet;
    public float TargetCaminadoRango;
    public Transform Enemigo;
    public float FuerzaLanzamiento;
    public Transform[] PuntosCaminado;
    private int SiguientePunto;
    public CharacterMangerController characterMangerController;
    public ManegerEnemigo2 ManagerE;
    private BoxCollider ZonaRR;
    public GameObject Tarjeta;
    public bool TieneTarjeta;
    public float RangoEscuchaActual;
    //Ataque
    public float TiempoEntreAtaques;
    public float TiempoEntreAtaquesFlash;
    public bool YaAtaque;
    private Vector3 posPlayer;

    //Estados
    public float RangoVision, RangoAtaque, AnguloVision, RangoEscucha, RangoAtacableConHumos;
    public bool JugadorAvistado, JugadorAtacable, JugadorEscuchado, EnemigoDetenido, JugadorCollision, HumoUsable, EnemigoAlertado, Encendido;

    private void Awake()
    {
        player = GameObject.Find("texturizaditoIdle").transform;
        agent = GetComponent<NavMeshAgent>();
        ManagerE = GetComponent<ManegerEnemigo2>();
        Encendido = true;
        RangoEscuchaActual = ManagerE.RangoEscucha;
    }
    public void OnEnable()
    {
        //SiguientePunto++; //resetear puntos de caminado al primero
        ActualizarPuntoDestino();
    }
    public void ActualizarPuntoDestino()
    {
        Indicador2.SetActive(false);
        ActualizarDestino(PuntosCaminado[SiguientePunto].position);

    }
    public void ActualizarDestino(Vector3 puntoDestino)
    {

        agent.isStopped = false;
        agent.destination = puntoDestino;

    }
    public void Detenerse()
    {
        agent.isStopped = true;
    }
    public void ApagarEnemigo()
    {
        Detenerse();
        RangoVision = 0;
        RangoAtaque = 0;
        AnguloVision = 0;
        RangoEscucha = 0;
        valorAlturaAgachado = 10f;
        RangoEscuchaActual = 0;
        RangoAtacableConHumos = 0;
        Debug.Log("ApagarEnemigo");
        ManagerE.enabled = !ManagerE.enabled;
        Indicador.SetActive(false);
        Indicador2.SetActive(false);
        EnemigoAlertado = false;
        agent.isStopped = true;
        Encendido = false;
        animador.SetBool("Dormir", true);
        animador.SetBool("Girar", false);
        animador.SetBool("Caminar", false);
        animador.SetBool("Golpe", false);
        animador.SetBool("Flash", false);
        animador.SetBool("Escribir", false);
        if (TieneTarjeta)
        {
            Tarjeta = Instantiate(Tarjeta, Enemigo.position + Enemigo.forward * 1, Quaternion.identity); TieneTarjeta = false; Rigidbody rig = Tarjeta.gameObject.GetComponent<Rigidbody>(); rig.AddForce(Enemigo.forward * 10, ForceMode.Impulse);
        }
    }
    public bool llegar()
    {
        return agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending;
    }
    public void Alerta()
    {
        Indicador.SetActive(true);
        Indicador2.SetActive(false);
        Debug.Log("alerta");
        if (!EnemigoDetenido)
        { Detenerse(); }
        EnemigoTransform.transform.Rotate(0f, VelocidadGiro * Time.deltaTime, 0f);
        EnemigoAlertado = true;
        StartCoroutine(enemigoAlertado(5));
    }
    public void ActivadorAlerta(float tiempo)
    {
        InvokeRepeating("Alerta", 0, tiempo); TiempoDeBusqueda = Time.deltaTime;
    }
    IEnumerator enemigoAlertado(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        Indicador.SetActive(false);
        TiempoAlerta = 0;
        CancelInvoke();
        EnemigoAlertado = false;
    }
    public bool AliadoRREnontrado()
    {
        return true;
    }
    public void seguir2() {
        agent.SetDestination(posPlayer);
        
    }
    private void FixedUpdate()
    {

        //Verificar rango de vision y ataque
        ojosPlayer.position = new Vector3(player.position.x, ojosPlayer.position.y, player.position.z);
        JugadorAtacable = Physics.CheckSphere(transform.position, RangoAtaque, QueEsJugador);
        JugadorEscuchado = Physics.CheckSphere(transform.position, RangoEscucha, QueEsJugador);
        HumoUsable = Physics.CheckSphere(transform.position, RangoAtacableConHumos, QueEsJugador);
        //if (JugadorALaVista(transform, player, RangoVision, AnguloVision)){JugadorAvistado = true;}else { JugadorAvistado = false; }
        TiempoAlerta = Time.deltaTime + 5f;
        JugadorAvistado = JugadorALaVista(transform, player, AnguloVision, RangoVision,agent);
        if (!JugadorAvistado && !EnemigoAlertado && Encendido) { ActualizarPuntoDestino(); EnemigoAlertado = false; StopCoroutine(enemigoAlertado(0)); CancelInvoke("Alerta"); animador.SetBool("Caminar", true); animador.SetBool("Girar", false); animador.SetBool("Escribir", false); animador.SetBool("Dormir", false); }
        if (!JugadorAvistado && JugadorEscuchado && !EnemigoAlertado) { ActivadorAlerta(0.01f); animador.SetBool("Caminar", false); animador.SetBool("Girar", true); animador.SetBool("Escribir", false); animador.SetBool("Dormir", false); }
        //if (!JugadorAvistado && !JugadorEscuchado && EnemigoAlertado) { ActivadorAlerta(); animador.SetBool("Caminar", false); animador.SetBool("Girar", true); animador.SetBool("Escribir", false); animador.SetBool("Dormir", false); }
        if (JugadorAvistado) {  seguir2(); posPlayer = player.position; Indicador2.SetActive(true); Indicador.SetActive(false); agent.isStopped = false; StopCoroutine(enemigoAlertado(0)); CancelInvoke("Alerta"); EnemigoAlertado = true;  animador.SetBool("Caminar", true); animador.SetBool("Girar", false); animador.SetBool("Escribir", false); animador.SetBool("Dormir", false); animador.SetBool("Golpe", false); animador.SetBool("Flash", false); }
        if (JugadorAtacable  && !YaAtaque) { atacarJugador(); Indicador2.SetActive(true); agent.isStopped = false; EnemigoAlertado = false; StopCoroutine(enemigoAlertado(0)); CancelInvoke("Alerta"); animador.SetBool("Caminar", false); animador.SetBool("Girar", false); animador.SetBool("Escribir", false); animador.SetBool("Dormir", false); animador.SetBool("Golpe", true); animador.SetBool("Flash", false); }
        if (llegar())
        {
            SiguientePunto = (SiguientePunto + 1) % PuntosCaminado.Length;
            if (EnemigoAlertado==true) { ActivadorAlerta(0.4f); animador.SetBool("Caminar", false); animador.SetBool("Girar", true); animador.SetBool("Escribir", false); animador.SetBool("Dormir", false); }
        }
        if (TiempoDeBusqueda >= TiempoAlerta)
        {
            EnemigoAlertado = false;
        }


        if (HumoUsable) { characterMangerController.HumoUsable = true; } else { characterMangerController.HumoUsable = false; }
        if (Input.GetKeyDown(KeyCode.Q) && HumoUsable && !JugadorEscuchado && !JugadorAvistado) { Invoke(nameof(ApagarEnemigo), 2); }
        if (Input.GetKey(KeyCode.LeftControl)) { valorAlturaAgachado = 1f; } else { valorAlturaAgachado = 4f; }
        if (characterMangerController.Agachado == true) { ManagerE.RangoEscucha = 2; } else { ManagerE.RangoEscucha = RangoEscuchaActual; }
    }


    public static bool JugadorALaVista(Transform objetoAVerificar, Transform objetivo, float anguloVision, float rangoVision, NavMeshAgent agent)
    {
        Collider[] sobrepuesto = new Collider[500];
        int cuenta = Physics.OverlapSphereNonAlloc(objetoAVerificar.position, rangoVision, sobrepuesto);
        for (int i = 0; i < cuenta + 1; i++)
        {
            if (sobrepuesto[i] != null)
            {
                if (sobrepuesto[i].transform == objetivo)
                {
                    Vector3 direccion = ((objetivo.position) - objetoAVerificar.position).normalized;
                    direccion.y *= 0;
                    float Angulo = Vector3.Angle(objetoAVerificar.forward, direccion);
                    if (Angulo <= anguloVision)
                    {
                        Ray ray = new Ray((objetoAVerificar.position + new Vector3(0, valorAlturaAgachado, 0)), (objetivo.position + new Vector3(0, valorAlturaAgachado, 0)) - objetoAVerificar.position);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, rangoVision))
                        {
                            if (hit.transform == objetivo)
                            {
                                agent.isStopped = true;
                                return true;

                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    private void seguirJugador()
    {
        //Debug.Log("Persigo");
        
        agent.SetDestination(posPlayer);
    }
    private void atacarJugador()
    {
        agent.SetDestination(player.position);

        if (!YaAtaque)
        {
            Debug.Log("Ataco");
            characterMangerController.VidaMaxima = characterMangerController.VidaMaxima - 1;
            transform.LookAt(player);
            YaAtaque = true;
            StartCoroutine(ResetearAtaque(TiempoEntreAtaques));
        }


    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Somnifero")
        {
            ApagarEnemigo();
            Debug.Log("Somnifero");
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (characterMangerController.VidaMaxima >= 0)
            {
                JugadorCollision = true;
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            JugadorCollision = false;
        }
    }
    
    IEnumerator ResetearAtaque(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        YaAtaque = false;
        Debug.Log("reseteo");

    }
    public void ResibirDa?o(int Da?o)
    {
        Vida -= Da?o;
        if (Vida <= 0) Invoke(nameof(DestruirEnemigo), .5f);

    }
    private void DestruirEnemigo()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, RangoAtaque);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, RangoVision);
        Vector3 Fov1 = Quaternion.AngleAxis(AnguloVision, transform.up) * transform.forward * RangoVision;
        Vector3 Fov2 = Quaternion.AngleAxis(-AnguloVision, transform.up) * transform.forward * RangoVision;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, Fov1);
        Gizmos.DrawRay(transform.position, Fov2);
        if (!JugadorAvistado)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position + new Vector3(0f, valorAlturaAgachado, 0f), ((player.position + new Vector3(0f, valorAlturaAgachado, 0f)) - transform.position).normalized * RangoVision);
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * RangoVision);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, RangoEscucha);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, RangoAtacableConHumos);

    }
}
