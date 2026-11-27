using UnityEngine;

// Aplica los ajustes de pantalla: modo de pantalla y resolución.
// El volumen NO va aquí: los Sliders de volumen se conectan al Audio Mixer desde su
// On Value Changed. Subtítulos e idioma se conectan desde "alCambiarValor" de su fila.
// Al iniciar el juego, aplica lo que el jugador dejó guardado la vez anterior.
public class AjustesPantalla : MonoBehaviour
{
    static readonly Vector2Int[] Resoluciones =
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(1600, 900),
        new Vector2Int(1280, 720),
    };

    static readonly FullScreenMode[] Modos =
    {
        FullScreenMode.ExclusiveFullScreen, // PANTALLA COMPLETA
        FullScreenMode.FullScreenWindow,    // SIN BORDES
        FullScreenMode.Windowed,            // VENTANA
    };

    // Fila "MODO DE PANTALLA" (0 completa, 1 sin bordes, 2 ventana)
    public void ModoPantalla(float i)
    {
        Screen.fullScreenMode = Modos[Mathf.Clamp((int)i, 0, Modos.Length - 1)];
    }

    // Fila "RESOLUCIÓN" (0 1920x1080, 1 1600x900, 2 1280x720)
    public void Resolucion(float i)
    {
        Vector2Int r = Resoluciones[Mathf.Clamp((int)i, 0, Resoluciones.Length - 1)];
        Screen.SetResolution(r.x, r.y, Screen.fullScreenMode);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AplicarGuardados()
    {
        if (Application.isEditor) return; // en el Editor no se cambia la ventana
        if (!PlayerPrefs.HasKey("heroe.modo_pantalla") && !PlayerPrefs.HasKey("heroe.resolucion")) return;
        FullScreenMode modo = Modos[Mathf.Clamp((int)PlayerPrefs.GetFloat("heroe.modo_pantalla", 0f), 0, Modos.Length - 1)];
        Vector2Int r = Resoluciones[Mathf.Clamp((int)PlayerPrefs.GetFloat("heroe.resolucion", 0f), 0, Resoluciones.Length - 1)];
        Screen.SetResolution(r.x, r.y, modo);
    }
}
