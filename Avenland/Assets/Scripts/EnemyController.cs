using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int sightRange;
    [SerializeField] private int health;

    private GameManager gameManager;
    public Vector2 enemyLocation;

    private GameObject detectionRangeVisualizer;

    private bool visualizingDetection = false;
    private float sightVisualizationMaxSize;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        enemyLocation = new Vector2(transform.position.x / 1.28f, transform.position.y / 1.28f);

        detectionRangeVisualizer = transform.GetChild(0).gameObject;

        //adjust the visual indicator to match the detection range
        detectionRangeVisualizer.transform.localScale = new Vector3(0, 0, 0);
        sightVisualizationMaxSize = 25.6f * sightRange;
    }

    // Update is called once per frame
    void Update()
    {
        if (!visualizingDetection)
        {
            StartCoroutine(VisualizeDetectionRange(2.5f));
        }
    }

    private IEnumerator VisualizeDetectionRange(float time)
    {
        visualizingDetection = true;

        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            detectionRangeVisualizer.transform.localScale = new Vector3(Mathf.Lerp(0, sightVisualizationMaxSize, elapsedTime / time), Mathf.Lerp(0, sightVisualizationMaxSize, elapsedTime / time), 0);
            yield return null;
        }

        elapsedTime = 0f;

        yield return new WaitForSeconds(1f);

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            detectionRangeVisualizer.transform.localScale = new Vector3(Mathf.Lerp(sightVisualizationMaxSize, 0, elapsedTime / time), Mathf.Lerp(sightVisualizationMaxSize, 0, elapsedTime / time), 0);
            yield return null;
        }

        visualizingDetection = false;
    }

    public bool LookForNearbyPlayer(Vector2 playerLocation)
    {
        if (Vector2.Distance(enemyLocation, playerLocation) <= sightRange)
        {
            Debug.Log("In Enemy Sight, distance meter");
            return true;
        }
        else
        {
            return false;
        }
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(transform.position, (sightRange*1.28f));
    //}
}
