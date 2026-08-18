using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SwingIndicatorView : IView
{
    readonly VisualTreeAsset _template;
    readonly VisualElement _root;
    readonly Dictionary<Transform, VisualElement> _active = new();

    const float MinScale = 0.6f;
    const float MaxScale = 1.15f;

    public SwingIndicatorView(VisualElement root, VisualTreeAsset template)
    {
        _root = root;
        _template = template;
    }

    public void Sync(IReadOnlyList<SwingAnchorCandidate> candidates, float maxDistance, Camera cam)
    {
        var seen = new HashSet<Transform>();

        foreach (var c in candidates)
        {
            seen.Add(c.Anchor);

            if (!_active.TryGetValue(c.Anchor, out var element))
            {
                element = _template.CloneTree();
                _root.Add(element);
                _active[c.Anchor] = element;
            }

            Vector2 screenPos = RuntimePanelUtils.CameraTransformWorldToPanel(_root.panel, c.Anchor.position, cam);
            element.style.left = screenPos.x;
            element.style.top = screenPos.y;

            float t = Mathf.InverseLerp(maxDistance, 0f, c.Distance); // far=0, close=1
            float scale = Mathf.Lerp(MinScale, MaxScale, t);
            element.style.scale = new StyleScale(new Scale(new Vector3(scale, scale, 1f)));
        }

        List<Transform> stale = null;
        foreach (var kvp in _active)
            if (!seen.Contains(kvp.Key))
                (stale ??= new List<Transform>()).Add(kvp.Key);

        if (stale != null)
            foreach (var t in stale)
            {
                _active[t].RemoveFromHierarchy();
                _active.Remove(t);
            }
    }

    public void SetConfirmed(Transform anchor)
    {
        if (!_active.TryGetValue(anchor, out var element)) return;
        var status = element.Q<VisualElement>("anchorIndicator-status-icon");
        status.style.backgroundColor = new StyleColor(Color.green);
    }

    public void Clear()
    {
        foreach (var kvp in _active) kvp.Value.RemoveFromHierarchy();
        _active.Clear();
    }
}