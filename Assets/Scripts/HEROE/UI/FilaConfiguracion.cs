using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[Serializable] public class EventoValor : UnityEvent<float> { }

// Una fila de la pantalla de Configuración.
// La fila es la que se selecciona con W/S, el stick o el mouse; A/D o la cruceta cambian su valor.
// Tipos:
//   Slider     usa un Slider normal de Unity (hijo "Slider"). El Audio Mixer se conecta en el
//              On Value Changed (Single) de ese Slider, como siempre. A/D lo mueven de 5 en 5 %.
//   Selector   "<  VALOR  >", da la vuelta al llegar al final (Subtítulos, Resolución…)
//   Segmentos  botones en fila, sin dar la vuelta (Tamaño del texto)
//   Accion     fila de la tabla de Controles: solo muestra tecla y botón
// Selector y Segmentos avisan por "alCambiarValor" (índice de la opción).
// Todo se guarda en PlayerPrefs con la clave "heroe.<clave>".
public class FilaConfiguracion : Selectable
{
    public enum TipoFila { Slider, Selector, Segmentos, Accion }

    [Header("Datos")]
    [SerializeField] TipoFila tipo = TipoFila.Selector;
    [SerializeField] string clave;
    [SerializeField] string titulo;
    [SerializeField, TextArea(2, 6)] string descripcion;
    [SerializeField] string[] opciones;
    [Tooltip("Slider: porcentaje (0 a 100). Selector y Segmentos: índice de la opción.")]
    [SerializeField] float valorPorDefecto;
    [Tooltip("Slider: cuánto se mueve con A/D, en porcentaje.")]
    [SerializeField] float paso = 5f;

    [Header("Piezas")]
    [SerializeField] PanelConfiguracion panel;
    [SerializeField] GameObject fondoNormal;
    [SerializeField] GameObject fondoActivo;
    [SerializeField] Slider slider;
    [SerializeField] Image sliderRelleno;
    [SerializeField] RectTransform selector;
    [SerializeField] TextMeshProUGUI textoValor;
    [SerializeField] RectTransform[] segmentos;
    [SerializeField] GameObject[] segmentosNormal;
    [SerializeField] GameObject[] segmentosActivo;
    [SerializeField] TextMeshProUGUI[] segmentosTexto;

    [Header("Eventos (Selector y Segmentos)")]
    public EventoValor alCambiarValor = new EventoValor();

    static readonly Color HUESO = new Color32(0xD5, 0xD9, 0xDC, 0xFF);
    static readonly Color AZUL = new Color32(0x87, 0x99, 0xAB, 0xFF);
    static readonly Color ROJO = new Color32(0xC3, 0x3A, 0x37, 0xFF);

    float valor;
    bool cargado;
    bool activaVisual;

    public float Valor => tipo == TipoFila.Slider && slider != null ? slider.value : valor;
    public string Titulo => titulo;
    public string Descripcion => descripcion;
    public TipoFila Tipo => tipo;
    public Slider SliderUI => slider;
    public float ValorPorDefecto => valorPorDefecto;

    // ---------- lo llama el constructor del Editor ----------
    public void Configurar(TipoFila t, string c, string tit, string desc, string[] ops, float porDefecto,
        PanelConfiguracion p, GameObject normal, GameObject activo)
    {
        tipo = t; clave = c; titulo = tit; descripcion = desc; opciones = ops;
        valorPorDefecto = porDefecto; valor = porDefecto;
        panel = p; fondoNormal = normal; fondoActivo = activo;
        transition = Transition.None;
    }

    public void ConfigurarSlider(Slider s, Image relleno, TextMeshProUGUI porcentaje)
    {
        slider = s; sliderRelleno = relleno; textoValor = porcentaje;
    }

    public void ConfigurarSelector(RectTransform caja, TextMeshProUGUI texto)
    {
        selector = caja; textoValor = texto;
    }

    public void ConfigurarSegmentos(RectTransform[] cajas, GameObject[] normales, GameObject[] activos, TextMeshProUGUI[] textos)
    {
        segmentos = cajas; segmentosNormal = normales; segmentosActivo = activos; segmentosTexto = textos;
    }

