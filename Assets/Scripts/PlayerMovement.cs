using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 0.5f;
    private PlayerInput playerInput;
    private Vector2 moveDirection = Vector2.zero;
    private Rigidbody2D rb2D;

    private void Awake() => playerInput = new PlayerInput();
    private void OnEnable() => playerInput.Enable();
    private void OnDisable() => playerInput.Disable();
    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        playerInput.Player.Move.performed += ctx => Move(ctx);
    }

    // Update is called once per frame
    void Update()
    {
    }


    void Move(CallbackContext context) {
        moveDirection = playerInput.Player.Move.ReadValue<Vector2>();
        if (moveDirection.x != 0) {
            //rb2D.MovePosition(new Vector2(transform.position.x, transform.position.y) + moveDirection * movementSpeed);
            rb2D.velocity = new Vector2(transform.position.x, transform.position.y) + moveDirection * movementSpeed;
        }
    }
}
