using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Deterministic;
using UnityEngine;
using Quantum;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class S_CarConfigSetter : MonoBehaviour
{
    public EntityPrototype entityPrototype;
    public VehicleController3DConfigAsset _vehicleController3DConfigAsset;
    public WheelControllerConfigAsset[] _wheelControllerConfigAssets;
    public List<GameObject> wheelPositions;
    public FP _rideHeight = 2;

    [InspectorButton("ReadFromCarConfigAsset")]
    public bool _readFromCarConfigAsset;

    [InspectorButton("ApplyChangesToCarConfigAsset")]
    public bool _applyChangesToCarConfigAsset;

    private void OnTransformChildrenChanged()
    {
        UpdateWheelPositions();
    }

    public void ReadFromCarConfigAsset()
    {
        Debug.Log("S_CarConfigSetter ReadFromCarConfigAsset called.");
        FindAndSetEntityPrototype();
        FindAndSetConfigAssets();
        if (_vehicleController3DConfigAsset == null)
        {
            Debug.LogError("VehicleController3DConfigAsset is null.");
            return;
        }

        // Read rideHeight from the config asset and update the local variable
        _rideHeight = _vehicleController3DConfigAsset.Settings.rideHeight;
        
        

        // Read other properties from the config asset and update corresponding local variables
        // Example: _anotherSetting = _vehicleController3DConfigAsset.Settings.anotherSetting;
        Debug.Log("Read values from VehicleController3DConfigAsset.");
    }

    public void ApplyChangesToCarConfigAsset()
    {
        Debug.Log("S_CarConfigSetter InitializeCarConfig called.");
        FindAndSetEntityPrototype();
        //FindAndSetVehicleController3DConfigAsset();
        if (_vehicleController3DConfigAsset != null)
        {
            // CheckForChildChange();
            UpdateRigConfig();
        }
        else
        {
            Debug.LogError("VehicleController3DConfigAsset is null.");
        }
    }

    private void FindAndSetEntityPrototype()
    {
        if (entityPrototype == null)
        {
            entityPrototype = GetComponentInParent<EntityPrototype>();
            Debug.Log(entityPrototype == null
                ? "Failed to find EntityPrototype in parent."
                : "Found EntityPrototype in parent.");
        }
    }

    private void FindAndSetConfigAssets()
    {
        var controller3D = entityPrototype?
            .GetComponents<EntityComponentVehicleController3D>();
        
        if (controller3D == null || controller3D.Length == 0)
        {
            Debug.Log("No VehicleController3D found in EntityPrototype.");
            return;
        }

        AssetGuid currentRigConfigID = controller3D[0].Prototype.RigConfig.Id;
        _vehicleController3DConfigAsset = UnityDB.FindAsset<VehicleController3DConfigAsset>(currentRigConfigID);
        Debug.Log(_vehicleController3DConfigAsset == null
            ? "Failed to find VehicleController3DConfigAsset."
            : "Found VehicleController3DConfigAsset.");
        
        
    }

    private void UpdateWheelPositions()
    {
        wheelPositions.Clear();
        foreach (Transform child in transform)
        {
            wheelPositions.Add(child.gameObject);
        }
    }

    private void UpdateRigConfig()
    {
        if (_vehicleController3DConfigAsset != null)
        {
            var RigConfig = _vehicleController3DConfigAsset.Settings;
            RigConfig.rideHeight = _rideHeight;
            RigConfig.wheelLocalPositions = new FPVector3[wheelPositions.Count];
            
            for (int i = 0; i < wheelPositions.Count; i++)
            {
                RigConfig.wheelLocalPositions[i] = wheelPositions[i].transform.localPosition.ToFPVector3();
            }
            
            Debug.Log("Updated RigConfig ride height.");
        }
        else
        {
            Debug.Log("VehicleController3DConfigAsset is null.");
        }
    }
}