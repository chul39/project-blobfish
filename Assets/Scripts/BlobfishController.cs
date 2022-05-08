using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobfishController : MonoBehaviour {

    [SerializeField] private State currentState;
    [SerializeField] float moveSpeed, eventInterval, eatingDistance, floatingTextDistance, paddingLeft, paddingRight, paddingTop, paddingBottom;
    [SerializeField] private int maxHealth, lv1GrowthCap, lv2GrowthCap, lv3GrowthCap;
    [SerializeField] private GameObject sceneManagerObject;
    [SerializeField] private GameObject positiveFloatingText, negativeFloatingText, statusText, levelUpText;

    public enum State { isIdle, isRoaming, isHungry }
    private SceneManager sceneManager;
    private Vector2 minBounds, maxBounds, roamVector;
    private GameObject currentTarget;
    private Animator animator;
    private float intervalTimeDiff;
    private int currentLv = 0, health, growth, currentGrowthCap;

    private void Start() {
        health = maxHealth;
        growth = 0;
        sceneManager = sceneManagerObject.GetComponent<SceneManager>();
        animator = GetComponent<Animator>();
        Vector2[] boundaries = sceneManager.GetBoundaries();
        minBounds = boundaries[0];
        maxBounds = boundaries[1];
        currentState = State.isIdle;
        currentGrowthCap = lv1GrowthCap;
        sceneManager.UpdateHealthText(health, maxHealth);
        sceneManager.UpdateGrowthText(currentLv, growth, currentGrowthCap);
    }

    private void Update() {
        if(!sceneManager.GetGameState()){
            animator.enabled = false;
        } else {
            UpdateFloatingTextPosition();
            animator.enabled = true;
            switch(currentState) {
                case State.isIdle:
                    intervalTimeDiff += Time.deltaTime;
                    if(intervalTimeDiff > eventInterval) UpdateStateInterval();
                    break;
                case State.isRoaming:
                    intervalTimeDiff += Time.deltaTime;
                    Roam();
                    if(intervalTimeDiff > eventInterval) UpdateStateInterval();
                    break;
                case State.isHungry:
                    intervalTimeDiff = 0f;
                    if(currentTarget != null && currentTarget.activeInHierarchy) MoveToTarget();
                    else {
                        currentState = State.isIdle;
                        statusText.SetActive(false);
                    }
                    break;
            }
        }
    }

    public void SetHealth(int hp) => health = hp;

    public void ChangeToInitialState() {
        health = maxHealth;
        growth = 0;
        intervalTimeDiff = 0f;
        gameObject.transform.position = transform.position = new Vector3(0f, 0f, 5f);
        roamVector = Vector2.zero;
        currentLv = 0;
        currentGrowthCap = lv1GrowthCap;
        currentTarget = null;
        currentState = State.isIdle;
        sceneManager.UpdateHealthText(health, maxHealth);
        sceneManager.UpdateGrowthText(currentLv, growth, currentGrowthCap);
    }

    private void UpdateFloatingTextPosition() {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + floatingTextDistance, -5f);
        if(currentLv == 2) pos.y = pos.y + 0.5f;
        else if(currentLv == 3) pos.y = pos.y + 1f;
        positiveFloatingText.transform.position = pos;
        negativeFloatingText.transform.position = pos;
        statusText.transform.position = pos;
    }

    private void UpdateStateInterval() {
        statusText.SetActive(false);
        int randomResult = Random.Range(0, 2);
        switch(randomResult) {
            case 0:
                currentState = currentState == State.isIdle ? State.isRoaming : State.isIdle;
                if(currentState == State.isRoaming) {
                    float x = Random.Range(-1f, 1f);
                    float y = Random.Range(-1f, 1f);
                    roamVector = new Vector2(x, y);
                    transform.localRotation = x > 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
                }
                intervalTimeDiff = 0f;
                transform.position = new Vector3(transform.position.x, transform.position.y, 5f);
                break;
            case 1:
                currentTarget = sceneManager.GetRandomObject(transform.position);
                currentState = currentTarget != null ? State.isHungry : State.isIdle;
                break;
        }
    }

    private void Roam() {
        Vector3 newPos = new Vector3();
        float delta = moveSpeed * Time.deltaTime;
        newPos.x = Mathf.Clamp(transform.position.x + (delta * roamVector.x), minBounds.x + paddingLeft, maxBounds.x - paddingRight);
        newPos.y = Mathf.Clamp(transform.position.y + (delta * roamVector.y), minBounds.y + paddingBottom, maxBounds.y - paddingTop);
        newPos.z = 5f;
        UpdateRoamVectorAtBorder(newPos);
        transform.position = newPos;
    }

    private void UpdateRoamVectorAtBorder(Vector3 newPos) {
        if(newPos.x <= minBounds.x + paddingLeft) {
            roamVector.x = roamVector.x * -1;
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        } else if(newPos.x >= maxBounds.x - paddingRight) {
            roamVector.x = roamVector.x * -1;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void MoveToTarget() {
        if(currentTarget == null) return;
        statusText.SetActive(true);
        Vector2 originalPosition = transform.position;
        Vector2 targetPosition = currentTarget.transform.position;
        float delta = moveSpeed * Time.deltaTime;
        float distanceFromTarget = eatingDistance + ((float)currentLv/10);
        transform.localRotation = targetPosition.x - originalPosition.x > 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        targetPosition.x = targetPosition.x - originalPosition.x > 0 ? targetPosition.x - distanceFromTarget : targetPosition.x + distanceFromTarget;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, delta);
    }

    private void LevelUp() {
        StartCoroutine(ShowLevelUpText());
        switch(currentLv) {
            case 0:
                growth -= currentGrowthCap;
                currentLv = 1;
                currentGrowthCap = lv2GrowthCap;
                transform.localScale = new Vector3(0.17f, 0.17f, 1f);
                break;
            case 1:
                growth -= currentGrowthCap;
                currentLv = 2;
                currentGrowthCap = lv3GrowthCap;
                transform.localScale = new Vector3(0.2f, 0.2f, 1f);
                break;
            case 2:
                growth = 0;
                currentLv = 3;
                currentGrowthCap = 0;
                transform.localScale = new Vector3(0.25f, 0.25f, 1f);
                break;
        }
    }
    IEnumerator ShowPositiveFloatingText(string str) {
        negativeFloatingText.SetActive(false);
        positiveFloatingText.GetComponent<TMPro.TMP_Text>().text = str;
        positiveFloatingText.SetActive(true);
        yield return new WaitForSeconds(2f);
        positiveFloatingText.SetActive(false);
    }

    IEnumerator ShowNegativeFloatingText(string str) {
        positiveFloatingText.SetActive(false);
        negativeFloatingText.GetComponent<TMPro.TMP_Text>().text = str;
        negativeFloatingText.SetActive(true);
        yield return new WaitForSeconds(2f);
        negativeFloatingText.SetActive(false);
    }

    IEnumerator ShowLevelUpText() {
        levelUpText.SetActive(true);
        yield return new WaitForSeconds(4f);
        levelUpText.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "PickUp" && currentState == State.isHungry) {
            statusText.SetActive(false);
            int healthToChange = col.gameObject.GetComponent<ObjectController>().GetHealthValue();
            if(healthToChange > 0 && currentLv != 3) {
                growth += 1;
                if(growth >= currentGrowthCap) LevelUp();
                StartCoroutine(ShowPositiveFloatingText("+" + healthToChange.ToString() + "HP +1EXP"));
            } else if (healthToChange > 0 && currentLv == 3) {
                StartCoroutine(ShowPositiveFloatingText("+" + healthToChange.ToString() + "HP"));
            } else {
                StartCoroutine(ShowNegativeFloatingText(healthToChange.ToString() + "HP"));
            }
            health += healthToChange;
            if(health > maxHealth) health = maxHealth;
            if(health > 0){
                animator.Play("Blob_Eat");
                col.gameObject.SetActive(false);
                currentTarget = null;
                intervalTimeDiff = 0f;
                currentState = State.isIdle;
                transform.position = new Vector3(transform.position.x, transform.position.y, 5f);
            } else {
                sceneManager.SetGameEndState();
            }
            sceneManager.UpdateHealthText(health, maxHealth);
            sceneManager.UpdateGrowthText(currentLv, growth, currentGrowthCap);
        }
    }

}
