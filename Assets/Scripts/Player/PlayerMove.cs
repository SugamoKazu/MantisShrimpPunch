using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float smoothSpeed = 3f;
    [SerializeField] private float chargeDeadZone = 0.05f;

    private Quaternion startRotation;
    private float lastNormX = 0f;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        if (!ModeManager.isConnectionMode)
        {
            float mouseX = Input.mousePosition.x;
            float normX = (mouseX / Screen.width) * 2f - 1f;
            normX = Mathf.Clamp(normX, -1f, 1f);

            bool isCharging = Input.GetMouseButton(0) || Input.GetMouseButton(1);

            if (isCharging)
            {
                if (Mathf.Abs(normX - lastNormX) < chargeDeadZone)
                {
                    normX = lastNormX;
                }
            }
            
            lastNormX = normX;

            float targetAngle = normX * 90f;

            Quaternion targetRotation = startRotation * Quaternion.Euler(0, targetAngle, 0);

            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                targetRotation,
                Time.deltaTime * smoothSpeed
            );
        }
    }
}
