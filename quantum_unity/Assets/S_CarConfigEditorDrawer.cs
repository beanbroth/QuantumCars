using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class S_CarConfigEditorDrawer : MonoBehaviour
{
    public VehicleController3DConfigAsset _vehicleController3DConfigAsset;
    public List<GameObject> wheelPositions;

    private void OnDrawGizmos()
    {
        if (_vehicleController3DConfigAsset == null)
        {
            return;
        }

        DrawRideHeightGizmo();
    }

    private void DrawRideHeightGizmo()
    {
        if (wheelPositions == null || wheelPositions.Count == 0)
        {
            return;
        }

        Gizmos.color = Color.green;
        foreach (var wheelPosition in wheelPositions)
        {
            if (wheelPosition != null)
            {
                // Use the object's downward vector
                Vector3 downwardVector = transform.TransformDirection(Vector3.down);
                Vector3 endPosition = wheelPosition.transform.position + downwardVector * _vehicleController3DConfigAsset.Settings.rideHeight.AsFloat;

                Gizmos.DrawLine(wheelPosition.transform.position, endPosition);
                Gizmos.DrawWireSphere(endPosition, 0.5f);
            }
        }
    }
    
}