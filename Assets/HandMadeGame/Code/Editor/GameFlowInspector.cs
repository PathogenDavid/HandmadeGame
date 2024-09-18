using UnityEditor;

[CustomEditor(typeof(GameFlow))]
public sealed class GameFlowInspector : Editor
{
    private new GameFlow target => (GameFlow)base.target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Inventory slots:");
        foreach (NestItem item in target.Inventory)
            EditorGUILayout.LabelField($"* {(item != null ? item.name : "<empty>")}");
    }
}
