using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Hace clicable con el mouse una tecla dibujada o una ayuda de la barra inferior
// (por ejemplo [ESC] VOLVER o [Q] / [E] de las pestañas). Los paneles lo agregan solos al
// iniciar, buscando esas piezas por su nombre; no hace falta ponerlo a mano.
// Si la pieza ya tiene un Button (enlazado en el Inspector), no hace nada: manda el Button.
public class ClicUI : MonoBehaviour, IPointerClickHandler
{
    public System.Action accion;

    public static void Conectar(Transform raiz, string nombre, System.Action accion)
    {
        Transform t = Buscar(raiz, nombre);
        if (t == null || t.GetComponent<Button>() != null) return;
        foreach (Graphic g in t.GetComponentsInChildren<Graphic>(true)) g.raycastTarget = true;
        ClicUI c = t.GetComponent<ClicUI>();
        if (c == null) c = t.gameObject.AddComponent<ClicUI>();
        c.accion = accion;
    }

    static Transform Buscar(Transform t, string nombre)
    {
        if (t.name == nombre) return t;
        foreach (Transform h in t)
        {
            Transform r = Buscar(h, nombre);
            if (r != null) return r;
        }
        return null;
    }

    public void OnPointerClick(PointerEventData datos)
    {
        if (datos.button == PointerEventData.InputButton.Left) accion?.Invoke();
    }
}
