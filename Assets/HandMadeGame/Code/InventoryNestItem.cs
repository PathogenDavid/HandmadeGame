using UnityEngine;

public sealed class InventoryNestItem : MonoBehaviour
{
    private void Start()
        => transform.rotation = Quaternion.Euler(0f, 180f, -20f);

    private void Update()
        => transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * 50f, Vector3.up);
}
