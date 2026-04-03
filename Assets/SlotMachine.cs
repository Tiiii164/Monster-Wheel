using UnityEngine;
using UnityEngine.UI;
using System.Collections;


[System.Serializable]
public class StripBorderMapping
{
    public int stripIndex;
    public int[] borderIndices;
}

public class SlotMachine : MonoBehaviour
{
    [SerializeField] private RectTransform stripRect;
    [SerializeField] private RectTransform iconRect;
    [SerializeField] private float spinSpeed = 800f;
    [SerializeField] private float slowdownRate = 50f;
    [SerializeField] private int loopRange;

    [SerializeField] private StripBorderMapping[] stripToBorderMap;


    private bool spinning = false;
    private bool stopping = false;
    private float currentSpeed;
    private Sprite result;
    private BorderLightController borderLightController;

    private void Start()
    {
        borderLightController = FindObjectOfType<BorderLightController>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!spinning)
                StartSpin();
            else if (!stopping)
                StopSpin();
        }

        if (spinning)
        {
            stripRect.localPosition += Vector3.up * currentSpeed * Time.deltaTime;

            if (stripRect.localPosition.y > loopRange)
                stripRect.localPosition = new Vector3(stripRect.localPosition.x, 0f, 0f);
        }
    }

    void StartSpin()
    {
        spinning = true;
        stopping = false;
        currentSpeed = spinSpeed;
        if (borderLightController != null)
            borderLightController.StartLight();
    }

    void StopSpin()
    {
        stopping = true;
        StartCoroutine(SlowDown());
    }

    IEnumerator SlowDown()
    {
        while (currentSpeed > 50f)
        {
            currentSpeed -= slowdownRate;
            yield return new WaitForSeconds(0.1f);
        }

        spinning = false;
        stopping = false;

        float y = Mathf.Round(stripRect.localPosition.y / iconRect.rect.height) * iconRect.rect.height;
        stripRect.localPosition = new Vector3(stripRect.localPosition.x, y, 0f);

        Image[] icons = stripRect.GetComponentsInChildren<Image>();
        int currentIndex = GetCurrentIndex();
        result = icons[currentIndex].sprite;

        StripBorderMapping mapping = stripToBorderMap[currentIndex];
        int[] possibleBorders = mapping.borderIndices;

        int borderIndex = possibleBorders[Random.Range(0, possibleBorders.Length)];
        borderLightController.StopLightAt(borderIndex);

        Debug.Log("Kết quả: " + result.name + ", Border: " + borderIndex);

        if (borderLightController != null)
            borderLightController.StopLightAt(currentIndex);
    }

    int GetCurrentIndex()
    {
        float y = stripRect.localPosition.y;
        int index = Mathf.RoundToInt(y / iconRect.rect.height) % stripRect.childCount;
        return Mathf.Abs(index);
    }
}
