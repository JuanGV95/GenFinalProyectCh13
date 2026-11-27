using UnityEngine;

// Progreso del jugador guardado entre partidas (PlayerPrefs):
// fondos, reputación, viales de Gen H y, por misión, la mejor calificación y las corridas.
// Lo leen la barra superior y la Selección de misión; lo actualiza la pantalla de Victoria / Derrota.
public static class Progreso
{
    const string FONDOS = "HEROE.Fondos";
    const string REPUTACION = "HEROE.Reputacion";
    const string VIALES = "HEROE.Viales";
    const string ORDEN = "SABCD"; // de mejor a peor

    public const int FondosIniciales = 1240;
    public const int VialesMaximos = 3;

    public static int Fondos
    {
        get => PlayerPrefs.GetInt(FONDOS, FondosIniciales);
        set { PlayerPrefs.SetInt(FONDOS, Mathf.Max(0, value)); PlayerPrefs.Save(); }
    }

    public static int Reputacion
    {
        get => PlayerPrefs.GetInt(REPUTACION, 0);
        set { PlayerPrefs.SetInt(REPUTACION, Mathf.Max(0, value)); PlayerPrefs.Save(); }
    }

    public static int Viales
    {
        get => PlayerPrefs.GetInt(VIALES, VialesMaximos);
        set { PlayerPrefs.SetInt(VIALES, Mathf.Clamp(value, 0, VialesMaximos)); PlayerPrefs.Save(); }
    }

    public static int Corridas(string idMision) => PlayerPrefs.GetInt("HEROE.Corridas." + idMision, 0);

    // "-" si nunca se ha completado.
    public static string MejorCalificacion(string idMision) => PlayerPrefs.GetString("HEROE.Mejor." + idMision, "-");

    public static void RegistrarCorrida(string idMision, string calificacion)
    {
        PlayerPrefs.SetInt("HEROE.Corridas." + idMision, Corridas(idMision) + 1);
        string mejor = MejorCalificacion(idMision);
        int actual = ORDEN.IndexOf(mejor);
        int nueva = ORDEN.IndexOf(calificacion);
        if (nueva >= 0 && (actual < 0 || nueva < actual))
            PlayerPrefs.SetString("HEROE.Mejor." + idMision, calificacion);
        PlayerPrefs.Save();
    }

    // Para pruebas: vuelve a los valores iniciales (no borra las corridas por misión).
    public static void Reiniciar()
    {
        PlayerPrefs.DeleteKey(FONDOS);
        PlayerPrefs.DeleteKey(REPUTACION);
        PlayerPrefs.DeleteKey(VIALES);
        PlayerPrefs.Save();
    }
}
