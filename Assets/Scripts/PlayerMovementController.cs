using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerMovementController : NetworkBehaviour
{
    public CharacterController controller;
    public Transform Cam;

    private Camera camcam;
    public CinemachineFreeLook cfl;

    public float speed;
    public float SmoothTurns = 0.1f;
    float TurnSmoothVel;

    public float Grav = 10f;

    private void Start()
    {
        camcam = Cam.gameObject.GetComponent<Camera>();
    }

    private void Update()
    {
        if (IsLocalPlayer)
        {
            if (!camcam.enabled)
            {
                camcam.enabled = true;
            }
            cfl.Priority = 10;
        } else
        {
            cfl.Priority = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!IsLocalPlayer) //If not the player, don't move
        {
            return;
        }

        Move();
    }

    private void Move()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(hor, 0, vert).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float TargetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetAngle, ref TurnSmoothVel, SmoothTurns);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, TargetAngle, 0f) * Vector3.forward;

            //sprint

            Vector3 movement = moveDir.normalized * speed * Time.deltaTime;
            controller.Move(movement);
        }
    }
}
