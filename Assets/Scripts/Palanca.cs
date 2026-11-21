using UnityEngine;
using System.Collections;

public class Palanca : MonoBehaviour
{
    public Puerta puertaFinal;
    public Renderer basePalanca;
    public float anguloBajada = 60f;
    public float duracion = 0.6f;

    private Transform pivote;
    private Vector3 eje;
    private bool activada, moviendo;

    public bool Activada => activada;

    void Start()
    {
        if (puertaFinal == null)
        {
            foreach (Puerta p in FindObjectsByType<Puerta>())
                if (p.bloqueada) { puertaFinal = p; break; }
        }

        CalcularGiro(out Vector3 punto, out eje);

        pivote = new GameObject("PivotePalanca").transform;
        pivote.position = punto;
        pivote.SetParent(transform.parent, true);
        transform.SetParent(pivote, true);
    }

    void CalcularGiro(out Vector3 punto, out Vector3 ejeGiro)
    {
        Bounds palo = GetComponent<Renderer>().bounds;

        if (basePalanca == null && transform.parent != null)
        {
            Transform t = transform.parent.Find("PalancaBase");
            if (t != null) basePalanca = t.GetComponent<Renderer>();
        }

        Vector3 centroBase = basePalanca != null ? basePalanca.bounds.center : palo.center;
        punto = palo.ClosestPoint(centroBase);
        punto.y = palo.center.y;

        Vector3 haciaPunta = palo.center - punto;
        haciaPunta.y = 0;
        ejeGiro = haciaPunta.sqrMagnitude > 0.0001f
            ? Vector3.Cross(Vector3.up, haciaPunta).normalized
            : Vector3.right;
    }

    void OnMouseDown() => Accionar();

    public void Accionar()
    {
        if (activada || moviendo) return;
        StartCoroutine(Mover());
    }

    IEnumerator Mover()
    {
        moviendo = true;
        Quaternion inicio = pivote.rotation;
        Quaternion fin = Quaternion.AngleAxis(anguloBajada, eje) * inicio;
        float t = 0;
        while (t < duracion)
        {
            t += Time.deltaTime;
            pivote.rotation = Quaternion.Slerp(inicio, fin, Mathf.SmoothStep(0, 1, t / duracion));
            yield return null;
        }
        pivote.rotation = fin;
        moviendo = false;
        activada = true;

        if (puertaFinal != null)
        {
            puertaFinal.bloqueada = false;
            if (!puertaFinal.Abierta) puertaFinal.Alternar();
        }
    }
}
