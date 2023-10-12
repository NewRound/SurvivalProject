using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;
    public float jumpForce;
    public LayerMask groundLayerMask;


    [Header("Look")]
    public Transform cameraCintainer;
    public float minLook;
    public float maxLook;
    private float camCurXRot;
    public float lookSensitivity;

    private Vector2 mouseDelta;

    [HideInInspector]
    public bool canLook = true;

    private Rigidbody _rigidbody;

    public static PlayerController instance;

    private void Awake()
    {
        instance = this;
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // 잠궈 놓겠다.
    }

    // 물리적인 처리
    private void FixedUpdate()
    {
        Move();
    }

    // 카메라 처리
    private void LateUpdate()
    {
        if(canLook)
        {
            CameraLook();
        }
    }

    // 입력값에 따라 변하는 curMovementInput을 항상 체크하며 움직임을 직접 적용시켜주는 부분.
    private void Move()
    {
        Vector3 direction = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        direction *= moveSpeed;
        direction.y = _rigidbody.velocity.y; // y값은 그대로 사용하기 위함.

        _rigidbody.velocity = direction;
    }

    // 카메라 루프
    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;                           // 마우스를 밀고 당겼을 때 위아래로 움직여야 함.
        camCurXRot = Mathf.Clamp(camCurXRot, minLook, maxLook);                 // min, max 안에 값을 가두는 역할
        cameraCintainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);      // 실질적인 마우스 조작에 따른 카메라 이동.

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0); // 좌우 마우스 움직임에 따른 카메라 이동.
    }

    public void OnLookInput(InputAction.CallbackContext context) 
    { 
        mouseDelta = context.ReadValue<Vector2>();

    }

    // 이동 처리
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        // inputAction => started : 제일 처음 1번
        //             => Performed : 누르고 있는 동안.
        //             => Canceled : 땔때 1번.
        if(context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            if(IsGrounded())
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f + (Vector3.up * 0.01f)), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f + (Vector3.up * 0.01f)), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f + (Vector3.up * 0.01f)), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f + (Vector3.up * 0.01f)), Vector3.down)
        };

        for(int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + (transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (transform.right * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.right * 0.2f), Vector3.down);
    }

    public void ToggleCursor(bool toggle)
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
