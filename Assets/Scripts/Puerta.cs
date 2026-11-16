using UnityEngine;
using System.Collections;

public class Puerta : MonoBehaviour
{
    public float angulo = 90f;
    public float duracion = 1f;
    public bool bloqueada = false;

    bool abierta, moviendo;

    void OnMouseDown() { if (!bloqueada) Alternar(); }

    public void Alternar()
    {
        if (moviendo) return;
        abierta = !abierta;
        StartCoroutine(Girar(abierta ? angulo : -angulo));
    }

    IEnumerator Girar(float total)
    {
        moviendo = true;
        float t = 0, hecho = 0;
        while (t < duracion)
        {
            t += Time.deltaTime;
            float objetivo = Mathf.Lerp(0, total, Mathf.SmoothStep(0, 1, t / duracion));
            transform.Rotate(Vector3.up, objetivo - hecho, Space.World);
            hecho = objetivo;
            yield return null;
        }
        moviendo = false;
    }
}