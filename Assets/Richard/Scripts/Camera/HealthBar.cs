using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public static HealthBar instance;
    void Awake()
    {
        instance = this;
    }
    public void HealthBarFill(float maxHealth, float currentHealth)
    {
        slider.value = currentHealth/maxHealth;
    }
}
