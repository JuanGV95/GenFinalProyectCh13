using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

// Menú de pausa de las misiones. Va en la raíz del prefab PausePanel (que trae su propio Canvas),
// así que basta con arrastrar el prefab a cualquier escena de nivel.
// - ESC o Start abre y cierra la pausa (detiene el tiempo con Time.timeScale = 0).
// - CONTINUAR, REINICIAR MISIÓN y SALIR A MISIONES se conectan en el On Click () de cada botón.
// - El panel de la derecha muestra los contadores de ResultadoMision y el tiempo de la misión.
public class PanelPausa : MonoBehaviour
{
    [SerializeField] GameObject contenido;

    [Header("Estado de la misión")]
    [SerializeField] TextMeshProUGUI titulo;
    [SerializeField] TextMeshProUGUI rehenes;
    [SerializeField] TextMeshProUGUI ciborgs;
    [SerializeField] TextMeshProUGUI viales;
    [SerializeField] TextMeshProUGUI danos;
    [SerializeField] TextMeshProUGUI tiempo;
    [SerializeField] TextMeshProUGUI agente;

    [Tooltip("Escena a la que lleva SALIR A MISIONES.")]
    [SerializeField] string escenaSalir = ResultadoMision.ESCENA_SELECCION;

    public UnityEvent alPausar;
    public UnityEvent alReanudar;

    public static bool EnPausa { get; private set; }

    CursorLockMode cursorAntes;
    bool cursorVisibleAntes;

    public void Configurar(GameObject cont, TextMeshProUGUI tit, TextMeshProUGUI reh, TextMeshProUGUI cib,
        TextMeshProUGUI via, TextMeshProUGUI dan, TextMeshProUGUI tie, TextMeshProUGUI age)
    {
        contenido = cont; titulo = tit; rehenes = reh; ciborgs = cib;
        viales = via; danos = dan; tiempo = tie; agente = age;
    }

    void Awake()
    {
        if (contenido != null) contenido.SetActive(false);
        EnPausa = false;
        AsegurarEventSystem();
        ClicUI.Conectar(transform, "AyudaContinuar", Reanudar);
    }

    void OnDestroy()
    {
        if (!EnPausa) return;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        EnPausa = false;
    }

    void Update()
    {
        if (EntradaUI.Pausa())
        {
            if (EnPausa) Reanudar();
            else Pausar();
        }
        if (EnPausa) ActualizarEstado();
    }

    public void Pausar()
    {
        if (EnPausa) return;
        EnPausa = true;
        cursorAntes = Cursor.lockState;
        cursorVisibleAntes = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        ActualizarEstado();
        if (contenido != null) contenido.SetActive(true);
        alPausar?.Invoke();
    }

    public void Reanudar()
    {
        if (!EnPausa) return;
        EnPausa = false;
        if (contenido != null) contenido.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = cursorAntes;
        Cursor.visible = cursorVisibleAntes;
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        alReanudar?.Invoke();
    }

    public void ReiniciarMision()
    {
        EnPausa = false;
        ResultadoMision.RepetirMision();
    }

    public void SalirDeLaMision()
    {
        EnPausa = false;
        ResultadoMision.IrA(escenaSalir);
    }

    void ActualizarEstado()
    {
        if (titulo != null) titulo.text = "MISIÓN: " + ResultadoMision.nombreMision;
        if (rehenes != null) rehenes.text = $"{ResultadoMision.rehenesRescatados}/{ResultadoMision.rehenesTotales}";
        if (ciborgs != null) ciborgs.text = ResultadoMision.ciborgsDesactivados.ToString();
        if (viales != null) viales.text = $"{Progreso.Viales}/{Progreso.VialesMaximos}";
        if (danos != null) danos.text = "$" + ResultadoMision.danos;
        if (tiempo != null)
        {
            int s = Mathf.FloorToInt(Time.timeSinceLevelLoad);
            tiempo.text = $"{s / 60:00}:{s % 60:00}";
        }
        if (agente != null) agente.text = ResultadoMision.agente;
    }

    // Si el nivel no tiene EventSystem, el menú no respondería al teclado ni al mouse.
    static void AsegurarEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null) return;
        GameObject es = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
        es.AddComponent<InputSystemUIInputModule>();
#else
        es.AddComponent<StandaloneInputModule>();
#endif
    }
}
