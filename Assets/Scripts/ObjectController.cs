using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour {

    [SerializeField] private bool allowMoveX, allowMoveY, fixSpawnPoint, isLivingThing;
    [SerializeField] private float moveSpeed, timeDiff, paddingX, paddingY, spawnPosY;
    [SerializeField] private int healthValue;

    private Vector2 minBounds, maxBounds, roamVector;
    private SceneManager sceneManager;
    private Animator animator;

    private void Start() {
        sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        animator = GetComponent<Animator>();
        Vector2[] boundaries = sceneManager.GetBoundaries();
        minBounds = boundaries[0];
        maxBounds = boundaries[1];
    }

    private void Update() {
        if(!sceneManager.GetGameState()){
            animator.enabled = false;
        } else {
            animator.enabled = true;
            if(isLivingThing) Roam();
            else Move();
        }
    }
 
    public Vector2 GetSpawnPosition() {
        float posX = Random.Range(-2.5f, 2.5f);
        float posY = fixSpawnPoint ? spawnPosY : Random.Range(-4f, 4f);
        Vector2 pos = new Vector2(posX, posY);
        Vector2 blobfishPos = GameObject.Find("Blobfish").transform.position;
        if(Vector2.Distance(pos, blobfishPos) <= 1.5f) ChangeSpawnPoint(pos);
        return pos;
    }

    public int GetHealthValue() => healthValue;

    private void ChangeSpawnPoint(Vector2 pos) {
        if(pos.x > 2f) pos.x -= 2.5f;
        else if(pos.x < -2f) pos.x += 2.5f;
        else pos.x *= -1;
        if(!fixSpawnPoint) {
            if(pos.y > 2f) pos.y -= 4f;
            else if(pos.y < -2f) pos.y += 4f;
            else pos.y *= -1;
        }
    }

    private void Roam() {
        timeDiff += Time.deltaTime;
        if(timeDiff > 5f){
            roamVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            timeDiff = 0f;
        }
        Vector2 newPos = new Vector2();
        float delta = moveSpeed * Time.deltaTime;
        newPos.x = allowMoveX ? Mathf.Clamp(transform.position.x + (delta * roamVector.x), minBounds.x + paddingX, maxBounds.x - paddingX) : transform.position.x;
        newPos.y = allowMoveY ? Mathf.Clamp(transform.position.y + (delta * roamVector.y), minBounds.y + paddingY, maxBounds.y - paddingY) : transform.position.y;
        transform.position = newPos;
    }

    private void Move() {
        Vector2 newPos = new Vector2();
        float delta = moveSpeed * Time.deltaTime;
        newPos.x = allowMoveX ? Mathf.Clamp(transform.position.x + delta, minBounds.x + paddingX, maxBounds.x - paddingX) : transform.position.x;
        newPos.y = allowMoveY ? Mathf.Clamp(transform.position.y + delta, minBounds.y + paddingY, maxBounds.y - paddingY) : transform.position.y;
        transform.position = newPos;
    }

}
