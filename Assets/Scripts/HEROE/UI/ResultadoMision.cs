using UnityEngine;
using UnityEngine.SceneManagement;

// Datos de la misión en curso y cambios de escena entre Selección, Misión, Victoria y Derrota.
//
// Para programación (GameManager):
//   - Durante la misión, sumar en estos contadores:
//       ResultadoMision.rehenesRescatados++;   ResultadoMision.rehenesTotales = 5;
//       ResultadoMision.ciborgsDesactivados++; ResultadoMision.vialesUsados++;
//       ResultadoMision.danos += 50;           // dinero en daños a la propiedad
//   - Al terminar:
//       ResultadoMision.TerminarMision(true);  // abre la escena Victoria
//       ResultadoMision.TerminarMision(false); // abre la escena Derrota
public static class ResultadoMision
{
    // Nombres de escena (deben estar en File > Build Profiles / Build Settings).
    public const string ESCENA_MENU = "MainMenu";
    public const string ESCENA_SELECCION = "SeleccionMision";
    public const string ESCENA_VICTORIA = "Victoria";
    public const string ESCENA_DERROTA = "Derrota";

    // Misión desplegada (la llena la Selección de misión).
    public static string idMision = "laboratorios-alfa";
    public static string nombreMision = "LABORATORIOS ALFA";
    public static string escenaMision;
    public static int pago = 400;
    public static int reputacionPorCompletar = 40;
    public static string agente = "MAYA";
    public static string alias = "SOMBRA";

    // Contadores de la partida (los llena el GameManager).
    public static int rehenesRescatados;
    public static int rehenesTotales;
    public static int ciborgsDesactivados;
    public static int vialesUsados;
    public static int danos;

    // Los llena TerminarMision.
    public static float segundos;
    public static bool victoria;

    // true cuando Victoria / Derrota ya sumó fondos y reputación (evita sumarlos dos veces).
    public static bool aplicado;

    public static void Preparar(string id, string nombre, string escena, int pagoMision, int reputacion)
    {
        idMision = id;
        nombreMision = nombre;
        escenaMision = escena;
        pago = pagoMision;
        reputacionPorCompletar = reputacion;
        ReiniciarContadores();
    }

    public static void ReiniciarContadores()
    {
        rehenesRescatados = 0;
        rehenesTotales = 0;
        ciborgsDesactivados = 0;
        vialesUsados = 0;
        danos = 0;
        segundos = 0f;
        aplicado = false;
    }

    public static void TerminarMision(bool gano)
    {
        victoria = gano;
        segundos = Time.timeSinceLevelLoad;
        if (string.IsNullOrEmpty(escenaMision)) escenaMision = SceneManager.GetActiveScene().name;
        aplicado = false;
        IrA(gano ? ESCENA_VICTORIA : ESCENA_DERROTA);
    }

    public static void RepetirMision()
    {
        string escena = string.IsNullOrEmpty(escenaMision) ? SceneManager.GetActiveScene().name : escenaMision;
        ReiniciarContadores();
        IrA(escena);
    }

    public static void IrA(string escena)
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(escena);
    }
}
