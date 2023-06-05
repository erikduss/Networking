using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonChestController : MonoBehaviour
{
    [SerializeField] public int sightRange;
    public Vector2 chestLocation;

    private GameObject detectionRangeVisualizer;

    private bool visualizingDetection = false;
    private float sightVisualizationMaxSize;

    // Start is called before the first frame update
    void Start()
    {
        chestLocation = new Vector2(transform.position.x / 1.28f, transform.position.y / 1.28f);

        detectionRangeVisualizer = transform.GetChild(0).gameObject;

        //adjust the visual indicator to match the detection range
        detectionRangeVisualizer.transform.localScale = new Vector3(0, 0, 0);
        sightVisualizationMaxSize = 2.56f * sightRange;
    }

    // Update is called once per frame
    void Update()
    {
        if (!visualizingDetection)
        {
            StartCoroutine(VisualizeDetectionRange(2.5f));
        }
    }

    public void UpdateSight()
    {
        sightVisualizationMaxSize = 2.56f * sightRange;
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
        if (Vector2.Distance(chestLocation, playerLocation) <= sightRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
