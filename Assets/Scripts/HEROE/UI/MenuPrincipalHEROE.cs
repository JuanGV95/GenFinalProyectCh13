using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

// H-EROE — Control del menú principal.
// - Al abrirse, selecciona la primera opción (teclado y mando funcionan de inmediato).
// - Si se hace clic en un espacio vacío, recupera la última opción seleccionada,
//   para que el teclado o el mando nunca se queden "sin foco".
// - Cambia el texto de descripción según la opción activa.
// No depende de GameManager, UIManager ni AudioManager: las acciones de cada opción
// se conectan en el On Click () de su Button.
public class MenuPrincipalHEROE : MonoBehaviour
{
    [SerializeField] OpcionMenuHEROE[] opciones;
    [SerializeField] TextMeshProUGUI descripcion;

    [Tooltip("Se dispara cada vez que cambia la opción activa. Útil para conectar un sonido.")]
    public UnityEvent alCambiarOpcion;

    GameObject ultima;

    public void Configurar(OpcionMenuHEROE[] ops, TextMeshProUGUI desc)
    {
        opciones = ops;
        descripcion = desc;
    }

    void OnEnable()
    {
        StartCoroutine(SeleccionarPrimera());
    }

    IEnumerator SeleccionarPrimera()
    {
        yield return null; // esperar a que el EventSystem esté listo
        if (opciones == null || opciones.Length == 0 || EventSystem.current == null) yield break;
        GameObject primera = ultima != null ? ultima : opciones[0].gameObject;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(primera);
    }

    void Update()
    {
        EventSystem es = EventSystem.current;
        if (es == null) return;
        if (es.currentSelectedGameObject == null && ultima != null && ultima.activeInHierarchy)
            es.SetSelectedGameObject(ultima);
    }

    public void Notificar(OpcionMenuHEROE opcion)
    {
        bool cambio = ultima != opcion.gameObject;
        ultima = opcion.gameObject;
        if (descripcion != null) descripcion.text = opcion.Descripcion;
        if (cambio) alCambiarOpcion?.Invoke();
    }
}
