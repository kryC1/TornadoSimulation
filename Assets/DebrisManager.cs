using UnityEngine;
using UnityEngine.UI;

public class DebrisManager : MonoBehaviour
{
    public Slider massSlider;

    void Start()
    {
        massSlider.onValueChanged.AddListener(UpdateDebrisMass);
    }

    void UpdateDebrisMass(float newMass)
    {
        Debris[] allDebris = FindObjectsOfType<Debris>();
        foreach (Debris d in allDebris)
        {
            Rigidbody rb = d.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = newMass;
            }
        }
    }
}
