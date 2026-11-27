using UnityEngine;
using UnityEngine.Events;

// Control de la pantalla de Créditos.
// ESC, el botón B del mando o un clic en [ESC] VOLVER disparan "alVolver"
// (el constructor lo conecta para volver al menú principal).
public class PanelCreditos : MonoBehaviour
{
    public UnityEvent alVolver = new UnityEvent();

    void Awake() => ClicUI.Conectar(transform, "AyudaVolver", Volver);

    public void Volver() => alVolver?.Invoke();

    void Update()
    {
        if (EntradaUI.Volver()) Volver();
    }
}
