using UnityEngine;

[DefaultExecutionOrder(-50)]
public class ObjetivoCamara : MonoBehaviour
{
    public Transform personaje;
    public float alturaCabeza = 1.5f;
    public float suavizadoAltura = 0.25f;

    [Header("Ratón")]
    public float sensibilidad = 2f;
    public bool invertirY = false;
    public float anguloMinimo = -35f;
    public float anguloMaximo = 60f;
    public bool mostrarPunto = true;

    private ControlAnimaciones control;
    private float alturaObjetivo;
    private float altura;
    private float velocidadAltura;
    private float giroHorizontal;
    private float giroVertical = 10f;

    void Start()
    {
        if (personaje == null)
        {
            ControlAnimaciones encontrado = FindAnyObjectByType<ControlAnimaciones>();
            if (encontrado == null)
            {
                Debug.LogError("ObjetivoCamara: no se ha encontrado el personaje");
                enabled = false;
                return;
            }
            personaje = encontrado.transform;
        }

        control = personaje.GetComponent<ControlAnimaciones>();
        alturaObjetivo = altura = personaje.position.y;
        transform.position = personaje.position + Vector3.up * alturaCabeza;
        giroHorizontal = personaje.eulerAngles.y;

        BloquearCursor(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            BloquearCursor(false);
        else if (Cursor.lockState != CursorLockMode.Locked && Input.GetMouseButtonDown(0))
            BloquearCursor(true);

        if (Cursor.lockState != CursorLockMode.Locked) return;

        giroHorizontal += Input.GetAxisRaw("Mouse X") * sensibilidad;
        giroVertical += Input.GetAxisRaw("Mouse Y") * sensibilidad * (invertirY ? 1f : -1f);
        giroVertical = Mathf.Clamp(giroVertical, anguloMinimo, anguloMaximo);
    }

    void LateUpdate()
    {
        float y = personaje.position.y;

        if (control == null || control.EnSuelo || y < alturaObjetivo)
            alturaObjetivo = y;

        altura = Mathf.SmoothDamp(altura, alturaObjetivo, ref velocidadAltura, suavizadoAltura);
        transform.position = new Vector3(personaje.position.x, altura + alturaCabeza, personaje.position.z);
        transform.rotation = Quaternion.Euler(giroVertical, giroHorizontal, 0f);
    }

    void BloquearCursor(bool bloquear)
    {
        Cursor.lockState = bloquear ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !bloquear;
    }

    void OnGUI()
    {
        if (!mostrarPunto || Cursor.lockState != CursorLockMode.Locked) return;
        GUI.DrawTexture(new Rect(Screen.width / 2f - 2, Screen.height / 2f - 2, 4, 4), Texture2D.whiteTexture);
    }
}
