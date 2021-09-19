using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float brakeForce = 5000;
    public float maxSteerAngle = 35;

    [Header("Driving Assist Settings")]
    public bool antiRoll;
    public bool steerAssist;
    public bool stabilityControl;
    public float antiRollStrength = 5000;

    [Header("Gearbox Settings")]
    public float[] gear = { 3.2f, 1.8f, 1.3f, 0.9f, 0.6f }; //gears(index) and ratios(values)
    public float finalDrive = 4.0f;
    public float transEffic = 0.7f;

    [Header("Colliders and Transforms")]
    public WheelCollider frontPassengerW;
    public WheelCollider frontDriverW;
    public WheelCollider rearPassengerW;
    public WheelCollider rearDriverW;
    public Transform frontPassengerT;
    public Transform frontDriverT;
    public Transform rearPassengerT;
    public Transform rearDriverT;

    public Engine carEngine;
    private float steerLimit;
    private int currentGear = 0; 

    public float SteeringAngle { get; set; }
    public float BrakePedalPosition { get; set; }
    public float WheelPosition { get; set; }
    public float GasPedalPosition { get; set; }

    public void Start()
    {
        carEngine = GetComponentInParent<Engine>();
    }

    private void FixedUpdate()
    {
        SlipDetection();
        Steer();
        Brake();
        TransmissionControlUnit();
        Transmission();

        if (antiRoll)
        {
            AntiRoll();
        }
        if (stabilityControl)
        {
            StabilityControl();
        }
    }

    public void SlipDetection()
    {
        //todo
    }

    /***************
    ***DRIVETRAIN***
    ****************/

    //determines shift logic
    private void TransmissionControlUnit()
    {
        //determine shift logic WIP
        //will probably get scrapped because of integrating it into the RL model
        
        if (carEngine.Rpm >= carEngine.maxEngineRpm * 0.9f || carEngine.Rpm >= carEngine.maxPowerRpm * 1.3f)
        {
            ShiftUp();
        }
        else if (carEngine.Rpm < 1001) //super weak, needs changing
        {
            ShiftDown();
        }
        
    }

    //for shift-stick
    
    public void ShiftUp()
    {
        if (currentGear < gear.Length)
        {
            currentGear++;
        }
        else { Debug.Log("Can't shift up"); }
    }

    public void ShiftDown()
    {
        if (currentGear > 1)
        {
            currentGear--;
        }
        else
        { 
            //Debug.Log("Can't shift down");
        }
    }
    

    //transfers torque from engine to wheels
    private void Transmission()
    {
        //torque is distributed to each wheel
        float wheelTorque = 0.5f * (float) carEngine.Torque * gear[GetGear()] * finalDrive * transEffic;
        frontDriverW.motorTorque = wheelTorque;
        frontPassengerW.motorTorque = wheelTorque;
    }

    public void SetGear(int _gear)
    {
        if (_gear < gear.Length && _gear >= 0)
        {
            currentGear = _gear;
        }
        else
        {
            Debug.Log("No such gear");
        }
    }

    public int GetGear()
    {
        return currentGear;
    }


    private void Steer()
    {
        if (steerAssist)
        {
            SteerLimiter();
        }
        else
        {
            steerLimit = 1;
        }

        SteeringAngle = WheelPosition * maxSteerAngle * steerLimit;
        frontDriverW.steerAngle = SteeringAngle;
        frontPassengerW.steerAngle = SteeringAngle;
        UpdateWheelPoses();
    }

    private void Brake()
    {
        frontDriverW.brakeTorque = BrakePedalPosition * brakeForce;
        frontPassengerW.brakeTorque = BrakePedalPosition * brakeForce;
        rearDriverW.brakeTorque = BrakePedalPosition * brakeForce;
        rearPassengerW.brakeTorque = BrakePedalPosition * brakeForce;
    }

    public void HandBrake() //not an actual handbrake, use only to completely freeze the vehicle
    {
        frontDriverW.brakeTorque = Mathf.Infinity;
        frontPassengerW.brakeTorque = Mathf.Infinity;
        rearDriverW.brakeTorque = Mathf.Infinity;
        rearPassengerW.brakeTorque = Mathf.Infinity;
    }

    /******************
    ***DRIVE ASSISTS***
    *******************/

    private void SteerLimiter() //***needs work***
    {
        float highSpeed = 30;
        float speed = GetComponent<Rigidbody>().velocity.magnitude;
        steerLimit =  1 - speed / highSpeed;
    }

    private void StabilityControl() //todo 
    {
        //todo
    }

    private void AntiRoll()
    {
        WheelHit hit;
        float travelL = 1.0f;
        float travelR = 1.0f;

        //front wheels
        bool groundedL = frontPassengerW.GetGroundHit(out hit);
        if (groundedL)
            travelL = ( -frontPassengerW.transform.InverseTransformPoint(hit.point).y - frontPassengerW.radius ) / frontPassengerW.suspensionDistance;

        bool groundedR = frontDriverW.GetGroundHit(out hit);
        if (groundedR)
            travelR = ( -frontDriverW.transform.InverseTransformPoint(hit.point).y - frontDriverW.radius ) / frontDriverW.suspensionDistance;

        float antiRollForce = ( travelL - travelR ) * antiRollStrength;

        if (groundedL)
            GetComponent<Rigidbody>().AddForceAtPosition(frontPassengerW.transform.up * -antiRollForce,
                   frontPassengerW.transform.position);
        if (groundedR)
            GetComponent<Rigidbody>().AddForceAtPosition(frontDriverW.transform.up * antiRollForce,
                   frontDriverW.transform.position);

        //rear wheels
        travelL = 1.0f;
        travelR = 1.0f;

        groundedL = rearPassengerW.GetGroundHit(out hit);
        if (groundedL)
            travelL = ( -rearPassengerW.transform.InverseTransformPoint(hit.point).y - rearPassengerW.radius ) / rearPassengerW.suspensionDistance;

        groundedR = rearDriverW.GetGroundHit(out hit);
        if (groundedR)
            travelR = ( -rearDriverW.transform.InverseTransformPoint(hit.point).y - rearDriverW.radius ) / rearDriverW.suspensionDistance;

        antiRollForce = ( travelL - travelR ) * antiRollStrength;

        if (groundedL)
            GetComponent<Rigidbody>().AddForceAtPosition(rearPassengerW.transform.up * -antiRollForce,
                   rearPassengerW.transform.position);
        if (groundedR)
            GetComponent<Rigidbody>().AddForceAtPosition(rearDriverW.transform.up * antiRollForce,
                   rearDriverW.transform.position);
    }

    /****************
     ******MISC******
     ****************/
    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontDriverW, frontDriverT);
        UpdateWheelPose(frontPassengerW, frontPassengerT);
        UpdateWheelPose(rearDriverW, rearDriverT);
        UpdateWheelPose(rearPassengerW, rearPassengerT);
    }

    private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
    {
        Vector3 _pos = transform.position;
        Quaternion _quat = transform.rotation;

        _collider.GetWorldPose(out _pos, out _quat);

        _transform.position = _pos;
        _transform.rotation = _quat;
    }

}
