using UnityEngine;
using TMPro;

// Copia el texto de otro TextMeshPro (por ejemplo, la sombra de la letra de calificación).
// Así el código solo escribe un texto y la sombra lo sigue sola, también en el Editor.
[ExecuteAlways]
public class CopiaTexto : MonoBehaviour
{
    public TMP_Text origen;
    TMP_Text propio;

    void LateUpdate()
    {
        if (origen == null) return;
        if (propio == null) propio = GetComponent<TMP_Text>();
        if (propio != null && propio.text != origen.text) propio.text = origen.text;
    }
}
