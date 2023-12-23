using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetSki : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Buoyancy buoyancy;
    [SerializeField] private Transform _transform;
    [SerializeField] private Transform propeller;

    [SerializeField] private float engine_rpm = 0f;
    [SerializeField] private float propellers_constant = 0.6f;
    [SerializeField] private float engine_max_rpm = 600.0f;
    [SerializeField] private float acceleration_cst = 1.0f;
    [SerializeField] private float drag = 0.01f;
    [SerializeField] private float max_reverse_speed = 4.0f;
    [SerializeField] private float max_speed = 50.0f;
    
    [SerializeField] private Vector2 rotate = new Vector2(-90f, 90f);
    [SerializeField] private Vector2 targetRotationXZ = new Vector2(0f, -7.5f);

    private Vector2 rotationXZ;
    private Quaternion rotation;
    private float startRotation = 0f;

    private float throttle = 0f;
    private int direction = 1;
    private bool forward = true;

    private bool inputThrottle = false;
    private bool inputBack = false;
    private bool inputLeft = false;
    private bool inputRight = false;

    private bool rotateToStart = false;

    private bool watered = false;

    private void Start()
    {
        startRotation = _transform.rotation.y;
        _transform.rotation = Quaternion.Euler(rotationXZ.x, _transform.rotation.eulerAngles.y, rotationXZ.y);
    }

    private void OnEnable()
    {
        buoyancy.OnWatered += OnWatered;
    }

    private void OnDisable()
    {
        buoyancy.OnWatered -= OnWatered;
    }

    private void OnWatered(bool watered)
    {
        this.watered = watered;
    }

    public void ThrottleUp(InputActionEventData data)
    {
        if (!data.GetButton())
        {
            inputThrottle = false;
            return;
        }
        inputThrottle = true;

        if (rb.velocity.magnitude <= max_reverse_speed && !forward)
        {
            forward = true;
            Reverse();
        }

        if (forward)
            ThrottleUp();
        else
        {
            ThrottleDown();
            Brake();
        }
    }

    private void ThrottleUp()
    {
        throttle += acceleration_cst;
        if (throttle > 1)
            throttle = 1;
    }

    void FixedUpdate()
    {
        //якщо гравець не керуЇ - зд≥йснювати зниженн€ оберт≥в двигуна
        if (!inputThrottle && !inputBack || !watered)
        {
            ThrottleDown();
            Brake();
        }

        float frame_rpm = engine_rpm;
        Vector3 force = propeller.forward * propellers_constant * engine_rpm;

        // еруванн€ прискоренн€м
        if (force.magnitude > 0 && rb.velocity.magnitude < max_speed && watered)
        {
            rb.AddForceAtPosition(force, propeller.position, ForceMode.Acceleration);
        }
        else
            //якщо прискоренн€ немаЇ - спов≥льнюватись за допомогою сили оберненоњ прискоренню
            if (rb.velocity.magnitude > 0)
                rb.AddForce(-rb.velocity*0.1f);

        throttle *= (1.0F - drag * 0.001F);
        engine_rpm = throttle * engine_max_rpm * direction;

        //Ќахил гидроцикла в залежност≥ в≥д швидкост≥ та напр€мку руху
        rotationXZ.x = targetRotationXZ.x * Mathf.Clamp01(rb.velocity.magnitude / 10f) * Mathf.Clamp01(direction);
        _transform.rotation = Quaternion.Euler(rotationXZ.x, _transform.rotation.eulerAngles.y, _transform.rotation.eulerAngles.z);

        //якщо не виконуЇтьс€ поворот - вир≥вн€ти гидроцикл
        if (!inputLeft && !inputRight)
        {
            if(_transform.rotation.eulerAngles.y != startRotation)
            {
                StopAllCoroutines();
                StartCoroutine(RotateJet(startRotation));
            }
        }
        else
        {   //Ћок повороту
            if(_transform.rotation.eulerAngles.y < rotate.x || _transform.rotation.eulerAngles.y > rotate.y)
            {
                float rotationy = _transform.rotation.eulerAngles.y > 180f ? _transform.rotation.eulerAngles.y - 360f : _transform.rotation.eulerAngles.y;
                rotationy = Mathf.Clamp(rotationy, rotate.x, rotate.y);
                _transform.rotation = Quaternion.Euler(rotationXZ.x, rotationy, rotationXZ.y);
            }
        }
    }

    public void ThrottleDown(InputActionEventData data)
    {
        if (!data.GetButton())
        {
            inputBack = false;
            return;
        }
        inputBack = true;

        if (rb.velocity.magnitude <= max_reverse_speed && forward)
        {
            forward = false;
            Reverse();
        }

        if (!forward)
            ThrottleUp();
        else
        {
            ThrottleDown();
            Brake();
        }
    }

    private void ThrottleDown()
    {
        throttle -= acceleration_cst;
        if (throttle < 0)
            throttle = 0;
    }

    public void Brake()
    {
        throttle *= 0.9F;
        if (rb.velocity.magnitude > 0)
            rb.AddForce(-rb.velocity * 0.1f);
    }

    public void Reverse()
    {
        direction *= -1;
    }

    public void RudderRight(InputActionEventData data)
    {
        if (!data.GetButton())
        {
            inputRight = false;
            return;
        }
        inputRight = true;
        rb.AddForce((_transform.right) * rb.velocity.magnitude * 0.1f);

        if (data.GetButtonDown())
        {
            Debug.Log("Right");
            StopAllCoroutines();
            StartCoroutine(RotateJet(rotate.y));
        }
    }

    public void RudderLeft(InputActionEventData data)
    {
        if (!data.GetButton())
        {
            inputLeft = false;
            return;
        }
        inputLeft = true;
        rb.AddForce((-_transform.right) * rb.velocity.magnitude * 0.1f);

        if (data.GetButtonDown())
        {
            Debug.Log("Left");
            StopAllCoroutines();
            StartCoroutine(RotateJet(rotate.x));
        }
    }

    IEnumerator RotateJet(float target)
    {
        rb.angularVelocity = Vector3.zero;
        float startRot = _transform.rotation.eulerAngles.y > 180f ? _transform.rotation.eulerAngles.y - 360f : _transform.rotation.eulerAngles.y;
        float time = 0f;
        float rotation = startRot;

        float startIncline = _transform.rotation.eulerAngles.z > 180f ? _transform.rotation.eulerAngles.z - 360f : _transform.rotation.eulerAngles.z;
        float targetIncline = -Mathf.Clamp(target, -targetRotationXZ.y, targetRotationXZ.y);

        while (rotation != target)
        {
            rotation = Mathf.LerpAngle(startRot, target, time);
            rotation = Mathf.Clamp(rotation, rotate.x, rotate.y);

            rotationXZ.y = Mathf.LerpAngle(startIncline, targetIncline, time);

            if (Mathf.Abs(target - _transform.rotation.y) <= 0.01f)
            {
                _transform.rotation = Quaternion.Euler(rotationXZ.x, target, rotationXZ.y);
                break;
            }
            _transform.rotation = Quaternion.Euler(rotationXZ.x, rotation, rotationXZ.y);
            time += 5f * Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
}
