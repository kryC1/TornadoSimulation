using UnityEngine;
using System.Collections;


public class TornadoDustTop : MonoBehaviour 
{
    public GameObject groundPSObj;
    public GameObject debugCylinder;

    public float rotationSpeed = 10f;
    public float bottomRadius = 15f;
    public float topRadius = 50f;
    public float height = 40f;
    public float tornadoHeight;

    public AnimationCurve dustShape;

    ParticleSystem groundPS;
    ParticleSystem.Particle[] m_Particles;

    Vector3 oldCenterPos;



    void Start()
    {
        groundPS = groundPSObj.GetComponent<ParticleSystem>();

        m_Particles = new ParticleSystem.Particle[groundPS.maxParticles];

        oldCenterPos = transform.position;
    }



    void LateUpdate()
    {
        //Change the shape of the ps
        var x = groundPS.shape;
        x.radius = bottomRadius;

        debugCylinder.transform.localScale = new Vector3(bottomRadius * 2f, 1f, bottomRadius * 2f);

        RotateParticles();

        oldCenterPos = transform.position;
    }



    void RotateParticles()
    {
        int numParticlesAlive = groundPS.GetParticles(m_Particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector3 particlePos = m_Particles[i].position;

            float particleY = particlePos.y;

            if (particleY > height + tornadoHeight + 20f)
            {
                m_Particles[i].remainingLifetime = -1f;

                continue;
            }

            if (particleY > height + tornadoHeight)
            {
                Color32 currentColor = m_Particles[i].startColor;

                currentColor.a = 150;

                m_Particles[i].startColor = currentColor;
            }


            Vector3 centerPos = transform.position;
            Vector3 posChange = centerPos - oldCenterPos;

            particlePos += posChange;



            float wantedRadius = (dustShape.Evaluate((particleY - tornadoHeight) / height) * (topRadius - bottomRadius)) + bottomRadius;

            particlePos = TornadoMath.CheckRadius(particlePos, centerPos, wantedRadius);

            Vector3 anglePos = particlePos - centerPos;

            float currentAngle = TornadoMath.GetAngle(anglePos.x, anglePos.z);

            float wantedSpeed = (1f - dustShape.Evaluate((particleY - tornadoHeight) / height)) * rotationSpeed;


            particlePos = TornadoMath.GetParticlePos(centerPos, currentAngle, wantedRadius, wantedSpeed);
            particlePos.y = particleY;

            m_Particles[i].position = particlePos;
            m_Particles[i].startSize = (150f * dustShape.Evaluate((particleY - tornadoHeight) / height)) + 45f;
        }

        groundPS.SetParticles(m_Particles, numParticlesAlive);
    }
}
