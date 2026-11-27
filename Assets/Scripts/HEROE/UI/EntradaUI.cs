using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Teclas y botones de mando que usan los paneles de la interfaz
// (lo que el EventSystem no cubre: volver, pausar, cambiar de pestaña, etc.).
// Arriba/abajo, izquierda/derecha y confirmar sobre botones los maneja el EventSystem.
public static class EntradaUI
{
    // ESC  /  botón B (Xbox) o círculo (PlayStation)
    public static bool Volver()
    {
#if ENABLE_INPUT_SYSTEM
        return (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            || (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }

    // ESC  /  botón Start (Menu en Xbox, Options en PlayStation)
    public static bool Pausa()
    {
#if ENABLE_INPUT_SYSTEM
        return (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            || (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }

    // ENTER  /  botón A (Xbox) o cruz (PlayStation).
    // Solo para pantallas sin botones (Victoria y Derrota); en los menús lo maneja el EventSystem.
    public static bool Confirmar()
    {
#if ENABLE_INPUT_SYSTEM
        return (Keyboard.current != null && (Keyboard.current.enterKey.wasPressedThisFrame
                                          || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
            || (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
#endif
    }

    // R  /  botón X (Xbox) o cuadrado (PlayStation)
    public static bool Repetir()
    {
#if ENABLE_INPUT_SYSTEM
        return (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            || (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.R);
#endif
    }

    // Q  /  LB
    public static bool PestanaAnterior()
    {
#if ENABLE_INPUT_SYSTEM
        return (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
            || (Gamepad.current != null && Gamepad.current.leftShoulder.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.Q);
#endif
    }

    // E  /  RB
    public static bool PestanaSiguiente()
    {
#if ENABLE_INPUT_SYSTEM
        return (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            || (Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.E);
#endif
    }

    // F  /  botón Y (Xbox) o triángulo (PlayStation)
    public static bool Restablecer()
    {
#if ENABLE_INPUT_SYSTEM
        return (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            || (Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.F);
#endif
    }
}
