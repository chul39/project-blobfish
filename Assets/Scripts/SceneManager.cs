using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

    [SerializeField] private GameObject blobfishObject, objectManagerObject;
    [SerializeField] private State currentState;

    public enum State { isPlaying, isPause };
    private Vector2 minBounds, maxBounds;
    private BlobfishController blobfish;
    private ObjectManager objectManager;


    private void Awake() {
        blobfish = blobfishObject.GetComponent<BlobfishController>();
        objectManager = objectManagerObject.GetComponent<ObjectManager>();
        InitBoundaries();
    }

    private void Update() {
        if(currentState == State.isPause) return;
        if(Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            OnInputEvent(mousePos2D);
        }
    }

    public Vector2[] GetBoundaries() => new Vector2[] { minBounds, maxBounds };

    public GameObject GetRandomObject(Vector2 refPos) => objectManager.GetRandomObject(refPos);

    public bool GetGameState() => currentState == State.isPlaying;

    public void SetGameState(bool targetState) => currentState = targetState ? State.isPlaying : State.isPause;

    public void UpdateHealthText(int hp, int maxHp) => gameObject.GetComponent<UIController>().UpdateHealthText(hp, maxHp);

    public void UpdateGrowthText(int lv, int exp, int toNextExp) => gameObject.GetComponent<UIController>().UpdateGrowthText(lv, exp, toNextExp);

    public void SetGameEndState() {
        currentState = State.isPause;
        gameObject.GetComponent<UIController>().HandleGameOver();
    }

    public void RestartGame() {
        blobfish.ChangeToInitialState();
        objectManager.DeactiveAllObject();
        currentState = State.isPlaying;
    }

    private void InitBoundaries() {
        Camera mainCamera = Camera.main;
        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0,0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1,1));
    }

    private void OnInputEvent(Vector2 inputPos) {
        RaycastHit2D hit = Physics2D.Raycast(inputPos, Vector2.zero);
        if(hit.collider != null && hit.collider.gameObject.tag == "PickUp") hit.collider.gameObject.SetActive(false);
    }

}
