using UnityEngine;

public class ControlAnimaciones : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 3f;
    public float velocidadGiro = 10f;

    [Header("Salto y gravedad")]
    public float alturaSalto = 1.2f;
    public float gravedad = -20f;

    [Header("Suelo")]
    public LayerMask capaSuelo;
    public float radioDeteccion = 0.25f;

    private Animator anim;
    private CharacterController cc;
    private Transform camara;
    private float velocidadY;
    private bool enSuelo;

    public bool EnSuelo => enSuelo;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.applyRootMotion = false;

        cc = GetComponent<CharacterController>();
        if (cc == null)
        {
            cc = gameObject.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.3f;
            cc.center = new Vector3(0, 0.9f, 0);
        }

        if (capaSuelo.value == 0)
            capaSuelo = LayerMask.GetMask("Suelo");

        if (Camera.main != null)
            camara = Camera.main.transform;
    }

    void Update()
    {
        enSuelo = Physics.CheckSphere(PosicionPies(), radioDeteccion, capaSuelo, QueryTriggerInteraction.Ignore);

        float moverX = Input.GetAxisRaw("Horizontal");
        float moverZ = Input.GetAxisRaw("Vertical");

        Vector3 adelante = Vector3.forward, derecha = Vector3.right;
        if (camara != null)
        {
            adelante = Vector3.ProjectOnPlane(camara.forward, Vector3.up).normalized;
            derecha = Vector3.ProjectOnPlane(camara.right, Vector3.up).normalized;
        }
        Vector3 direccion = adelante * moverZ + derecha * moverX;
        if (direccion.sqrMagnitude > 1f) direccion.Normalize();

        bool seMueve = direccion.sqrMagnitude > 0.01f;

        if (seMueve)
        {
            Quaternion objetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, objetivo, velocidadGiro * Time.deltaTime);
        }

        if (enSuelo && velocidadY < 0)
            velocidadY = -2f;

        if (enSuelo && Input.GetKeyDown(KeyCode.Space))
        {
            velocidadY = Mathf.Sqrt(alturaSalto * -2f * gravedad);
            anim.SetTrigger("saltar");
        }

        velocidadY += gravedad * Time.deltaTime;

        Vector3 movimiento = direccion * velocidad;
        movimiento.y = velocidadY;
        cc.Move(movimiento * Time.deltaTime);

        anim.SetBool("estarAndando", seMueve);
    }

    Vector3 PosicionPies()
    {
        CharacterController c = cc != null ? cc : GetComponent<CharacterController>();
        if (c == null) return transform.position;
        float escala = transform.lossyScale.y;
        Vector3 centro = transform.TransformPoint(c.center);
        float mitadAltura = c.height * 0.5f * escala;
        return centro + Vector3.down * (mitadAltura + c.skinWidth);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = enSuelo ? Color.green : Color.red;
        Gizmos.DrawWireSphere(PosicionPies(), radioDeteccion);
    }
}
