using UnityEngine;
using TMPro;

// Pantalla de Victoria o de Derrota (cada una en su propia escena).
// Lee ResultadoMision, calcula la Hoja de Liquidación y la calificación,
// y suma fondos y reputación una sola vez.
//   Victoria: ENTER continúa a la Selección de misión, R repite la misión.
//   Derrota:  ENTER reintenta la misión, ESC vuelve a la Selección de misión.
public class PantallaResultado : MonoBehaviour
{
    [SerializeField] bool esVictoria = true;

    [Header("Hoja de Liquidación")]
    [SerializeField] TextMeshProUGUI[] etiquetas = new TextMeshProUGUI[5];
    [SerializeField] TextMeshProUGUI[] valores = new TextMeshProUGUI[5];
    [SerializeField] TextMeshProUGUI total;
    [SerializeField] RectTransform barraValor;
    [SerializeField] float anchoBarra = 730f;
    [SerializeField] TextMeshProUGUI barraTexto;
    [SerializeField] TextMeshProUGUI agente;
    [SerializeField] TextMeshProUGUI alias;

    [Header("Calificación y recompensa")]
    [SerializeField] TextMeshProUGUI calificacion;
    [SerializeField] TextMeshProUGUI recompensa;

    [Header("Barra superior")]
    [SerializeField] TextMeshProUGUI fondos;
    [SerializeField] TextMeshProUGUI reputacion;
    [SerializeField] TextMeshProUGUI viales;

    [Header("Balance (ajustable)")]
    [SerializeField] int puntosPorRehen = 100;
    [SerializeField] int puntosPorCiborg = 25;
    [Tooltip("Segundos de misión sin penalización.")]
    [SerializeField] int segundosSinPenalizacion = 300;
    [Tooltip("Cada cuántos segundos extra se resta 1 punto.")]
    [SerializeField] int segundosPorPunto = 10;
    [Tooltip("Reputación que abre el Acto II (igual que la misión de acto).")]
    [SerializeField] int metaReputacion = 300;

    static readonly Color ROJO = new Color32(195, 58, 55, 255);
    static readonly Color HUESO = new Color32(213, 217, 220, 255);

