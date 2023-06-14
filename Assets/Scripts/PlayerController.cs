using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerController : EntityController
{    
    private List<UnitController> unitList = new List<UnitController>();
    private Vector2 directionMovement;
    
    // private List<UnitController> unitList = new List<UnitController>();

    private List<Vector3> mousePositions = new List<Vector3>();
    private bool isDrawingFormation = false;
    public int mousePositionInterval = 100;

    public int playerSpeed = 1;
    public GameEnums.CommandTypes selectedCommand = GameEnums.CommandTypes.FOLLOW;
    private int selectedSquad = 0;

    public LineRenderer realTimeFormationRenderer;
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
        Vector2 mov = directionMovement * movementSpeed;
        Vector2 newPos = currentPos + mov * Time.fixedDeltaTime;

        rb.MovePosition(newPos);
    }

    void SendCommand(List<Vector3> formationPositions) {
        Debug.Log("Command");
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        BattleManager.Instance.SendCommandToUnits(selectedCommand, mousePos, formationPositions);
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
        Vector3 mousePos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0) ;

        if (mousePositions.Count == 0) {
            mousePositions.Add(mousePos);
            realTimeFormationRenderer.positionCount += 1;
            realTimeFormationRenderer.SetPosition(0, new Vector3(mousePos.x, mousePos.y, 0));
            // Debug.Log("Added Position in " + mousePos);
            return;
        }

        if (Vector2.Distance(mousePos, mousePositions[mousePositions.Count - 1]) > mousePositionInterval) {
            mousePositions.Add(mousePos);
            realTimeFormationRenderer.positionCount += 1;
            realTimeFormationRenderer.SetPosition(realTimeFormationRenderer.positionCount - 1, new Vector3(mousePos.x, mousePos.y, 0));

            // Debug.Log("Added Position in " + mousePos);
            return;
        }
    }
    
    void PlayerControls() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            BattleManager.Instance.ResetGame();
        }

        if(Input.GetKeyDown(KeyCode.Return)) {
            BattleManager.Instance.ResumeGame();
        }

        if (BattleManager.Instance.isPaused) return;

        directionMovement = Vector2.ClampMagnitude(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), 1);

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            isDrawingFormation = !isDrawingFormation;
            if (isDrawingFormation)
            {
                mousePositions.Clear();
                realTimeFormationRenderer.positionCount = 0;
                realTimeFormationRenderer.enabled = true;
            }
        }

        if (Input.GetMouseButtonDown(0) && !isDrawingFormation && !EventSystem.current.IsPointerOverGameObject())
        {
            SendCommand(mousePositions);
            realTimeFormationRenderer.enabled = false;
        }
        else if (Input.GetMouseButton(0) && isDrawingFormation)
        {
            DrawFormation();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            ChangeCommand(true);
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow)) {
            ChangeCommand(false);
        }

        //if (Input.GetKeyDown(KeyCode.Q)) {
        //    selectedSquad = BattleManager.Instance.UpdateSelectedSquad(--selectedSquad);
        //}
        //else if(Input.GetKeyDown(KeyCode.E)) {
        //    selectedSquad = BattleManager.Instance.UpdateSelectedSquad(++selectedSquad);
        //}
    }

    public Vector2 GetPosition() {
        return rb.position;
    }

}
