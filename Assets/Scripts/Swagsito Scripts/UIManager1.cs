using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UiManager : MonoBehaviour
{
    [SerializeField] GameObject UICanvas;

    [SerializeField] List<BaseUIPanel> screens = new List<BaseUIPanel>();

    [SerializeField] List<BaseUIPanel> overlays = new List<BaseUIPanel>();

    BaseUIPanel currenScreen;

    private readonly List<BaseUIPanel> activeOverlays = new List<BaseUIPanel>();

    bool isTransitioning;

    public void ShowScreen(string screenName)
    {
        BaseUIPanel panel = screens.Find(i => i.PanelName == screenName);

        if (panel == null)
        {
            /*Debug.LogError("No se encontro " + screenName);*/
            return;
        }

        if (panel == currenScreen)
        {
            /*Debug.Log("La pantalla" + screenName + "ya esta activa");*/
            return;
        }

        if (!isTransitioning)
        {
            StartCoroutine(ShowScreenCoroutine(panel));
        }
    }

    public IEnumerator ShowScreenCoroutine(BaseUIPanel nextScreen)
    {
        isTransitioning = true;

        if (currenScreen != null)
        {
            yield return currenScreen.HideCoroutine();
            currenScreen.gameObject.SetActive(false);
        }

        currenScreen = nextScreen;
        currenScreen.gameObject.SetActive(true);
        currenScreen.Show();

        yield return currenScreen.ShowCoroutine(false);
        isTransitioning = false;

    }

    public void Hide()
    {
        if (isTransitioning) return;
        StartCoroutine(HideScreenCoroutine());
    }

    public IEnumerator HideScreenCoroutine()
    {
        if (currenScreen == null) yield break;
        isTransitioning = true;

        BaseUIPanel hideScreen = currenScreen;
        currenScreen = null;

        yield return hideScreen.HideCoroutine();
        hideScreen.gameObject.SetActive(false);
        isTransitioning = false;
    }

    public void ShowOverlay(string overlayName)
    {
        BaseUIPanel overlay = overlays.Find(i => i != null && i.PanelName == overlayName);

        if (overlay == null)
        {
            Debug.LogError($"No se encontró el overlay '{overlayName}'. " + "Comprueba la lista Overlays y el nombre del panel.",this);
            return;
        }

        if (activeOverlays.Contains(overlay)) return;

        activeOverlays.Add(overlay);

        overlay.gameObject.SetActive(true);
        overlay.Show();

        StartCoroutine(overlay.ShowCoroutine(false));
    }

    public void HideOverlay(string overlayName)
    {
        BaseUIPanel overlay = activeOverlays.Find(i => i.PanelName == overlayName);

        if (overlay == null)
        {
            Debug.LogError("No se encontro " + overlayName); return;
        }

        if (overlay == currenScreen)
        {
            Debug.Log("La overlay" + overlayName + "ya esta activa"); return;
        }

        if (overlay != null)
        {
            StartCoroutine(CloseOverlayCoroutine(overlay));
        }
    }

    public IEnumerator CloseOverlayCoroutine(BaseUIPanel overlay)
    {
        activeOverlays.Remove(overlay);
        yield return overlay.HideCoroutine();

        overlay.gameObject.SetActive(false);
    }

    public void CloseAllOverlays()
    {
        foreach (BaseUIPanel overlay in activeOverlays.ToArray())
        {
            if (overlay)
            {
                StartCoroutine(CloseOverlayCoroutine(overlay));
            }
        }
    }


}