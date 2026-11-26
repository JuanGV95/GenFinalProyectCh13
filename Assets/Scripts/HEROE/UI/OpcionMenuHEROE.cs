using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// H-EROE — Una opción del menú principal (va junto al Button de la opción).
// El Button hace el Sprite Swap de la barra roja; este script anima lo que el
// Sprite Swap no puede: el texto se corre y crece, y aparece la tecla ENTER.
// Funciona igual con mouse, teclado y mando.
[RequireComponent(typeof(Button))]
public class OpcionMenuHEROE : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    [SerializeField] MenuPrincipalHEROE menu;
    [SerializeField] RectTransform texto;
    [SerializeField] TextMeshProUGUI textoTMP;
    [SerializeField] GameObject teclaEnter;
    [SerializeField, TextArea] string descripcion;

    [Header("Animación")]
    [SerializeField] float xNormal = 44f;
    [SerializeField] float xActiva = 114f;
    [SerializeField] float tamNormal = 30f;
    [SerializeField] float tamActivo = 32f;
    [SerializeField] float velocidad = 18f;

    Button boton;

    public string Descripcion => descripcion;

    public void Configurar(RectTransform txt, TextMeshProUGUI tmp, GameObject tecla, string desc)
    {
        texto = txt;
        textoTMP = tmp;
        teclaEnter = tecla;
        descripcion = desc;
    }

    public void AsignarMenu(MenuPrincipalHEROE m) => menu = m;

    void Awake() => boton = GetComponent<Button>();

    void Update()
    {
        EventSystem es = EventSystem.current;
        bool activa = es != null && es.currentSelectedGameObject == gameObject;

        if (teclaEnter != null && teclaEnter.activeSelf != activa) teclaEnter.SetActive(activa);

        // unscaledDeltaTime: también se anima con el juego en pausa (timeScale = 0)
        float t = 1f - Mathf.Exp(-velocidad * Time.unscaledDeltaTime);
        if (texto != null)
        {
            Vector2 p = texto.anchoredPosition;
            p.x = Mathf.Lerp(p.x, activa ? xActiva : xNormal, t);
            texto.anchoredPosition = p;
        }
        if (textoTMP != null) textoTMP.fontSize = Mathf.Lerp(textoTMP.fontSize, activa ? tamActivo : tamNormal, t);
    }

    public void OnSelect(BaseEventData datos)
    {
        if (menu != null) menu.Notificar(this);
    }

    // El mouse selecciona la opción al pasar por encima: así mouse, teclado y mando
    // comparten una sola opción activa.
    public void OnPointerEnter(PointerEventData datos)
    {
        if (boton != null && boton.interactable && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
