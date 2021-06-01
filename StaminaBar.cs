using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StaminaBar : MonoBehaviour
{
    public Slider Stamina;
    public int StaminaMax = 100;
    private int StaminaActual;
    public static StaminaBar instance;
    private WaitForSeconds RegeneracionTick = new WaitForSeconds(0.1f);
    private Coroutine Regeneracion;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        StaminaActual = StaminaMax;
        Stamina.maxValue = StaminaMax;
        Stamina.value = StaminaMax;
    }
    public void StaminaUsada(int cantidad)
    {
        if (StaminaActual - cantidad >= 0)
        {
            StaminaActual -= cantidad;
            Stamina.value = StaminaActual;
            if (Regeneracion != null)
                StopCoroutine(Regeneracion);
                Regeneracion = StartCoroutine(RegeneracionStamina());
            
        }
        else
        {
            Debug.Log("Sin Stamina");
        }
    }
    private IEnumerator RegeneracionStamina()
    {
        yield return new WaitForSeconds(1);
        while (StaminaActual < StaminaMax)
        {
            StaminaActual += StaminaMax / 50;
            Stamina.value = StaminaActual;
            yield return RegeneracionTick;

        }
        Regeneracion = null;
    }
    
}
