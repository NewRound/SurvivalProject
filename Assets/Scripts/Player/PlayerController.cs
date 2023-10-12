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
        Cursor.lockState = CursorLockMode.Locked;  // ��� ���ڴ�.
    }

    // �������� ó��
    private void FixedUpdate()
    {
        Move();
    }

    // ī�޶� ó��
    private void LateUpdate()
    {
        if(canLook)
        {
            CameraLook();
        }
    }

    // �Է°��� ���� ���ϴ� curMovementInput�� �׻� üũ�ϸ� �������� ���� ��������ִ� �κ�.
    private void Move()
    {
        Vector3 direction = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        direction *= moveSpeed;
        direction.y = _rigidbody.velocity.y; // y���� �״�� ����ϱ� ����.

        _rigidbody.velocity = direction;
    }

    // ī�޶� ����
    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;                           // ���콺�� �а� ����� �� ���Ʒ��� �������� ��.
        camCurXRot = Mathf.Clamp(camCurXRot, minLook, maxLook);                 // min, max �ȿ� ���� ���δ� ����
        cameraCintainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);      // �������� ���콺 ���ۿ� ���� ī�޶� �̵�.

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0); // �¿� ���콺 �����ӿ� ���� ī�޶� �̵�.
    }

    public void OnLookInput(InputAction.CallbackContext context) 
    { 
        mouseDelta = context.ReadValue<Vector2>();

    }

    // �̵� ó��
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        // inputAction => started : ���� ó�� 1��
        //             => Performed : ������ �ִ� ����.
        //             => Canceled : ���� 1��.
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