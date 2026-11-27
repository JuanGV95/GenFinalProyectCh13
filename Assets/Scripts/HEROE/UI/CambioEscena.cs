using UnityEngine;

// Para conectar desde el On Click () de un botón: carga una escena por su nombre.
// Ejemplo: JUGAR del menú principal → Cargar("SeleccionMision").
public class CambioEscena : MonoBehaviour
{
    public void Cargar(string escena) => ResultadoMision.IrA(escena);
}
