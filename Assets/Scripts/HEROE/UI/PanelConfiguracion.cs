using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

// Control de la pantalla de Configuración (Sonido / Idioma / Controles / Gráficos).
//   Q / E  o  LB / RB   cambian de pestaña (también con clic en la pestaña)
//   W / S, flechas, stick o cruceta   recorren las filas
//   A / D o izquierda / derecha   cambian el valor de la fila activa
//   F  o  Y   restablecen los valores de la pestaña actual
//   ESC  o  B   disparan "alVolver" (el constructor lo conecta para volver al menú)
public class PanelConfiguracion : MonoBehaviour
{
    [Header("Pestañas (mismo orden en todos los arreglos)")]
    [SerializeField] GameObject[] pestanasNormal;
    [SerializeField] GameObject[] pestanasActiva;
    [SerializeField] TextMeshProUGUI[] pestanasTexto;
    [SerializeField] GameObject[] contenidos;
    [SerializeField] FilaConfiguracion[] primeraFila;
    [SerializeField] int pestanaInicial = 0;

    [Header("Descripción")]
    [SerializeField] TextMeshProUGUI descTitulo;
    [SerializeField] TextMeshProUGUI descTexto;

    [Header("Ayudas de la barra inferior")]
    [SerializeField] GameObject ayudaAjustar;     // A/D AJUSTAR
    [SerializeField] GameObject ayudaReasignar;   // ENTER REASIGNAR (pestaña Controles)
    [SerializeField] int indiceControles = 2;

    [Header("Eventos")]
    public UnityEvent alVolver = new UnityEvent();
    [Tooltip("Se dispara al cambiar de fila o de pestaña. Útil para conectar un sonido.")]
    public UnityEvent alCambiarSeleccion = new UnityEvent();

    static readonly Color HUESO = new Color32(0xD5, 0xD9, 0xDC, 0xFF);
    static readonly Color AZUL = new Color32(0x87, 0x99, 0xAB, 0xFF);

    int actual;
    GameObject ultima;

    public int PestanaActual => actual;

    public void Configurar(GameObject[] normal, GameObject[] activa, TextMeshProUGUI[] textos, GameObject[] conts,
        FilaConfiguracion[] primeras, TextMeshProUGUI titulo, TextMeshProUGUI texto, GameObject ayAjustar, GameObject ayReasignar)
    {
        pestanasNormal = normal; pestanasActiva = activa; pestanasTexto = textos; contenidos = conts;
        primeraFila = primeras; descTitulo = titulo; descTexto = texto;
        ayudaAjustar = ayAjustar; ayudaReasignar = ayReasignar;
    }

    void Awake()
    {
        // Mouse: las teclas dibujadas y las ayudas de la barra inferior también se pueden clicar.
        ClicUI.Conectar(transform, "TeclaQ", PestanaAnterior);
        ClicUI.Conectar(transform, "TeclaE", PestanaSiguiente);
        ClicUI.Conectar(transform, "AyudaPestana", PestanaSiguiente);
        ClicUI.Conectar(transform, "AyudaVolver", Volver);
        ClicUI.Conectar(transform, "AyudaRestablecer", RestablecerPestana);
    }

    public void Volver() => alVolver?.Invoke();
    public void PestanaAnterior() { if (Cantidad > 0) CambiarPestana((actual - 1 + Cantidad) % Cantidad); }
    public void PestanaSiguiente() { if (Cantidad > 0) CambiarPestana((actual + 1) % Cantidad); }

    void OnEnable()
    {
        if (Cantidad == 0) return;
        actual = Mathf.Clamp(pestanaInicial, 0, Cantidad - 1);
        MostrarPestana(actual);
        if (Application.isPlaying) StartCoroutine(SeleccionarPrimera());
    }

    int Cantidad => contenidos != null ? contenidos.Length : 0;

    IEnumerator SeleccionarPrimera()
    {
        yield return null; // esperar a que el EventSystem esté listo
        SeleccionarPrimeraFila();
    }

    void Update()
    {
        if (Cantidad == 0) return;
        if (EntradaUI.Volver()) { Volver(); return; }
        if (EntradaUI.PestanaAnterior()) PestanaAnterior();
        else if (EntradaUI.PestanaSiguiente()) PestanaSiguiente();
        if (EntradaUI.Restablecer()) RestablecerPestana();

        // Si se hace clic en un espacio vacío, el teclado y el mando no pierden el foco.
        EventSystem es = EventSystem.current;
        if (es != null && es.currentSelectedGameObject == null && ultima != null && ultima.activeInHierarchy)
            es.SetSelectedGameObject(ultima);
    }

    public void CambiarPestana(int i)
    {
        if (Cantidad == 0) return;
        i = Mathf.Clamp(i, 0, Cantidad - 1);
        bool cambio = i != actual;
        actual = i;
        MostrarPestana(i);
        SeleccionarPrimeraFila();
        if (cambio) alCambiarSeleccion?.Invoke();
    }

    void MostrarPestana(int i)
    {
        for (int p = 0; p < Cantidad; p++)
        {
            bool on = p == i;
            if (contenidos[p] != null) contenidos[p].SetActive(on);
            if (pestanasNormal != null && p < pestanasNormal.Length && pestanasNormal[p] != null) pestanasNormal[p].SetActive(!on);
            if (pestanasActiva != null && p < pestanasActiva.Length && pestanasActiva[p] != null) pestanasActiva[p].SetActive(on);
            if (pestanasTexto != null && p < pestanasTexto.Length && pestanasTexto[p] != null) pestanasTexto[p].color = on ? HUESO : AZUL;
        }
        bool controles = i == indiceControles;
        if (ayudaAjustar != null) ayudaAjustar.SetActive(!controles);
        if (ayudaReasignar != null) ayudaReasignar.SetActive(controles);
    }

    void SeleccionarPrimeraFila()
    {
        if (primeraFila == null || actual >= primeraFila.Length || primeraFila[actual] == null) return;
        FilaConfiguracion f = primeraFila[actual];
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(f.gameObject);
        }
        Notificar(f);
    }

    public void RestablecerPestana()
    {
        if (actual >= Cantidad || contenidos[actual] == null) return;
        foreach (FilaConfiguracion f in contenidos[actual].GetComponentsInChildren<FilaConfiguracion>(true))
            f.Restablecer();
    }

    // Lo llama cada fila al quedar seleccionada.
    public void Notificar(FilaConfiguracion fila)
    {
        bool cambio = ultima != fila.gameObject;
        ultima = fila.gameObject;
        if (descTitulo != null) descTitulo.text = fila.Titulo;
        if (descTexto != null) descTexto.text = fila.Descripcion;
        if (cambio) alCambiarSeleccion?.Invoke();
    }
}
