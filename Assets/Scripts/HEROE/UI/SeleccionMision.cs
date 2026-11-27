using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

// Datos de una misión de la Selección de misión. Se editan en el Inspector de SeleccionMision.
[System.Serializable]
public class DatosMision
{
    [Tooltip("Identificador interno (para guardar la mejor calificación). No se muestra.")]
    public string id = "mision";
    public string nombre = "MISIÓN";
    [TextArea] public string detalle;
    [Tooltip("Nombre de la escena de la misión. Vacío = PRÓXIMAMENTE.")]
    public string escena;
    [Tooltip("Imagen opcional de la misión para la parte superior de la tarjeta.")]
    public Sprite imagen;
    public int pago = 400;
    public int reputacion = 40;
    [Tooltip("Costo en dinero para desplegar. 0 = gratis.")]
    public int costo;
    [Tooltip("Viales de Gen H que consume desplegar.")]
    public int costoViales;
    [Range(1, 3)] public int dificultad = 1;
    [Tooltip("Misión que abre el siguiente acto: se bloquea hasta alcanzar la reputación requerida.")]
    public bool misionDeActo;
    public int reputacionRequerida;
}

// Pantalla de Selección de misión (escena SeleccionMision).
// Tres contratos repetibles y la misión de acto, que se abre con reputación.
// Izquierda / derecha (A/D, flechas, cruceta o stick) cambia de tarjeta; ENTER, A o un clic despliega.
// ESC o B vuelve al menú principal. Q / E muestran que los demás actos siguen cerrados.
public class SeleccionMision : MonoBehaviour
{
    [SerializeField] DatosMision[] misiones;
    [SerializeField] TarjetaMision[] tarjetas;

    [Header("Barra superior")]
    [SerializeField] TextMeshProUGUI fondos;
    [SerializeField] TextMeshProUGUI reputacion;
    [SerializeField] TextMeshProUGUI viales;

    [SerializeField] TextMeshProUGUI aviso;
    [SerializeField] string escenaVolver = ResultadoMision.ESCENA_MENU;
    [SerializeField] string avisoActos = "EL ACTO II SE ABRE AL COMPLETAR LA MISIÓN DE ACTO.";

    int seleccionada;
    float avisoHasta;

    public void Configurar(DatosMision[] m, TarjetaMision[] t, TextMeshProUGUI fon, TextMeshProUGUI rep,
        TextMeshProUGUI via, TextMeshProUGUI avi)
    {
        misiones = m; tarjetas = t; fondos = fon; reputacion = rep; viales = via; aviso = avi;
    }

    void Awake()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ClicUI.Conectar(transform, "AyudaVolver", Volver);
        ClicUI.Conectar(transform, "AyudaDesplegar", () => Desplegar(seleccionada));
        ClicUI.Conectar(transform, "AyudaPestana", () => Avisar(avisoActos));
        ClicUI.Conectar(transform, "TeclaQ", () => Avisar(avisoActos));
        ClicUI.Conectar(transform, "TeclaE", () => Avisar(avisoActos));
        if (aviso != null) aviso.text = "";
    }

    void OnEnable()
    {
        Refrescar();
        StartCoroutine(SeleccionarPrimera());
    }

    IEnumerator SeleccionarPrimera()
    {
        yield return null; // esperar a que el EventSystem esté listo
        if (tarjetas == null || tarjetas.Length == 0 || EventSystem.current == null) yield break;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(tarjetas[Mathf.Clamp(seleccionada, 0, tarjetas.Length - 1)].gameObject);
    }

    void Update()
    {
        if (EntradaUI.Volver()) { Volver(); return; }
        if (EntradaUI.PestanaAnterior() || EntradaUI.PestanaSiguiente()) Avisar(avisoActos);

        // Si un clic en un espacio vacío o en un botón de la barra inferior quita la selección,
        // se recupera la última tarjeta (así el teclado y el mando nunca se quedan sin foco).
        EventSystem es = EventSystem.current;
        if (es != null && tarjetas != null && tarjetas.Length > 0)
        {
            GameObject actual = es.currentSelectedGameObject;
            if (actual == null || actual.GetComponent<TarjetaMision>() == null)
                es.SetSelectedGameObject(tarjetas[Mathf.Clamp(seleccionada, 0, tarjetas.Length - 1)].gameObject);
        }

        if (avisoHasta > 0f && Time.unscaledTime > avisoHasta)
        {
            avisoHasta = 0f;
            if (aviso != null) aviso.text = "";
        }
    }

    public void Refrescar()
    {
        int rep = Progreso.Reputacion;
        if (fondos != null) fondos.text = "$" + Progreso.Fondos.ToString("N0").Replace(",", ".");
        if (reputacion != null) reputacion.text = rep.ToString("000");
        if (viales != null) viales.text = $"{Progreso.Viales}/{Progreso.VialesMaximos}";
        if (misiones == null || tarjetas == null) return;
        for (int i = 0; i < tarjetas.Length && i < misiones.Length; i++)
            if (tarjetas[i] != null) tarjetas[i].Mostrar(misiones[i], Disponible(misiones[i]), rep);
    }

    public bool Disponible(DatosMision d) =>
        d != null && !string.IsNullOrEmpty(d.escena) && Progreso.Reputacion >= d.reputacionRequerida;

    public void Notificar(int indice) => seleccionada = indice;

    // Lo llama el On Click () de cada tarjeta (ENTER, botón A o clic).
    public void Desplegar(int indice)
    {
        if (misiones == null || indice < 0 || indice >= misiones.Length) return;
        DatosMision d = misiones[indice];

        if (Progreso.Reputacion < d.reputacionRequerida)
        { Avisar($"NECESITAS {d.reputacionRequerida} DE REPUTACIÓN. TIENES {Progreso.Reputacion}."); return; }
        if (string.IsNullOrEmpty(d.escena))
        { Avisar("ESTA MISIÓN TODAVÍA NO ESTÁ DISPONIBLE."); return; }
        if (Progreso.Fondos < d.costo)
        { Avisar("FONDOS INSUFICIENTES PARA EL SEGURO DEL AGENTE."); return; }
        if (Progreso.Viales < d.costoViales)
        { Avisar("NO TE QUEDAN SUFICIENTES VIALES DE GEN H."); return; }
        if (!Application.CanStreamedLevelBeLoaded(d.escena))
        { Avisar($"LA ESCENA \"{d.escena}\" NO ESTÁ EN BUILD SETTINGS."); return; }

        Progreso.Fondos -= d.costo;
        Progreso.Viales -= d.costoViales;
        ResultadoMision.Preparar(d.id, d.nombre, d.escena, d.pago, d.reputacion);
        SceneManager.LoadScene(d.escena);
    }

    // Para el On Click () de [ENTER] DESPLEGAR en la barra inferior.
    public void DesplegarSeleccionada() => Desplegar(seleccionada);

    // Para el On Click () de [Q] / [E] y [Q/E] PESTAÑA.
    public void AvisarActos() => Avisar(avisoActos);

    public void Volver() => ResultadoMision.IrA(escenaVolver);

    void Avisar(string texto)
    {
        if (aviso == null) return;
        aviso.text = texto;
        avisoHasta = Time.unscaledTime + 3f;
    }
}
