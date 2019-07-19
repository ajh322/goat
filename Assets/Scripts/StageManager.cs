using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public List<GameObject> StageList;

    [SerializeField] private GameObject StartPosition;
    [SerializeField] private GameObject Stage;

    private static StageManager _instance;

    public static StageManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnStages();
    }

    public void SpawnStages()
    {
        ClearStages();
        // StageList 7~15
        //-1~1
        float currentX = 0, currentY = 0;
        for (int i = 0; i < 100; i++)
        {
            currentX -= Random.Range(5, 8);
            currentY += Random.Range(-2, 2);
            GameObject obj = Instantiate(Stage,
                StartPosition.transform.position + new Vector3(currentX, currentY, 0),
                Quaternion.identity);
            StageList.Add(obj);
        }
    }

    public void ClearStages()
    {
        foreach (GameObject obj in StageList)
        {
            if (obj)
                Destroy(obj);
        }

        StageList.Clear();
    }
}
