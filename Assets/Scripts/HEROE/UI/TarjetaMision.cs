using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// Una tarjeta de la Selección de misión (va junto al Button de la tarjeta).
// El Button cambia el fondo al seleccionarla (Sprite Swap: borde rojo y sombra);
// este script sube la tarjeta, muestra el botón DESPLEGAR y escribe sus datos.
// La misma clase sirve para la misión de acto (usa además la barra de reputación y el candado).
[RequireComponent(typeof(Button))]
public class TarjetaMision : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    public SeleccionMision seleccion;
    public int indice;
    [Tooltip("Hijo con todo el dibujo de la tarjeta: es lo que sube al seleccionarla.")]
    public RectTransform cuerpo;

    [Header("Contenido")]
    public Image imagen;
    public TextMeshProUGUI nombre;
    public TextMeshProUGUI detalle;
    public TextMeshProUGUI pago;
    public TextMeshProUGUI mejor;
    public TextMeshProUGUI costo;
    public TextMeshProUGUI estado;
    public Image[] dificultad;
    public GameObject botonDesplegar;

    [Header("Solo misión de acto")]
    public RectTransform barraValor;
    public float anchoBarra = 490f;
    public TextMeshProUGUI barraTexto;
    public TextMeshProUGUI faltan;
    public TextMeshProUGUI pista;
    public GameObject candado;

    [Header("Animación")]
    public float subida = 16f;
    public float velocidad = 18f;

    static readonly Color LLENO = new Color32(213, 217, 220, 255);
    static readonly Color VACIO = new Color32(17, 22, 29, 255);

    Button boton;
    float yBase;
    bool disponible;

    void Awake()
    {
        boton = GetComponent<Button>();
        if (cuerpo != null) yBase = cuerpo.anchoredPosition.y;
    }

    void Update()
    {
        EventSystem es = EventSystem.current;
        bool activa = es != null && es.currentSelectedGameObject == gameObject;

        bool mostrarBoton = activa && disponible;
        if (botonDesplegar != null && botonDesplegar.activeSelf != mostrarBoton) botonDesplegar.SetActive(mostrarBoton);

        if (cuerpo != null)
        {
            float t = 1f - Mathf.Exp(-velocidad * Time.unscaledDeltaTime);
            Vector2 p = cuerpo.anchoredPosition;
            p.y = Mathf.Lerp(p.y, yBase + (activa ? subida : 0f), t);
            cuerpo.anchoredPosition = p;
        }
    }

    public void Mostrar(DatosMision d, bool esDisponible, int reputacionActual)
    {
        disponible = esDisponible;
        bool proximamente = string.IsNullOrEmpty(d.escena);
        bool bloqueada = reputacionActual < d.reputacionRequerida;

        if (nombre != null) nombre.text = d.nombre;
        if (detalle != null) detalle.text = d.detalle;
        if (imagen != null && d.imagen != null)
        {
            imagen.sprite = d.imagen;
            imagen.color = Color.white;
            imagen.preserveAspect = true;
        }
        if (pago != null) pago.text = $"${d.pago}  /  +{d.reputacion} REP";
        if (dificultad != null)
            for (int i = 0; i < dificultad.Length; i++)
                if (dificultad[i] != null) dificultad[i].color = i < d.dificultad ? LLENO : VACIO;
        if (mejor != null) mejor.text = $"{Progreso.MejorCalificacion(d.id)}  /  x{Progreso.Corridas(d.id)}";
        if (costo != null)
        {
            if (d.misionDeActo) costo.text = $"COSTO: {d.costoViales} VIALES DE GEN H   /   RECOMPENSA: ${d.pago}";
            else costo.text = d.costo <= 0 ? "COSTO: GRATIS" : $"COSTO: ${d.costo} DE SEGURO";
        }
        if (estado != null) estado.text = proximamente ? "PRÓXIMAMENTE" : "";

        // Misión de acto
        if (barraValor != null)
        {
            float f = d.reputacionRequerida > 0 ? Mathf.Clamp01((float)reputacionActual / d.reputacionRequerida) : 1f;
            Vector2 s = barraValor.sizeDelta;
            s.x = anchoBarra * f;
            barraValor.sizeDelta = s;
        }
        if (barraTexto != null) barraTexto.text = $"{reputacionActual:000} / {d.reputacionRequerida:000}";
        if (faltan != null)
        {
            faltan.gameObject.SetActive(bloqueada || proximamente);
            faltan.text = bloqueada ? $"TE FALTAN {d.reputacionRequerida - reputacionActual} PUNTOS" : "PRÓXIMAMENTE";
        }
        if (pista != null) pista.gameObject.SetActive(bloqueada);
        if (candado != null) candado.SetActive(bloqueada);
    }

    public void OnSelect(BaseEventData datos)
    {
        if (seleccion != null) seleccion.Notificar(indice);
    }

    // El mouse selecciona la tarjeta al pasar por encima: mouse, teclado y mando comparten la selección.
    public void OnPointerEnter(PointerEventData datos)
    {
        if (boton != null && boton.interactable && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
