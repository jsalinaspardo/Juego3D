using UnityEngine;

public class ControlAnimaciones : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Detecta si pulsas WASD o las flechas de dirección
        float moverX = Input.GetAxisRaw("Horizontal");
        float moverZ = Input.GetAxisRaw("Vertical");
        bool seMueve = (moverX != 0 || moverZ != 0);

        anim.SetBool("estaAndando", seMueve);

        // Detecta la barra espaciadora para saltar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("saltar");
        }
    }
}
