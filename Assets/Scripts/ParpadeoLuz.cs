using UnityEngine;

[RequireComponent(typeof(Light))]
public class ParpadeoLuz : MonoBehaviour
{
    public float variacion = 0.35f;
    public float velocidad = 6f;

    private Light luz;
    private float intensidadBase;
    private float semilla;

    void Start()
    {
        luz = GetComponent<Light>();
        intensidadBase = luz.intensity;
        semilla = Random.Range(0f, 100f);
    }

    void Update()
    {
        float ruido = Mathf.PerlinNoise(semilla, Time.time * velocidad) * 2f - 1f;
        luz.intensity = intensidadBase * (1f + ruido * variacion);
    }
}
