using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForceDebugUI : MonoBehaviour
{
    public TMP_Text suctionText;
    public TMP_Text liftText;
    public TMP_Text rotationText;
    public TMP_Text massText;

    public TornadoData tornadoData;
    public DebrisManager debrisManager;

    void Update()
    {
        if (tornadoData != null)
        {
            suctionText.text = $"Suction Force: {tornadoData.suctionForce:F1}";
            liftText.text = $"Lift Force: {tornadoData.liftForce:F1}";
            rotationText.text = $"Rotation Force: {tornadoData.rotationForce:F1}";
        }

        if (debrisManager != null && debrisManager.massSlider != null)
        {
            massText.text = $"Debris Mass: {debrisManager.massSlider.value:F1}";
        }
    }
}
