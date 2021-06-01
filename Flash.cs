using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flash : MonoBehaviour
{    //private AudioSource SonidoExplosion;
    private Transform cam;
    private Transform cam2;
    private RawImage TexturaFlash;
    private float alpha;
    private RawImage ImagenSecundariaCamara;
    public GameObject Humo;
    [Range(0, 180)]
    public int ANGULO_MAX_FLASH;

    [Range(0, 180)]
    public int ANGULO_TOTAL_FLASH;

    [Range(0, 50)]
    public int DISTANCIA_MAX_FLASH;

    [Range(0.5f, 3.0f)]
    public float EsperaFlash;

    public float DuracionBaseFlash;
    private float VelocidadFlashReducida;
    void Start()
    {
        //SonidoExplosion = GetComponent<AudioSource>();

        cam = CharacterMangerController.Manager.GetCamTransform();
        cam2 = CharacterMangerController.Manager.GetFlashCamTransform();
        TexturaFlash = CharacterMangerController.Manager.GetTexturaFlash();
        ImagenSecundariaCamara = CharacterMangerController.Manager.GetFlashCamImage();
        ImagenSecundariaCamara.gameObject.SetActive(false);
    }
    void Update()
    {

    }
    public void Lanzar()
    {
        Invoke("FlashExplosion", EsperaFlash);
    }
    void FlashExplosion()
    {
        //SonidoExplosion.Play();
        Vector3 DireccionGranada = transform.position - cam.position;
        float angle = Vector3.Angle(DireccionGranada, cam.forward);
        float initialFlash = (angle < ANGULO_TOTAL_FLASH) ? 1.0f : 1.0f - angle / ANGULO_MAX_FLASH;
        float distance = Vector3.Distance(transform.position, cam.position);
        VelocidadFlashReducida = DuracionBaseFlash * (distance / DISTANCIA_MAX_FLASH);
        RaycastHit hit;
        bool grenadeHidden = Physics.Raycast(cam.position, DireccionGranada, out hit, distance - 1.0f);
        bool noAngle = (angle > ANGULO_MAX_FLASH);
        bool tooFar = (distance > DISTANCIA_MAX_FLASH);
        //if (!grenadeHidden && !noAngle && !tooFar)
        //{
        Debug.Log("exploto");
        Color c = TexturaFlash.color;
        c.a = initialFlash;
        TexturaFlash.color = c;
        cam2.transform.position = cam.transform.position;
        cam2.transform.rotation = cam.transform.rotation;
        ImagenSecundariaCamara.gameObject.SetActive(true);
        Color c2 = ImagenSecundariaCamara.color;
        c2.a = initialFlash;
        ImagenSecundariaCamara.color = c2;
        InvokeRepeating("ReducirFlash", initialFlash, 0.1f);
        //}
    }
    void ReducirFlash()
    {
        Color c = TexturaFlash.color;
        c.a = c.a - 0.05f;
        TexturaFlash.color = c;
        Color c2 = ImagenSecundariaCamara.color;
        c2.a = c2.a - 0.05f;
        ImagenSecundariaCamara.color = c2;
        if (c.a <= 0.0f && c2.a <= 0.0f)
        {
            ImagenSecundariaCamara.gameObject.SetActive(true);
            CancelInvoke();
            //Destroy(this.gameObject);
            Humo.SetActive(true);
            Invoke("Apagarhumo", 5);
        }
    }
    void Apagarhumo()
    {
        Destroy(this.gameObject);
    }
}
