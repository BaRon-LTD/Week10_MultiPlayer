// using UnityEngine;
// using Fusion;

// public class Player: NetworkBehaviour
// {
//     private CharacterController _cc;

//     [SerializeField] float speed = 5f;
//     [SerializeField] GameObject ballPrefab;

//     private Camera firstPersonCamera;
//     public override void Spawned() {
//         _cc = GetComponent<CharacterController>();
//         if (HasStateAuthority) {
//             firstPersonCamera = Camera.main;
//             var firstPersonCameraComponent = firstPersonCamera.GetComponent<FirstPersonCamera>();
//             if (firstPersonCameraComponent && firstPersonCameraComponent.isActiveAndEnabled)
//                 firstPersonCameraComponent.SetTarget(this.transform);
//         }
//     }

//     private Vector3 moveDirection;
//     public override void FixedUpdateNetwork() {
//         if (GetInput(out NetworkInputData inputData)) {
//             if (inputData.moveActionValue.magnitude > 0) {
//                 inputData.moveActionValue.Normalize();   //  Ensure that the vector magnitude is 1, to prevent cheating.
//                 moveDirection = new Vector3(inputData.moveActionValue.x, 0, inputData.moveActionValue.y);
//                 Vector3 DeltaX = speed * moveDirection * Runner.DeltaTime;
//                 //Debug.Log($"{speed} * {moveDirection} * {Runner.DeltaTime} = {DeltaX}");
//                 _cc.Move(DeltaX);
//             }

//             if (HasStateAuthority) { // Only the server can spawn new objects ; otherwise you will get an exception "ClientCantSpawn".
//                 if (inputData.shootActionValue) {
//                     Debug.Log("SHOOT!");
//                     Runner.Spawn(ballPrefab,
//                         transform.position + moveDirection, Quaternion.LookRotation(moveDirection),
//                         Object.InputAuthority);
//                 }
//             }
//         }
//     }
// }



// using UnityEngine;
// using Fusion;

// public class Player : NetworkBehaviour
// {
//     private CharacterController _cc;

//     [SerializeField] float speed = 5f;
//     [SerializeField] GameObject ballPrefab;
//     [SerializeField] GameObject shieldEffectPrefab;

//     private Camera firstPersonCamera;

//     // Shield-related fields
//     [Networked] private TickTimer ShieldTimer { get; set; } // Tracks shield duration
//     [Networked] public bool IsShielded { get; private set; } // Tracks whether the player is shielded
//     private GameObject shieldEffectInstance;

//     public override void Spawned()
//     {
//         _cc = GetComponent<CharacterController>();
//         if (HasStateAuthority)
//         {
//             firstPersonCamera = Camera.main;
//             var firstPersonCameraComponent = firstPersonCamera.GetComponent<FirstPersonCamera>();
//             if (firstPersonCameraComponent && firstPersonCameraComponent.isActiveAndEnabled)
//                 firstPersonCameraComponent.SetTarget(this.transform);
//         }
//     }

//     private Vector3 moveDirection;

//     public override void FixedUpdateNetwork()
//     {
//         if (GetInput(out NetworkInputData inputData))
//         {
//             if (inputData.moveActionValue.magnitude > 0)
//             {
//                 inputData.moveActionValue.Normalize(); // Ensure that the vector magnitude is 1
//                 moveDirection = new Vector3(inputData.moveActionValue.x, 0, inputData.moveActionValue.y);
//                 Vector3 DeltaX = speed * moveDirection * Runner.DeltaTime;
//                 _cc.Move(DeltaX);
//             }

//             if (HasStateAuthority)
//             {
//                 if (inputData.shootActionValue)
//                 {
//                     Debug.Log("SHOOT!");
//                     Runner.Spawn(ballPrefab,
//                         transform.position + moveDirection, Quaternion.LookRotation(moveDirection),
//                         Object.InputAuthority);
//                 }
//             }
//         }

//         // Check if the shield timer has expired
//         if (IsShielded && ShieldTimer.Expired(Runner))
//         {
//             DeactivateShield();
//         }
//     }

//     public void ActivateShield(float duration)
//     {
//         if (IsShielded) return; // Prevent multiple shields

//         IsShielded = true;
//         ShieldTimer = TickTimer.CreateFromSeconds(Runner, duration);

//         // Instantiate the shield visual effect
//         if (shieldEffectPrefab != null)
//         {
//             shieldEffectInstance = Instantiate(shieldEffectPrefab, transform.position, Quaternion.identity, transform);
//         }
//     }

//     private void DeactivateShield()
//     {
//         IsShielded = false;

//         // Destroy the shield visual effect
//         if (shieldEffectInstance != null)
//         {
//             Destroy(shieldEffectInstance);
//             shieldEffectInstance = null;
//         }
//     }

//     private void OnCollisionEnter(Collision collision)
//     {
//         if (IsShielded && collision.gameObject.CompareTag("Ball"))
//         {
//             // Absorb the ball hit
//             Debug.Log("Shield absorbed the ball!");
//         }
//     }
// }







using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    private CharacterController _cc;

    [SerializeField] float speed = 5f;
    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject shieldEffectPrefab;

    private Camera firstPersonCamera;

    // Shield-related fields
    [Networked] private TickTimer ShieldTimer { get; set; } // Tracks shield duration
    [Networked] public bool IsShielded { get; private set; } // Tracks whether the player is shielded
    private GameObject shieldEffectInstance;

    public override void Spawned()
    {
        _cc = GetComponent<CharacterController>();
        if (HasStateAuthority)
        {
            firstPersonCamera = Camera.main;
            var firstPersonCameraComponent = firstPersonCamera.GetComponent<FirstPersonCamera>();
            if (firstPersonCameraComponent && firstPersonCameraComponent.isActiveAndEnabled)
                firstPersonCameraComponent.SetTarget(this.transform);
        }
    }

    private Vector3 moveDirection;

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData inputData))
        {
            if (inputData.moveActionValue.magnitude > 0)
            {
                inputData.moveActionValue.Normalize(); // Ensure that the vector magnitude is 1
                moveDirection = new Vector3(inputData.moveActionValue.x, 0, inputData.moveActionValue.y);
                Vector3 DeltaX = speed * moveDirection * Runner.DeltaTime;
                _cc.Move(DeltaX);
            }

            if (HasStateAuthority)
            {
                if (inputData.shootActionValue)
                {
                    Debug.Log("SHOOT!");
                    Runner.Spawn(ballPrefab,
                        transform.position + moveDirection, Quaternion.LookRotation(moveDirection),
                        Object.InputAuthority);
                }
            }
        }

        // Check if the shield timer has expired
        if (IsShielded && ShieldTimer.Expired(Runner))
        {
            DeactivateShield();
        }
    }

    public void ActivateShield(float duration)
    {
        if (IsShielded) return; // Prevent multiple shields

        IsShielded = true;
        ShieldTimer = TickTimer.CreateFromSeconds(Runner, duration);

        // Instantiate the shield visual effect
        if (shieldEffectPrefab != null)
        {
            shieldEffectInstance = Instantiate(shieldEffectPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    private void DeactivateShield()
    {
        IsShielded = false;

        // Destroy the shield visual effect
        if (shieldEffectInstance != null)
        {
            Destroy(shieldEffectInstance);
            shieldEffectInstance = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsShielded && collision.gameObject.CompareTag("Ball"))
        {
            // Absorb the ball hit
            Debug.Log("Shield absorbed the ball!");
        }
    }
}
