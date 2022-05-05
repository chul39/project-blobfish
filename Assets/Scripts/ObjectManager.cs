using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RefObject {

    public GameObject prefab;
    public bool hasInit, hasFixedSpawnX, hasFixedSpawnY;
    public float spawnPosX, spawnPosY;

    public RefObject(GameObject newPrefab) => (prefab, hasInit) = (newPrefab, false);

    public void SetInit(bool init) => hasInit = init;

}

public class ObjectManager : MonoBehaviour {

    [SerializeField] private float timeDiff, eventInterval;
    [SerializeField] private GameObject food1, food2, food3, trash1, trash2, trash3, trash4, trash5, trash6;

    private List<RefObject> pickups = new List<RefObject>();
    private List<GameObject> pooledPickups = new List<GameObject>();
    private SceneManager sceneManager;

    private void Awake() {
        sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        pickups.Add(new RefObject(food1));
        pickups.Add(new RefObject(food2));
        pickups.Add(new RefObject(food3));
        pickups.Add(new RefObject(trash1));
        pickups.Add(new RefObject(trash2));
        pickups.Add(new RefObject(trash3));
        pickups.Add(new RefObject(trash4));
        pickups.Add(new RefObject(trash5));
        pickups.Add(new RefObject(trash6));
    }

    private void Update() {
        if(!sceneManager.GetGameState()) return;
        timeDiff += Time.deltaTime;
        if(timeDiff > eventInterval) SpawnOne();
    }

    public void SpawnOne() {
        timeDiff = 0f;
        RefObject refTarget = pickups[Random.Range(0, pickups.Count)];
        Vector2 spawnPos = refTarget.prefab.GetComponent<ObjectController>().GetSpawnPosition();
        GameObject target = pooledPickups.Find(x => x.name.Contains(refTarget.prefab.name) && !x.activeInHierarchy);
        if(!refTarget.hasInit) {
            InitPrefab(refTarget.prefab, spawnPos);
            refTarget.SetInit(true);
        } else if(target == null) {  
            List<GameObject> tmp = pooledPickups.FindAll(x => x.name.Contains(refTarget.prefab.name) && x.activeInHierarchy);
            if(tmp.Count < 3) InitPrefab(refTarget.prefab, spawnPos);
        } else {
            target.transform.position = spawnPos;
            target.SetActive(true); 
        }
    }

    private void InitPrefab(GameObject targetPrefab, Vector2 spawnPosition) {
        Instantiate(targetPrefab, spawnPosition, transform.rotation);
        pooledPickups.Add(GameObject.Find(targetPrefab.name + "(Clone)"));
    }

    public GameObject GetRandomObject(Vector2 refPosition) => pooledPickups.Where(x => x.activeInHierarchy).Count() > 0 ? pooledPickups[Random.Range(0, pooledPickups.Count)] : null;

    public void DeactiveAllObject() {
        foreach(GameObject item in pooledPickups) {
            if(!item.activeInHierarchy) continue;
            item.SetActive(false);
        }
    }

}