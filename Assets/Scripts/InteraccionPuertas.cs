using UnityEngine;

public class InteraccionPuertas : MonoBehaviour
{
    public float distancia = 2f;
    public KeyCode tecla = KeyCode.E;
    public float velocidadFundido = 6f;

    private Puerta[] puertas;
    private Palanca[] palancas;
    private Puerta puertaCercana;
    private Palanca palancaCercana;
    private string textoAviso;
    private bool conTecla;
    private float alphaAviso;

    void Start()
    {
        puertas = FindObjectsByType<Puerta>();
        palancas = FindObjectsByType<Palanca>();
    }

    void Update()
    {
        float dPuerta = BuscarMasCercano(puertas, out puertaCercana);
        float dPalanca = BuscarMasCercano(palancas, out palancaCercana);

        if (palancaCercana != null && palancaCercana.Activada) palancaCercana = null;

        if (palancaCercana != null && puertaCercana != null)
        {
            if (puertaCercana.bloqueada || dPalanca < dPuerta) puertaCercana = null;
            else palancaCercana = null;
        }

        ActualizarAviso();

        if (!Input.GetKeyDown(tecla)) return;

        if (palancaCercana != null)
            palancaCercana.Accionar();
        else if (puertaCercana != null && !puertaCercana.bloqueada)
            puertaCercana.Alternar();
    }

    float BuscarMasCercano<T>(T[] lista, out T cercano) where T : Component
    {
        cercano = null;
        float mejor = distancia;

        foreach (T obj in lista)
        {
            float d = Vector3.Distance(transform.position, PuntoMasCercano(obj));
            if (d < mejor)
            {
                mejor = d;
                cercano = obj;
            }
        }
        return mejor;
    }

    Vector3 PuntoMasCercano(Component obj)
    {
        Renderer r = obj.GetComponentInChildren<Renderer>();
        if (r != null) return r.bounds.ClosestPoint(transform.position);
        return obj.transform.position;
    }

    void ActualizarAviso()
    {
        string texto = null;
        bool mostrarTecla = true;

        if (palancaCercana != null)
            texto = "Accionar la palanca";
        else if (puertaCercana != null && puertaCercana.bloqueada)
        {
            texto = "Cerrada a cal y canto... algún mecanismo ha de abrirla";
            mostrarTecla = false;
        }
        else if (puertaCercana != null)
            texto = puertaCercana.Abierta ? "Cerrar la puerta" : "Abrir la puerta";

        if (texto != null)
        {
            textoAviso = texto;
            conTecla = mostrarTecla;
        }

        float destino = texto != null ? 1f : 0f;
        alphaAviso = Mathf.MoveTowards(alphaAviso, destino, velocidadFundido * Time.deltaTime);
    }

    void OnGUI()
    {
        if (alphaAviso <= 0f || textoAviso == null) return;
        EstiloMedieval.DibujarAviso(conTecla ? tecla.ToString() : null, textoAviso, alphaAviso);
    }
}
