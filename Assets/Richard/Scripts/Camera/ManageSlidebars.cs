using UnityEngine;

public class ManageSlidebars : MonoBehaviour
{
    private Camera camera;
    void Awake()
    {
        camera = Camera.main;
    }
    void LateUpdate()
    {
        transform.rotation = camera.transform.rotation;
    }

}
