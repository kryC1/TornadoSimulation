using UnityEngine;
using System.Collections;

public class Tornado : MonoBehaviour
{
    public GameObject skeletonCylinder;
    public GameObject tornadoPSObj;
    public GameObject groundPSObj;
    public GameObject tornadoTopPSObj;
    public GameObject junkPSObj;
    public GameObject chaseObj;
    public GameObject chaseRotateObj;
    public GameObject targetObj;
    public GameObject debugSphere;

    public bool isDebug;

    [Header("Tornado data")]
    public AnimationCurve tornadoShape;
    public float tornadoSpeed = 10f;
    public float tornadoRotSpeed = 1f;
    public float tornadoSpinSpeed = 1f;
    public float tornadoHeight = 50f;
    public float topRadius = 20f;

    public int pieces = 10;

    public float m;
    public float k;
    public float c;
    public float aboveFactor = 1f; // 0.6
    public float belowFactor = 1f;
    public float chaseFactor = 1f;

    [System.NonSerialized]
    public Transform[] skeletonPiecesArray;

    private Vector3[] posNew;
    private Vector3[] posOld;
    private Vector3[] velArray;

    Vector3 oldChasePos;


    void Start()
    {
        BuildTornado();

        posNew = new Vector3[pieces];
        posOld = new Vector3[pieces];
        velArray = new Vector3[pieces];

        for (int i = 0; i < pieces; i++)
        {
            posNew[i] = Vector3.zero;
            posOld[i] = skeletonPiecesArray[i].position;
            velArray[i] = Vector3.zero;
        }

        chaseObj.transform.position = transform.position;

        TornadoDust dustScript = tornadoPSObj.GetComponent<TornadoDust>();

        dustScript.skeletonPiecesArray = skeletonPiecesArray;
        dustScript.tornadoShape = tornadoShape;
        dustScript.tornadoHeight = tornadoHeight;
        dustScript.topRadius = topRadius;
        dustScript.tornadoTrans = transform;
        dustScript.rotationSpeed = tornadoSpinSpeed;


        TornadoDust junkScript = junkPSObj.GetComponent<TornadoDust>();

        junkScript.skeletonPiecesArray = skeletonPiecesArray;
        junkScript.tornadoShape = tornadoShape;
        junkScript.tornadoHeight = tornadoHeight;
        junkScript.topRadius = topRadius;
        junkScript.tornadoTrans = transform;
        junkScript.rotationSpeed = tornadoSpinSpeed;


        TornadoDustTop tornadoDustTopScript = tornadoTopPSObj.GetComponent<TornadoDustTop>();

        tornadoDustTopScript.bottomRadius = topRadius;
        tornadoDustTopScript.rotationSpeed = tornadoSpinSpeed;
        tornadoDustTopScript.tornadoHeight = tornadoHeight;
    }



    void Update()
    {
        groundPSObj.GetComponent<GroundDust>().rotationSpeed = tornadoSpinSpeed;

        tornadoPSObj.GetComponent<TornadoDust>().rotationSpeed = tornadoSpinSpeed;

        MoveTornado();
    }



    void FixedUpdate()
    {
        TornadoDynamics();
    }



    void BuildTornado()
    {
        AddSkeletonPieces();
    }



    void AddSkeletonPieces()
    {
        skeletonPiecesArray = new Transform[pieces];

        float pieceHeight = tornadoHeight / pieces;
        float currentY = 0f;

        for (int i = 0; i < pieces; i++)
        {
            GameObject newPiece = Instantiate(skeletonCylinder) as GameObject;

            Vector3 newScale = skeletonCylinder.transform.localScale;

            newScale.z = pieceHeight / 2f;

            newPiece.transform.localScale = newScale;

            Vector3 position = transform.position;

            position.y = currentY;

            newPiece.transform.position = position;
            newPiece.transform.parent = transform;

            skeletonPiecesArray[i] = newPiece.transform;

            currentY += pieceHeight;
        }
    }



    void MoveTornado()
    {
        Vector3 targetDir = targetObj.transform.position - chaseObj.transform.position;
        Vector3 newDir = Vector3.RotateTowards(chaseObj.transform.forward, targetDir, tornadoRotSpeed * Time.deltaTime, 0.0F);

        chaseObj.transform.rotation = Quaternion.LookRotation(newDir);
        chaseObj.transform.Translate(Vector3.forward * tornadoSpeed * Time.deltaTime);

        float rotationSpeed = 40f;
        float radius = 300f;

        Vector3 chasePos = chaseObj.transform.position;
        Vector3 rotatePos = chaseRotateObj.transform.position;

        chaseRotateObj.transform.position += (chasePos - oldChasePos);

        float currentRadius = (rotatePos - chasePos).magnitude;

        Vector3 dir = (rotatePos - chasePos).normalized;

        chaseRotateObj.transform.position += (radius - currentRadius) * dir;
        chaseRotateObj.transform.RotateAround(chasePos, transform.up, rotationSpeed * Time.deltaTime);

        oldChasePos = chasePos;
    }



    void TornadoDynamics()
    {
        Vector3 chasePos = chaseObj.transform.position;
        Vector3 rotatePos = chaseRotateObj.transform.position;

        chasePos.y = 0f;

        float h = 0.02f;

        for (int i = 0; i < pieces; i++)
        {
            Vector3 oldPosVec = posOld[i];
            Vector3 accVec = Vector3.zero;

            if (i == 0)
            {
                accVec = (-k * (oldPosVec - posOld[i + 1]) * aboveFactor - k * (oldPosVec - rotatePos) * chaseFactor) / m;
            }
            else if (i == pieces - 1)
            {
                accVec = (-k * (oldPosVec - chasePos) + k * (posOld[i - 1] - oldPosVec) * belowFactor) / m;
            }
            else
            {
                accVec = (-k * (oldPosVec - posOld[i - 1]) * belowFactor + k * (posOld[i + 1] - oldPosVec) * aboveFactor) / m;
            }

            accVec -= (c * velArray[i]) / m;

            posNew[i] = oldPosVec + h * velArray[i];
            posNew[i].y = posOld[i].y;

            velArray[i] = velArray[i] + h * accVec;
        }


        for (int i = 0; i < pieces; i++)
        {
            Vector3 newPos = posNew[i];

            newPos.y = skeletonPiecesArray[i].position.y;

            skeletonPiecesArray[i].position = newPos;

            posOld[i] = posNew[i];
        }


        for (int i = 0; i < pieces; i++)
        {
            if (i > 0)
            {
                skeletonPiecesArray[i].LookAt(skeletonPiecesArray[i - 1].position);
            }
            else
            {
                skeletonPiecesArray[i].LookAt(skeletonPiecesArray[i].position + -Vector3.up);
            }
        }


        Vector3 groundPos = posNew[0];

        groundPos.y = 0f;

        groundPSObj.transform.position = groundPos;

        Vector3 topPos = posNew[posNew.Length - 1];

        topPos.y = tornadoHeight;

        tornadoTopPSObj.transform.position = topPos;
    }

}