    // ---------- ciclo de vida ----------
    protected override void Awake()
    {
        base.Awake();
        AsegurarZonaDeClic();
        if (Application.isPlaying && slider != null)
        {
            slider.onValueChanged.AddListener(AlMoverSlider);
            // El Slider no es una parada aparte para el teclado: se maneja desde la fila.
            slider.navigation = new Navigation { mode = Navigation.Mode.None };
        }
    }

    // La fila necesita una imagen propia (transparente) que reciba el mouse siempre.
    void AsegurarZonaDeClic()
    {
        Image zona = GetComponent<Image>();
        if (zona == null)
        {
            zona = gameObject.AddComponent<Image>();
            zona.color = new Color(0f, 0f, 0f, 0f);
        }
        zona.raycastTarget = true;
        if (targetGraphic == null) targetGraphic = zona;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (Application.isPlaying && !cargado)
        {
            cargado = true;
            string k = "heroe." + clave;
            bool hay = !string.IsNullOrEmpty(clave) && PlayerPrefs.HasKey(k);
            if (tipo == TipoFila.Slider)
            {
                // Al asignar el valor se dispara On Value Changed: el Audio Mixer queda al día.
                if (slider != null && hay) slider.value = PlayerPrefs.GetFloat(k);
            }
            else valor = hay ? PlayerPrefs.GetFloat(k) : valorPorDefecto;
        }
        else if (!Application.isPlaying && tipo != TipoFila.Slider)
        {
            valor = valorPorDefecto;
        }
        activaVisual = EstaSeleccionada();
        Refrescar();
    }

    protected override void OnDestroy()
    {
        if (slider != null) slider.onValueChanged.RemoveListener(AlMoverSlider);
        base.OnDestroy();
    }

    void Update()
    {
        EventSystem es = EventSystem.current;
        // Al hacer clic o arrastrar el Slider, Unity lo selecciona a él: la selección vuelve a la
        // fila (el arrastre sigue funcionando) para que W/S y el resaltado sigan bien.
        if (es != null && slider != null && es.currentSelectedGameObject == slider.gameObject)
            es.SetSelectedGameObject(gameObject);

        bool a = EstaSeleccionada();
        if (a != activaVisual) { activaVisual = a; Refrescar(); }
    }

    bool EstaSeleccionada()
    {
        EventSystem es = EventSystem.current;
        return es != null && es.currentSelectedGameObject == gameObject;
    }

    // ---------- navegación ----------
    public override void OnMove(AxisEventData datos)
    {
        if (tipo != TipoFila.Accion && (datos.moveDir == MoveDirection.Left || datos.moveDir == MoveDirection.Right))
        {
            Ajustar(datos.moveDir == MoveDirection.Right ? 1 : -1);
            datos.Use();
            return;
        }
        base.OnMove(datos);
    }

    public override void OnSelect(BaseEventData datos)
    {
        base.OnSelect(datos);
        if (panel != null) panel.Notificar(this);
        activaVisual = true;
        Refrescar();
    }

    public override void OnDeselect(BaseEventData datos)
    {
        base.OnDeselect(datos);
        activaVisual = EstaSeleccionada();
        Refrescar();
    }