    public void Configurar(bool victoria, TextMeshProUGUI[] etq, TextMeshProUGUI[] val, TextMeshProUGUI tot,
        RectTransform barra, TextMeshProUGUI barraTxt, TextMeshProUGUI age, TextMeshProUGUI ali,
        TextMeshProUGUI cal, TextMeshProUGUI rec, TextMeshProUGUI fon, TextMeshProUGUI rep, TextMeshProUGUI via)
    {
        esVictoria = victoria; etiquetas = etq; valores = val; total = tot; barraValor = barra;
        barraTexto = barraTxt; agente = age; alias = ali; calificacion = cal; recompensa = rec;
        fondos = fon; reputacion = rep; viales = via;
    }

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ClicUI.Conectar(transform, "AyudaContinuar", Continuar);
        ClicUI.Conectar(transform, "AyudaRepetir", Repetir);
        ClicUI.Conectar(transform, "AyudaVolver", Volver);
        Mostrar();
    }

    void Update()
    {
        if (EntradaUI.Confirmar()) Continuar();
        else if (esVictoria && EntradaUI.Repetir()) Repetir();
        else if (!esVictoria && EntradaUI.Volver()) Volver();
    }

    public void Continuar()
    {
        if (esVictoria) ResultadoMision.IrA(ResultadoMision.ESCENA_SELECCION);
        else ResultadoMision.RepetirMision();
    }

    public void Repetir() => ResultadoMision.RepetirMision();

    public void Volver() => ResultadoMision.IrA(ResultadoMision.ESCENA_SELECCION);

    void Mostrar()
    {
        int rescatados = ResultadoMision.rehenesRescatados;
        int totales = ResultadoMision.rehenesTotales;
        int segs = Mathf.FloorToInt(ResultadoMision.segundos);

        int pRehenes = rescatados * puntosPorRehen;
        int pCiborgs = ResultadoMision.ciborgsDesactivados * puntosPorCiborg;
        int pTiempo = -Mathf.Max(0, (segs - segundosSinPenalizacion) / Mathf.Max(1, segundosPorPunto));
        int puntos = pRehenes + pCiborgs + pTiempo;
        string nota = Calificar(rescatados, totales);

        Fila(0, $"REHENES RESCATADOS  {rescatados}/{totales}", "+" + pRehenes, false);
        Fila(1, $"CIBORGS DESACTIVADOS  {ResultadoMision.ciborgsDesactivados}", "+" + pCiborgs, false);
        Fila(2, $"VIALES DE GEN H  {ResultadoMision.vialesUsados}/{Progreso.VialesMaximos}", "0", false);
        Fila(3, "DAÑOS A LA PROPIEDAD", ResultadoMision.danos > 0 ? "-$" + ResultadoMision.danos : "$0", ResultadoMision.danos > 0);
        Fila(4, $"PENALIZACIÓN DE TIEMPO  {segs / 60:00}:{segs % 60:00}", pTiempo.ToString(), pTiempo < 0);
        if (total != null) total.text = puntos.ToString();
        if (calificacion != null) calificacion.text = nota;
        if (agente != null) agente.text = ResultadoMision.agente;
        if (alias != null) alias.text = ResultadoMision.alias;

        int fondosGanados = esVictoria ? ResultadoMision.pago - ResultadoMision.danos : -ResultadoMision.danos;
        int repGanada = esVictoria ? ResultadoMision.reputacionPorCompletar : 0;

        if (!ResultadoMision.aplicado)
        {
            ResultadoMision.aplicado = true;
            Progreso.Fondos += fondosGanados;
            Progreso.Reputacion += repGanada;
            if (esVictoria) Progreso.RegistrarCorrida(ResultadoMision.idMision, nota);
        }

        if (recompensa != null)
        {
            if (esVictoria) recompensa.text = $"+${Mathf.Max(0, fondosGanados)}  /  +{repGanada} REPUTACIÓN";
            else recompensa.text = ResultadoMision.danos > 0
                ? $"MULTA DE ${ResultadoMision.danos} DEDUCIDA DEL SUELDO"
                : "SIN MULTA. PERO QUEDA EN TU EXPEDIENTE";
        }

        int rep = Progreso.Reputacion;
        if (barraValor != null)
        {
            Vector2 t = barraValor.sizeDelta;
            t.x = anchoBarra * Mathf.Clamp01((float)rep / Mathf.Max(1, metaReputacion));
            barraValor.sizeDelta = t;
        }
        if (barraTexto != null) barraTexto.text = $"{rep:000} / {metaReputacion:000} para abrir el Acto II";
        if (fondos != null) fondos.text = "$" + Progreso.Fondos.ToString("N0").Replace(",", ".");
        if (reputacion != null) reputacion.text = rep.ToString("000");
        if (viales != null) viales.text = $"{Progreso.Viales}/{Progreso.VialesMaximos}";
    }

    void Fila(int i, string etiqueta, string valor, bool negativo)
    {
        if (etiquetas != null && i < etiquetas.Length && etiquetas[i] != null) etiquetas[i].text = etiqueta;
        if (valores != null && i < valores.Length && valores[i] != null)
        {
            valores[i].text = valor;
            valores[i].color = negativo ? ROJO : HUESO;
        }
    }

    // S: todos los rehenes y sin daños. A: todos los rehenes. B: 60 % o más. C: menos. D: misión fallida.
    string Calificar(int rescatados, int totales)
    {
        if (!esVictoria) return "D";
        float r = totales > 0 ? (float)rescatados / totales : 1f;
        if (r >= 1f && ResultadoMision.danos <= 0) return "S";
        if (r >= 1f) return "A";
        if (r >= 0.6f) return "B";
        return "C";
    }
}
