using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    private GameObject text;
    [SerializeField]
    private GameObject checkPoint;

    private Renderer checkpointRenderer;
    private Material checkpointMaterial;

    private void Start()
    {
        checkpointRenderer = checkPoint.GetComponent<Renderer>();
        checkpointMaterial = checkpointRenderer.material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Checkpoint")
        {
            text.SetActive(true);

            StartCoroutine(DisableCheckPoint());
        }
    }

    private IEnumerator DisableCheckPoint()
    {
        yield return new WaitForSeconds(3f);

        /*float fadeDuration = 1f;
        float elapsedTime = 0f;

        Color originalColor = checkpointMaterial.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            Color newColor = originalColor;
            newColor.a = alpha;

            checkpointMaterial.color = newColor;

            yield return null;
        }*/

        checkPoint.SetActive(false);
    }
}
