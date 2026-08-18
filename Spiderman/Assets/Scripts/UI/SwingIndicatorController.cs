using UnityEngine;
using UnityEngine.UIElements;

public class SwingIndicatorController : MonoBehaviour
{
    [SerializeField] UIDocument document;
    [SerializeField] VisualTreeAsset indicatorTemplate;
    [SerializeField] SwingMovement swingMovement; // implements ISwingTargetProvider
    [SerializeField] Camera cam;
    [SerializeField] float detectionRadius = 25f;

    SwingIndicatorView view;

    void Start()
    {
        view = new SwingIndicatorView(document.rootVisualElement, indicatorTemplate);
        swingMovement.OnSwingStarted += HandleConfirmed;
    }

    void Update()
    {
        view.Sync(swingMovement.VisibleAnchors, detectionRadius, cam);
    }

    void HandleConfirmed()
    {
        if (swingMovement.CurrentAnchor != null)
            view.SetConfirmed(swingMovement.CurrentAnchor);
    }

    void OnDestroy() => swingMovement.OnSwingStarted -= HandleConfirmed;
}
