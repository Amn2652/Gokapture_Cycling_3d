using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BicycleVehicle : MonoBehaviour
{
    float horizontalInput;
    bool keyInput;  // New variable to check key press for running

    public Transform handle;
    Rigidbody rb;

    public Vector3 COG;

    [SerializeField] float motorforce;

    float steeringAngle;
    [SerializeField] float currentSteeringAngle;
    [Range(0f, 0.1f)][SerializeField] float speedteercontrolTime;
    [SerializeField] float maxSteeringAngle;
    [Range(0.000001f, 1)][SerializeField] float turnSmoothing;

    [SerializeField] float maxlayingAngle = 45f;
    public float targetlayingAngle;
    [Range(-40, 40)] public float layingammount;
    [Range(0.000001f, 1)][SerializeField] float leanSmoothing;

    [SerializeField] WheelCollider frontWheel;
    [SerializeField] WheelCollider backWheel;

    [SerializeField] Transform frontWheeltransform;
    [SerializeField] Transform backWheeltransform;

    [SerializeField] TrailRenderer fronttrail;
    [SerializeField] TrailRenderer rearttrail;

    public bool frontGrounded;
    public bool rearGrounded;

    // New variables for player-specific input settings
    [SerializeField] private KeyCode moveKey = KeyCode.W;  // Key for forward movement
    [SerializeField] private KeyCode leftKey = KeyCode.A;  // Key for steering left
    [SerializeField] private KeyCode rightKey = KeyCode.D; // Key for steering right

    void Start()
    {
        StopEmitTrail();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        GetInput();
        HandleEngine();
        HandleSteering();
        UpdateWheels();
        UpdateHandle();
        LayOnTurn();
        DownPresureOnSpeed();
        EmitTrail();
    }

    public void GetInput()
    {
        // Check for movement and steering input based on the specified keys
        keyInput = Input.GetKey(moveKey);
        horizontalInput = 0;

        if (Input.GetKey(leftKey))
        {
            horizontalInput = -1;
        }
        else if (Input.GetKey(rightKey))
        {
            horizontalInput = 1;
        }
    }

    public void HandleEngine()
    {
        if (keyInput)  // Apply motor force only when the specific key is pressed
        {
            backWheel.motorTorque = motorforce;
        }
        else
        {
            backWheel.motorTorque = 0;  // Stop moving when the key is released
        }
    }

    public void DownPresureOnSpeed()
    {
        Vector3 downforce = Vector3.down;
        float downpressure;
        if (rb.velocity.magnitude > 5)
        {
            downpressure = rb.velocity.magnitude;
            rb.AddForce(downforce * downpressure, ForceMode.Force);
        }
    }

    public void SpeedSteerinReductor()
    {
        if (rb.velocity.magnitude < 5)
        {
            maxSteeringAngle = Mathf.LerpAngle(maxSteeringAngle, 50, speedteercontrolTime);
        }
        else if (rb.velocity.magnitude > 5 && rb.velocity.magnitude < 10)
        {
            maxSteeringAngle = Mathf.LerpAngle(maxSteeringAngle, 30, speedteercontrolTime);
        }
        else if (rb.velocity.magnitude > 10 && rb.velocity.magnitude < 15)
        {
            maxSteeringAngle = Mathf.LerpAngle(maxSteeringAngle, 15, speedteercontrolTime);
        }
        else if (rb.velocity.magnitude > 15 && rb.velocity.magnitude < 20)
        {
            maxSteeringAngle = Mathf.LerpAngle(maxSteeringAngle, 10, speedteercontrolTime);
        }
        else if (rb.velocity.magnitude > 20)
        {
            maxSteeringAngle = Mathf.LerpAngle(maxSteeringAngle, 5, speedteercontrolTime);
        }
    }

    public void HandleSteering()
    {
        SpeedSteerinReductor();
        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, maxSteeringAngle * horizontalInput, turnSmoothing);
        frontWheel.steerAngle = currentSteeringAngle;

        targetlayingAngle = maxlayingAngle * -horizontalInput;
    }

    private void LayOnTurn()
    {
        Vector3 currentRot = transform.rotation.eulerAngles;

        if (rb.velocity.magnitude < 1)
        {
            layingammount = Mathf.LerpAngle(layingammount, 0f, 0.05f);
            transform.rotation = Quaternion.Euler(currentRot.x, currentRot.y, layingammount);
            return;
        }

        if (currentSteeringAngle < 0.5f && currentSteeringAngle > -0.5)
        {
            layingammount = Mathf.LerpAngle(layingammount, 0f, leanSmoothing);
        }
        else
        {
            layingammount = Mathf.LerpAngle(layingammount, targetlayingAngle, leanSmoothing);
            rb.centerOfMass = new Vector3(rb.centerOfMass.x, COG.y, rb.centerOfMass.z);
        }

        transform.rotation = Quaternion.Euler(currentRot.x, currentRot.y, layingammount);
    }

    public void UpdateWheels()
    {
        UpdateSingleWheel(frontWheel, frontWheeltransform);
        UpdateSingleWheel(backWheel, backWheeltransform);
    }

    public void UpdateHandle()
    {
        Quaternion sethandleRot;
        sethandleRot = frontWheeltransform.rotation;
        handle.localRotation = Quaternion.Euler(handle.localRotation.eulerAngles.x, currentSteeringAngle, handle.localRotation.eulerAngles.z);
    }

    private void EmitTrail()
    {
        frontGrounded = frontWheel.GetGroundHit(out WheelHit Fhit);
        rearGrounded = backWheel.GetGroundHit(out WheelHit Rhit);

        fronttrail.emitting = frontGrounded;
        rearttrail.emitting = rearGrounded;
    }

    private void StopEmitTrail()
    {
        fronttrail.emitting = false;
        rearttrail.emitting = false;
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
