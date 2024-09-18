using UnityEditor;
using UnityEngine;

public static class MiscTools
{
    [MenuItem("Birdecorator/Center visual")]
    private static void CenterVisual()
    {
        GameObject selection = Selection.activeGameObject;
        Renderer renderer = selection.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("Selected object does not appear to be a visual.");
            return;
        }

        Transform transform = selection.transform;
        Transform parent = transform.parent;

        Vector3 oldParentPosition = parent.localPosition;
        parent.position = Vector3.zero;
        transform.position = Vector3.zero;
        
        Bounds bounds = renderer.bounds;
        transform.position = -bounds.center;

        parent.localPosition = oldParentPosition;
    }
}
