using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Text")]
    public GameObject textClimb;
    public GameObject textGlide;
    public GameObject textSwing;
    public GameObject textComplete;

    [Header("Objects to disable")]
    public GameObject climb;
    public GameObject glide;
    public GameObject swing;
    public GameObject complete;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "Checkpoint Climb")
        {
            climb.SetActive(false);
            textClimb.SetActive(true);

            StartCoroutine(DisableCheckPoint(textClimb));
        }

        if (collision.gameObject.name == "Checkpoint Glide")
        {
            glide.SetActive(false);
            textGlide.SetActive(true);

            StartCoroutine(DisableCheckPoint(textGlide));
        }

        if (collision.gameObject.name == "Checkpoint Swing")
        {
            swing.SetActive(false);
            textSwing.SetActive(true);

            StartCoroutine(DisableCheckPoint(textSwing));
        }

        if (collision.gameObject.name == "Checkpoint Complete")
        {
            complete.SetActive(false);
            textComplete.SetActive(true);

            StartCoroutine(DisableCheckPoint(textComplete));
        }
    }

    private IEnumerator DisableCheckPoint(GameObject Text)
    {
        yield return new WaitForSeconds(3f);
        Text.SetActive(false);
    }
}
