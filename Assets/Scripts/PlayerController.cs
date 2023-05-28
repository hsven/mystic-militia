using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb = null;
    
    private Vector2 directionMovement;
    
    // private List<UnitController> unitList = new List<UnitController>();

    private List<Vector3> mousePositions = new List<Vector3>();
    private bool isDrawingFormation = false;
    public int mousePositionInterval = 100;

    public int playerSpeed = 1;
    public GameEnums.CommandTypes selectedCommand = GameEnums.CommandTypes.FOLLOW;

    // Start is called before the first frame update
    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerControls();
    }

    void FixedUpdate() {
        PlayerMovement();
    }

    void PlayerMovement() {
        Vector2 currentPos = rb.position;
        Vector2 mov = directionMovement * playerSpeed;
        Vector2 newPos = currentPos + mov * Time.fixedDeltaTime;

        rb.MovePosition(newPos);
    }

    void SendCommand() {
        Debug.Log("Command");
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        foreach (UnitController unit in BattleManager.Instance.playerUnits)
        {
            unit.SetCommand(selectedCommand, new Vector2(mousePos.x, mousePos.y));
        }
    }

    void ChangeCommand(bool isUp) {
        GameEnums.CommandTypes newCommand = selectedCommand;
        System.Array enumTypes = System.Enum.GetValues(typeof(GameEnums.CommandTypes));

        if (isUp) {
            newCommand++;

            selectedCommand = (GameEnums.CommandTypes) ((int)(newCommand) % enumTypes.Length);
        }
        else{
            newCommand--;

            if (newCommand == 0) {
                selectedCommand = GameEnums.CommandTypes.FOLLOW;
            }
            else {
                selectedCommand = (GameEnums.CommandTypes) (Mathf.Abs(enumTypes.Length + (int)(newCommand)) % enumTypes.Length);
            }
        }
        Debug.Log("Current Command: " + selectedCommand);
    }

    void DrawFormation() {
        if (!isDrawingFormation) {
            mousePositions.Clear();
            isDrawingFormation = true;
        }

        Vector3 mousePos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0) ;

        if (mousePositions.Count == 0) {
            mousePositions.Add(mousePos);
            Debug.Log("Added Position in " + mousePos);
            return;
        }

        if (Vector2.Distance(mousePos, mousePositions[mousePositions.Count - 1]) > mousePositionInterval) {
            mousePositions.Add(mousePos);
            Debug.Log("Added Position in " + mousePos);
            return;
        }
    }
    
    void PlayerControls() {
        directionMovement = Vector2.ClampMagnitude(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), 1);

        if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt)) {
            DrawFormation();
        }
        else if (Input.GetMouseButtonDown(0)) {
            isDrawingFormation = false;
            BattleManager.Instance.SetFormation(mousePositions);
            SendCommand();
        }
        else{
            isDrawingFormation = false;
        } 

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            ChangeCommand(true);
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow)) {
            ChangeCommand(false);
        }

    }

    public int RegisterUnit(UnitController unit) {
        BattleManager.Instance.playerUnits.Add(unit);
        return BattleManager.Instance.playerUnits.Count - 1;
    }

    public Vector2 GetPosition() {
        return rb.position;
    }
}
