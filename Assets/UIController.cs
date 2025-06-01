using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Tornado tornado;

    public Slider speedSlider;
    public Slider rotSpeedSlider;
    public Slider spinSlider;
    public Slider heightSlider;
    public Slider topRadiusSlider;

    void Start()
    {
        // Initialize sliders from tornado values
        speedSlider.value = tornado.tornadoSpeed;
        rotSpeedSlider.value = tornado.tornadoRotSpeed;
        spinSlider.value = tornado.tornadoSpinSpeed;
        heightSlider.value = tornado.tornadoHeight;
        topRadiusSlider.value = tornado.topRadius;

        // Add listeners
        speedSlider.onValueChanged.AddListener(UpdateSpeed);
        rotSpeedSlider.onValueChanged.AddListener(UpdateRotation);
        spinSlider.onValueChanged.AddListener(UpdateSpin);
        heightSlider.onValueChanged.AddListener(UpdateHeight);
        topRadiusSlider.onValueChanged.AddListener(UpdateRadius);
    }

    void UpdateSpeed(float value)
    {
        tornado.tornadoSpeed = value;
        UpdateForces();
    }

    void UpdateRotation(float value)
    {
        tornado.tornadoRotSpeed = value;
        UpdateForces();
    }

    void UpdateSpin(float value)
    {
        tornado.tornadoSpinSpeed = value;

        // Also update particle systems
        if (tornado.tornadoPSObj != null)
            tornado.tornadoPSObj.GetComponent<TornadoDust>().rotationSpeed = value;
        if (tornado.junkPSObj != null)
            tornado.junkPSObj.GetComponent<TornadoDust>().rotationSpeed = value;
        if (tornado.tornadoTopPSObj != null)
            tornado.tornadoTopPSObj.GetComponent<TornadoDustTop>().rotationSpeed = value;
        if (tornado.groundPSObj != null)
            tornado.groundPSObj.GetComponent<GroundDust>().rotationSpeed = value;

        UpdateForces();
    }

    void UpdateHeight(float value)
    {
        tornado.tornadoHeight = value;

        // Update dependent scripts
        if (tornado.tornadoPSObj != null)
            tornado.tornadoPSObj.GetComponent<TornadoDust>().tornadoHeight = value;
        if (tornado.junkPSObj != null)
            tornado.junkPSObj.GetComponent<TornadoDust>().tornadoHeight = value;
        if (tornado.tornadoTopPSObj != null)
            tornado.tornadoTopPSObj.GetComponent<TornadoDustTop>().tornadoHeight = value;

        UpdateForces();
    }

    void UpdateRadius(float value)
    {
        tornado.topRadius = value;

        if (tornado.tornadoPSObj != null)
            tornado.tornadoPSObj.GetComponent<TornadoDust>().topRadius = value;
        if (tornado.junkPSObj != null)
            tornado.junkPSObj.GetComponent<TornadoDust>().topRadius = value;
        if (tornado.tornadoTopPSObj != null)
        {
            var top = tornado.tornadoTopPSObj.GetComponent<TornadoDustTop>();
            top.topRadius = value;
            top.bottomRadius = value;
        }
        if (tornado.groundPSObj != null)
        {
            var ground = tornado.groundPSObj.GetComponent<GroundDust>();
            ground.bottomRadius = value;
        }

        UpdateForces();
    }

    void UpdateForces()
    {
        // Reference values
        float refSpeed = 40f;
        float refSpin = 200f;
        float refHeight = 200f;
        float refRadius = 30f;
        float refRotSpeed = 0.5f;

        float baseSuction = 400f;
        float baseLift = 1000f;
        float baseRotation = 1200f;

        // Normalized factors
        float speedFactor = tornado.tornadoSpeed / refSpeed;
        float spinFactor = tornado.tornadoSpinSpeed / refSpin;
        float heightFactor = tornado.tornadoHeight / refHeight;
        float radiusFactor = tornado.topRadius / refRadius;
        float rotSpeedFactor = tornado.tornadoRotSpeed / refRotSpeed;

        // Weighted force influences
        float suctionInfluence = (0.45f * speedFactor) + (0.25f * radiusFactor) + (0.2f * spinFactor) + (0.1f * rotSpeedFactor);
        float liftInfluence = (0.6f * heightFactor) + (0.4f * spinFactor);
        float rotationInfluence = (0.6f * spinFactor) + (0.25f * radiusFactor) + (0.15f * rotSpeedFactor);

        TornadoData.current.suctionForce = baseSuction * suctionInfluence;
        TornadoData.current.liftForce = baseLift * liftInfluence;
        TornadoData.current.rotationForce = baseRotation * rotationInfluence;
    }

}
