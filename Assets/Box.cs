using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class Box : MonoBehaviour
{
    private Animator animator;
    public Transform start;
    public Transform end;
    public GameObject paperPrefab;
    public InputField maxInputField;
    float moveDuration = 2.6f;
    int paperNumber;
    int max;

    private GameObject paperInstance;
    AudioSource audioSource;
    public AudioClip treasureSound;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (maxInputField != null)
        {
            maxInputField.onEndEdit.AddListener(SetMaxValue);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (paperInstance == null)
            {
                StartCoroutine(PlayAnimationAndSpawnPaper());
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetPaperAndAnimation();
        }
    }

    IEnumerator PlayAnimationAndSpawnPaper()
    {
        if (animator != null)
        {
            animator.SetTrigger("Open");
        }

        yield return new WaitForSeconds(0.7f);

        audioSource.PlayOneShot(treasureSound);

        yield return new WaitForSeconds(1.1f);

        if (paperPrefab != null && start != null)
        {
            paperNumber = Random.Range(1, max + 1);

            paperInstance = Instantiate(paperPrefab, start.position, start.rotation);
            StartCoroutine(MovePaper());
        }
    }

    IEnumerator MovePaper()
    {
        float timer = 0f;
        float maxRotationAngle = 180f;
        float rotationSpeedFactor = 0.2f;

        Quaternion startRotation = Quaternion.Euler(18.882f, 0f, 0f);
        Quaternion endRotation = Quaternion.Euler(18.882f, 0f, 0f);

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / moveDuration;

            Vector3 position = Vector3.Lerp(start.position, end.position, t);
            paperInstance.transform.position = position;

            float randomX = Mathf.Sin(timer * rotationSpeedFactor) * maxRotationAngle * (1 - t);
            float randomY = Mathf.Cos(timer * rotationSpeedFactor) * maxRotationAngle * (1 - t);
            float randomZ = Mathf.Sin(timer * rotationSpeedFactor * 0.5f) * maxRotationAngle * (1 - t);
            Quaternion randomRotation = Quaternion.Euler(randomX, randomY, randomZ);

            Quaternion interpolatedRotation = Quaternion.Lerp(startRotation, endRotation, t);
            paperInstance.transform.rotation = randomRotation;

            yield return null;
        }

        paperInstance.transform.rotation = endRotation;

        AddTextToPaper(paperInstance, paperNumber);
    }

    void AddTextToPaper(GameObject paper, int number)
    {
        GameObject textObject = new GameObject("PaperText");
        textObject.transform.SetParent(paper.transform);
        textObject.transform.localPosition = Vector3.forward * -0.1f;
        textObject.transform.localRotation = Quaternion.identity;

        TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();
        textMesh.text = number.ToString();
        textMesh.fontSize = 5;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.color = Color.black;
    }

    void ResetPaperAndAnimation()
    {
        if (paperInstance != null)
        {
            Destroy(paperInstance);
            paperInstance = null;
        }

        if (animator != null)
        {
            animator.SetTrigger("Reset");
        }
    }

    void SetMaxValue(string value)
    {
        if (int.TryParse(value, out int result) && result > 0)
        {
            max = result;

            if (maxInputField != null)
            {
                maxInputField.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("유효한 숫자를 입력하세요.");
        }
    }
}