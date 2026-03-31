using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float velocidadParallax = 0.5f;

    private Transform camara;
    private float longitudTextura;
    private Transform copia1;
    private Transform copia2;
    private Vector3 posicionInicial;

    private void Start()
    {
        camara = Camera.main.transform;
        longitudTextura = GetComponent<SpriteRenderer>().bounds.size.x;
        posicionInicial = transform.position;

        copia1 = Instantiate(gameObject, transform.parent).transform;
        copia1.localScale = transform.localScale;
        Destroy(copia1.GetComponent<ParallaxBackground>());

        copia2 = Instantiate(gameObject, transform.parent).transform;
        copia2.localScale = transform.localScale;
        Destroy(copia2.GetComponent<ParallaxBackground>());

        // Posiciona las copias al lado
        copia1.position = new Vector3(posicionInicial.x - longitudTextura, posicionInicial.y, posicionInicial.z);
        copia2.position = new Vector3(posicionInicial.x + longitudTextura, posicionInicial.y, posicionInicial.z);
    }

    private void LateUpdate()
    {
        // Mueve con parallax
        float nuevaX = camara.position.x * velocidadParallax;
        transform.position = new Vector3(nuevaX, posicionInicial.y, posicionInicial.z);

        // Copias siguen al principal
        copia1.position = new Vector3(transform.position.x - longitudTextura, posicionInicial.y, posicionInicial.z);
        copia2.position = new Vector3(transform.position.x + longitudTextura, posicionInicial.y, posicionInicial.z);

        // Si la cámara avanza más de una longitud
        // el sprite principal salta para mantenerse centrado
        float diff = camara.position.x - transform.position.x;
        if (diff > longitudTextura)
            posicionInicial.x += longitudTextura;
        else if (diff < -longitudTextura)
            posicionInicial.x -= longitudTextura;
    }
}