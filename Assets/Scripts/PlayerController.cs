using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    
    [Range(1, 100)]
    public int movementSpeed = 50;

    public LineRenderer realTimeFormationRenderer;

    [SerializeField]
    Volume volume;
    Vignette vignette = null;
    private Tween vignetteTween;
    [SerializeField]
    PixelPerfectCamera ppCamera;

    private void Start()
    {
        volume.profile.TryGet<Vignette>(out vignette);
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
        UIBattleSquadSelector.Instance.SelectedCommandType(selectedCommand);
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
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!BattleManager.Instance.isBattleActive) BattleManager.Instance.StartGame();
        }

        if (!BattleManager.Instance.isBattleActive) return;

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if(!BattleManager.Instance.isPaused) BattleManager.Instance.PauseGame();
            else BattleManager.Instance.ResumeGame();
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

                UpdateVignetteTween(0.5f, 1.5f);
            }
            else UpdateVignetteTween(0f, 0.75f);
        }

        if(Input.GetMouseButtonUp(0) && isDrawingFormation)
        {
            isDrawingFormation = false;
            UpdateVignetteTween(0f, 0.75f);

            SendCommand(mousePositions);
            realTimeFormationRenderer.enabled = false;
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

        //if (Input.GetKeyDown(KeyCode.UpArrow)) {
        //    ChangeCommand(true);
        //}
        //else if(Input.GetKeyDown(KeyCode.DownArrow)) {
        //    ChangeCommand(false);
        //}

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeCommand(true);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeCommand(false);
        }

        if(Input.mouseScrollDelta.y > 0)
        {
            CameraScroll(true);
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            CameraScroll(false);
        }
    }

    private void UpdateVignetteTween(float target, float duration)
    {
        vignetteTween.Kill();
        vignetteTween = DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, target, duration);
    }

    private void CameraScroll(bool isUp)
    {
        int maxPPU = 32;
        int minPPU = 22;

        if (isUp)
        {
            var targetVal = ppCamera.assetsPPU + 2;
            if (targetVal > maxPPU) targetVal = maxPPU;

            DOTween.To(() => ppCamera.assetsPPU, x => ppCamera.assetsPPU = x, targetVal, 0f);
        }
        else
        {
            var targetVal = ppCamera.assetsPPU - 2;
            if(targetVal < minPPU) targetVal = minPPU;

            DOTween.To(() => ppCamera.assetsPPU, x => ppCamera.assetsPPU = x, targetVal, 0f);
        }
    }
}
