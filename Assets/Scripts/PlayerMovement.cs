using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 0.5f;
    public Tilemap map;
    public Tilemap background;
    public TileBase ladder;
    public Text warning;
    private PlayerInput playerInput;
    private Vector2 moveDirection = Vector2.zero;
    private Vector3 velocity = Vector3.zero;
    private int collectedDiamonds = 0;
    private bool isMoving;
    private Rigidbody2D rb2D;

    private void Awake() => playerInput = new PlayerInput();
    private void OnEnable() => playerInput.Enable();
    private void OnDisable() => playerInput.Disable();
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("collectedDiamonds", 30);
        rb2D = GetComponent<Rigidbody2D>();
        playerInput.Player.Move.performed += ctx => Move(ctx);
        playerInput.Player.Move.started += ctx => SetIsMoving(ctx);
        playerInput.Player.Move.canceled += ctx => SetIsNotMoving(ctx);
        playerInput.Player.Fire.performed += ctx => Dig(ctx);
        playerInput.Player.Return.performed += ctx => ReturnDiamonds(ctx);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving) {
            Move(new CallbackContext());
        }
    }

    private void SetIsMoving(CallbackContext context) => isMoving = true;
    private void SetIsNotMoving(CallbackContext context) => isMoving = false;

    void Move(CallbackContext context) {
        moveDirection = playerInput.Player.Move.ReadValue<Vector2>();
        if (moveDirection.x != 0) {
            // Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(moveDirection.x * movementSpeed, rb2D.velocity.y);
			// And then smoothing it out and applying it to the character
			//rb2D.velocity = Vector3.SmoothDamp(rb2D.velocity, targetVelocity, ref velocity, 3);
            // Test without smoothing since it doesnt work
            rb2D.velocity = targetVelocity;
        }
        if (moveDirection.y < 0) {
            //Look down
            //if Ladder climb down else dig
        }
        if (moveDirection.y > 0) {
            //Look up;
            //if Ladder climb up
            Vector3Int gridPosition = map.WorldToCell(transform.position);
            if (background.HasTile(gridPosition)) {
                Vector3 targetVelocity = new Vector2(rb2D.velocity.x, moveDirection.y * movementSpeed);
                rb2D.velocity = targetVelocity;
            }
        }
    }

    void Dig(CallbackContext context) {
        // Find the tile we are looking at
        Vector2 tileUnderneath = new Vector2(transform.position.x, transform.position.y) + moveDirection;
        // Translate worldPosition to gridPosition
        Vector3Int gridPosition = map.WorldToCell(tileUnderneath);
        // Check if position is on a tile
        if (map.HasTile(gridPosition)) {
            // If tile contains resources -> collect
            TileBase tileToDestory = map.GetTile(gridPosition);
            if (tileToDestory.name == "Diamonds") {
                // Collect diamonds
                collectedDiamonds += 3;
            }
            // If tile is destroyable
            if (tileToDestory.name == "Dirt" || tileToDestory.name == "Diamonds") {
            // Remove looked at tile
            map.SetTile(gridPosition,null);
            }
        }
        // If direction was down place a ladder
        if (moveDirection.y < 0) {
            background.SetTile(gridPosition,ladder);
            // And add a ladder above
            Vector2 tileAbove = new Vector2(transform.position.x, transform.position.y + 1);
            Vector3Int gridPositionAbove = map.WorldToCell(tileAbove);
            background.SetTile(gridPositionAbove,ladder);
        }
    }

    void ReturnDiamonds(CallbackContext context) {
        if (IsNearGenerator()) {
            SaveCollectedDiamonds();
        } else {
            ShowWarning("Get near a generator to return resources!");
        }
    }

    bool IsNearGenerator() {
        // Find tiles next to player
        for (int i = -1; i<1; i++) {
            for (int j = -1; j<1; j++) {
                Vector2 direction = new Vector2(i,j);
                Vector2 tileNextTo = new Vector2(transform.position.x, transform.position.y) + direction;
                Vector3Int gridPosition = map.WorldToCell(tileNextTo);
                if (map.HasTile(gridPosition)) {
                    // If tile is generator
                    TileBase isGenerator = map.GetTile(gridPosition);
                    if (isGenerator.name == "Generator") {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void SaveCollectedDiamonds() {
        PlayerPrefs.SetInt("collectedDiamonds", PlayerPrefs.GetInt("collectedDiamonds",0) + collectedDiamonds);
        PlayerPrefs.Save();
        collectedDiamonds = 0;
    }

    void ShowWarning(string msg) {
        warning.text = msg;
        warning.enabled = true;
        Invoke("RemoveWarning", 3f);
    }

    void RemoveWarning() {
        warning.enabled = false;
    }
}
