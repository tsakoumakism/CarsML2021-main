using UnityEngine;

public class Engine : MonoBehaviour
{
    //engine torques for rpm values in 500rpm increments
    [Header("Engine Stats")]//rpm: 0   500  1000  1500   2000  2500  3000  3500  4000  4500 5000   5500  6000 6500  7000
    //public float[] torqueTable = { 0f, 100f, 130f, 160f, 190f, 240f, 280f, 320f, 350f, 330f, 280f, 250f, 230f, 200f, 180f };
    public float maxEngineHorsePower = 250;
    public float maxEngineRpm = 6000;
    public float maxPowerRpm = 3500;

    //public float GasPedalPosition { get; set; } //might rework this
    private float gasPedalPosition;

    //engine rpm internal parameters
    //private float rpmDecayRate = 90;
    //private float maxRpmDelta = 100;
    public float idleRpm = 1000;

    public float Rpm { get; protected set; } //current rpm

    //engine power internal parameters
    private float maxEngineTorque;
    public float Torque { get; protected set; } //current torque

    private CarController carController;

    //todo: private readonly float[,,] powerCurves;

    //torque calculation variables
    // Engine Torque Formula
    // Te = Pe / We = P1 + P2*We + P3*We^2
    // Te = Torque Engine
    // Pe = Power Engine
    // We = Angular Velocity Engine
    // P1..P3 = Constants
    private float p1, p2, p3;
    private float p1Ratio = 1, p2Ratio = 1, p3Ratio = 1; //generally 1 for gasoline engines
    private float mHpWattConvertingConstant = 745.699872f;
    private float engineMaxPower;
    private float engineMaxPowerAngularVelocity;

    private void Start()
    {
        carController = GetComponentInParent<CarController>();

        Rpm = idleRpm;
        //todo: initalize gearbox

        //determine engine power equation parameters
        engineMaxPower = maxEngineHorsePower * mHpWattConvertingConstant;
        engineMaxPowerAngularVelocity = maxPowerRpm * (Mathf.PI / 30);
        p1 = p1Ratio * (engineMaxPower / engineMaxPowerAngularVelocity);
        p2 = p2Ratio * (engineMaxPower / Mathf.Pow(engineMaxPowerAngularVelocity, 2));
        p3 = -p3Ratio * (engineMaxPower / Mathf.Pow(engineMaxPowerAngularVelocity, 3));
    }

    private void FixedUpdate()
    {
        gasPedalPosition = carController.GasPedalPosition;

        //determine engine RPM
        //if (gasPedalPosition == 0) //rpm decay if no gas pressed
        //{
        //    Rpm = Mathf.MoveTowards(Rpm, idleRpm, rpmDecayRate);
        //}
        //else //rpm increase when stepping on gas
        //{
        //    Rpm = Mathf.MoveTowards(Rpm, gasPedalPosition * maxEngineRpm, gasPedalPosition * maxRpmDelta);
        //}

        //***need to add power curves***

        maxEngineTorque = GetMaxEngineTorque();
        if (maxEngineTorque < 0)
        {
            maxEngineTorque = 0;
        }

        Torque = maxEngineTorque * gasPedalPosition;
        Rpm = carController.frontDriverW.rpm * carController.gear[carController.GetGear()] * carController.finalDrive;
        if (Rpm < idleRpm)
        {
            Rpm = idleRpm;
        }
    }

    public float GetMaxEngineTorque()
    {
        return p1 + (p2 * Rpm * (Mathf.PI / 30)) + p3 * Mathf.Pow(Rpm * (Mathf.PI / 30), 2);
    }
}