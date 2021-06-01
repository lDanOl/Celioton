using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterMangerController : MonoBehaviour
{
    public Camera CamaraTerceraPersona;
    public float Velocidad;
    private float Velocidadbase;
    public float VelocidadCorrer;
    public float VelocidadRotacion ;
    public float VelocidadSalto;
    public float VelocidadBlendAnimacion = 6f;
    public Slider Stamina;
    public Text NDardos;
    public Text NTarjetas;
    public GameObject Corazon;
    public GameObject CorazonRoto;
    public GameObject Sinenergia;
    public CharacterController Controlador;
    public CapsuleCollider Controlador2;
    public Animator Animador;
    float AjusteRotacion=0f;
    float AjusteVelocidadRotacion=0f;
    float AjusteVelocidadAnimacion=0f;
    float VelocidadY = 0;
    float Gravedad = -9.81f;
    public bool Saltar = false;
    public bool Correr = false;
    public bool Agachado = false;
    public bool YaAtaque;
    public bool HumoUsable;
    public Transform cam;
    public Transform cam2;
    public RawImage TexturaFlash;
    public RawImage ImagenSecundariaCamara;
    public static CharacterMangerController Manager;
    public int VidaMaxima;
    private int VidaActual;
    public Material ColorCuerpo;
    private Color ColorBase;
    public Transform Dardos;
    public Transform Humos;
    public GameObject DardoPrefab;
    public GameObject SomniferosHumo;
    private GameObject Somniferos;
    public float FuerzaLanzamiento;
    public float TiempoEntreAtaques;
    private float TiempoEntreAtaquesHumo=2;
    public int NumeroDardos;
    public int NumeroTarjetas;
    



    private void Awake()
    {
        Manager = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Controlador = GetComponent<CharacterController>();
        Animador = GetComponent<Animator>();

        //ColorBase = ColorCuerpo.GetColor("_BaseColor");
        ColorBase = Color.white;
        VidaActual = VidaMaxima;
        
        Velocidadbase = Velocidad;
        
    }
    public void AtacarEnemigo()
    {
        if (NumeroDardos >= 1)
        {
            if (!HumoUsable)
            {
                DardoPrefab = Instantiate(DardoPrefab, Dardos.position + Dardos.forward * 1, Dardos.rotation);
                DardoPrefab.gameObject.GetComponent<MeshRenderer>().enabled = true;
                Rigidbody rig = DardoPrefab.gameObject.GetComponent<Rigidbody>();
                rig = DardoPrefab.gameObject.GetComponent<Rigidbody>();
                rig.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rig.isKinematic = false;
                DardoPrefab.GetComponent<Rigidbody>().AddForce(Dardos.forward * FuerzaLanzamiento, ForceMode.Impulse);
                Invoke(nameof(resetearAtaque), TiempoEntreAtaques);
                YaAtaque = true;
                NumeroDardos=NumeroDardos-1;
            }
            else
            {
                Somniferos = Instantiate(SomniferosHumo, Humos.position + Humos.forward * 1, Humos.rotation);
                Invoke(nameof(resetearAtaqueHumo), TiempoEntreAtaques);
                YaAtaque = true;
                NumeroDardos = NumeroDardos - 1;
            }
        }
        
    }
    public void resetearAtaque()
    {
        YaAtaque = false;
        
        
    } public void resetearAtaqueHumo()
    {
        YaAtaque = false;
        Destroy(Somniferos.gameObject);
        
    }
    
    // Update is called once per frame
    void Update()
    {
        NDardos.text = NumeroDardos.ToString();
        NTarjetas.text = NumeroTarjetas.ToString();
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        if (VidaMaxima > 1) { CorazonRoto.SetActive(false); Corazon.SetActive(true); }//ColorCuerpo.SetColor("_BaseColor", ColorBase);
        else if (VidaMaxima == 1) { CorazonRoto.SetActive(true); Corazon.SetActive(false); }//ColorCuerpo.SetColor("_BaseColor", Color.red);
        else if (VidaMaxima <= 0) { /*ColorCuerpo.SetColor("_BaseColor", Color.black); */SceneManager.LoadScene("Menu", LoadSceneMode.Single); }
        Animador.SetBool("Dardo", false);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!YaAtaque) { AtacarEnemigo(); Animador.SetBool("Dardo", true); }

        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Agachado = true;
            Animador.SetBool("Agacharse", true);
            Velocidad = 5f;
        }
        else { Agachado = false; Animador.SetBool("Agacharse", false); Velocidad = Velocidadbase; }
        if (Input.GetButtonDown("Saltar") && !Saltar)
        {
            Saltar = true;
            Animador.SetTrigger("Saltar");
            VelocidadY += VelocidadSalto;
        }
        if (!Controlador.isGrounded)
        {
            VelocidadY += Gravedad * Time.deltaTime;
        }
        else if(VelocidadY<0) { VelocidadY = 0; }
        Animador.SetFloat("VelocidadY", VelocidadY / VelocidadSalto);
        if (Saltar && VelocidadY < 0)
        {
            RaycastHit toque;
            if(Physics.Raycast(transform.position,Vector3.down,out toque, 0.5f, LayerMask.GetMask("Default")))
            {
                Saltar = false;
                Animador.SetTrigger("Aterrizar");
            }
        }
        Correr = Input.GetKey(KeyCode.LeftShift);
        Vector3 movimiento = new Vector3(x, 0, z).normalized;
        Vector3 rotacion = Quaternion.Euler(0, CamaraTerceraPersona.transform.rotation.eulerAngles.y, 0) * movimiento;
        Vector3 movimientoVertical = Vector3.up * VelocidadY;


        //Controlador.Move((movimientoVertical + (rotacion * (Correr ? VelocidadCorrer : Velocidad))) * Time.deltaTime);
        if (x > 0f || z > 0f) { Animador.SetBool("Movimiento", true); } else { Animador.SetBool("Movimiento", false); }
        Controlador.Move((movimientoVertical + (rotacion * Velocidad)) * Time.deltaTime); 
        if (rotacion.magnitude > 0)
        {
            AjusteRotacion = Mathf.Atan2(rotacion.x, rotacion.z) * Mathf.Rad2Deg;
            AjusteVelocidadAnimacion = Correr ? 1 : .5f;
        }
        else
        {
            AjusteVelocidadAnimacion = 0;
        }
        if (Stamina.value != 0)
        {
            Sinenergia.SetActive(false);
            if (Correr&&!Agachado) {
                
                if (Animador.GetFloat("Velocidad")>=0.5) {
                    StaminaBar.instance.StaminaUsada(1);
                    Controlador.Move((movimientoVertical + (rotacion * VelocidadCorrer)) * Time.deltaTime);
                }
                 }
        }
        if (Stamina.value == 0)
        {
            Animador.SetFloat("Velocidad", 0.5f);
            Sinenergia.SetActive(true);
        }
            Animador.SetFloat("Velocidad", Mathf.Lerp(Animador.GetFloat("Velocidad"), AjusteVelocidadAnimacion, VelocidadBlendAnimacion * Time.deltaTime));
        Quaternion rotacionActual = transform.rotation;
        Quaternion rotacionElegida = Quaternion.Euler(0, AjusteRotacion, 0);
        transform.rotation = Quaternion.Lerp(rotacionActual, rotacionElegida, VelocidadRotacion * Time.deltaTime);
        if (Agachado == true)
        {
            //Animador.SetBool("Agachado", true);
            Controlador.height = 6f;
            Controlador.center = new Vector3(0f, 2.96f, 0f);
            Controlador2.height = 6f;
            Controlador2.center = new Vector3(0f, 2.96f, 0f);
            

        }
        else
        {
            //Animador.SetBool("Agachado", false);
            Controlador.height = 9.16f;
            Controlador.center = new Vector3(0f, 4.53f, 0f);
            Controlador2.height = 9.16f;
            Controlador2.center = new Vector3(0f, 4.53f, 0f);
            
        }
    }
    
    public Transform GetCamTransform() { return cam; }

    public Transform GetFlashCamTransform() { return cam2; }

    public RawImage GetTexturaFlash() { return TexturaFlash; }

    public RawImage GetFlashCamImage() { return ImagenSecundariaCamara; }
}