    // El mouse selecciona la fila al pasar por encima (igual que en el menú principal).
    public override void OnPointerEnter(PointerEventData datos)
    {
        base.OnPointerEnter(datos);
        if (IsInteractable() && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    // Clic en el selector: mitad izquierda = anterior, mitad derecha = siguiente.
    // Clic en un segmento: lo elige. (El Slider maneja su propio clic y arrastre.)
    public override void OnPointerDown(PointerEventData datos)
    {
        base.OnPointerDown(datos);
        if (datos.button != PointerEventData.InputButton.Left) return;
        Camera cam = datos.pressEventCamera;

        if (tipo == TipoFila.Selector && selector != null &&
            RectTransformUtility.ScreenPointToLocalPointInRectangle(selector, datos.position, cam, out Vector2 q) &&
            selector.rect.Contains(q))
        {
            Ajustar(q.x < selector.rect.center.x ? -1 : 1);
        }
        else if (tipo == TipoFila.Segmentos && segmentos != null)
        {
            for (int i = 0; i < segmentos.Length; i++)
                if (segmentos[i] != null && RectTransformUtility.RectangleContainsScreenPoint(segmentos[i], datos.position, cam))
                    FijarValor(i, true);
        }
    }

    // ---------- valor ----------
    public void Ajustar(int dir)
    {
        switch (tipo)
        {
            case TipoFila.Slider:
                if (slider == null) return;
                float p = paso / 100f;
                float n = Mathf.Round(slider.normalizedValue / p) * p + dir * p;
                slider.normalizedValue = Mathf.Clamp01(n);   // dispara On Value Changed
                break;
            case TipoFila.Selector:
                if (opciones == null || opciones.Length == 0) return;
                int cant = opciones.Length;
                FijarValor(((int)valor + dir + cant) % cant, true);
                break;
            case TipoFila.Segmentos:
                if (opciones == null || opciones.Length == 0) return;
                FijarValor(Mathf.Clamp((int)valor + dir, 0, opciones.Length - 1), true);
                break;
        }
    }

    // Selector y Segmentos (el Slider guarda su valor en AlMoverSlider).
    public void FijarValor(float v, bool avisar)
    {
        if (tipo == TipoFila.Slider) { if (slider != null) slider.value = v; return; }
        if (Mathf.Approximately(v, valor)) { Refrescar(); return; }
        valor = v;
        Guardar(valor);
        Refrescar();
        if (avisar) alCambiarValor?.Invoke(valor);
    }

    void AlMoverSlider(float v)
    {
        Guardar(v);
        Refrescar();
    }

    void Guardar(float v)
    {
        if (!Application.isPlaying || string.IsNullOrEmpty(clave)) return;
        PlayerPrefs.SetFloat("heroe." + clave, v);
        PlayerPrefs.Save();
    }

    public void Restablecer()
    {
        switch (tipo)
        {
            case TipoFila.Accion: return;
            case TipoFila.Slider:
                if (slider != null) slider.normalizedValue = Mathf.Clamp01(valorPorDefecto / 100f);
                return;
            default:
                FijarValor(valorPorDefecto, true);
                return;
        }
    }

    // ---------- visual ----------
    public void Refrescar()
    {
        bool activa = activaVisual;
        if (fondoNormal != null && fondoNormal.activeSelf == activa) fondoNormal.SetActive(!activa);
        if (fondoActivo != null && fondoActivo.activeSelf != activa) fondoActivo.SetActive(activa);

        switch (tipo)
        {
            case TipoFila.Slider:
                if (sliderRelleno != null) sliderRelleno.color = activa ? HUESO : ROJO;
                if (textoValor != null && slider != null)
                {
                    textoValor.text = Mathf.RoundToInt(slider.normalizedValue * 100f) + "%";
                    textoValor.color = activa ? HUESO : AZUL;
                }
                break;

            case TipoFila.Selector:
                if (textoValor != null && opciones != null && opciones.Length > 0)
                {
                    int i = Mathf.Clamp((int)valor, 0, opciones.Length - 1);
                    textoValor.text = "<      " + opciones[i] + "      >";
                }
                break;

            case TipoFila.Segmentos:
                int sel = Mathf.RoundToInt(valor);
                if (segmentosNormal != null)
                    for (int s = 0; s < segmentosNormal.Length; s++)
                    {
                        bool on = s == sel;
                        if (segmentosNormal[s] != null) segmentosNormal[s].SetActive(!on);
                        if (segmentosActivo != null && s < segmentosActivo.Length && segmentosActivo[s] != null) segmentosActivo[s].SetActive(on);
                        if (segmentosTexto != null && s < segmentosTexto.Length && segmentosTexto[s] != null) segmentosTexto[s].color = on ? HUESO : AZUL;
                    }
                break;
        }
    }
}
