using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Analytics;
using UnityEngine.Purchasing;

/// <summary>
/// Manager that holds references to the different managers
/// </summary>
/// <remarks>
/// Author Tyler Furness, March 2017
/// </remarks>
public class NexusManager : MonoBehaviour
{
    #region Variables
    
    private static bool _isDestroying = false;

    private IAPManager _iapManager;
    public IAPManager IAPManager { get { return _iapManager; } }

    private ADManager _adManager;
    public ADManager ADManager { get { return _adManager; } }

    private AnalyticManager _analyticManager;
    public AnalyticManager AnalyticManager { get { return _analyticManager; } }

    #endregion

    /// <summary>
    /// Make sure we have a single instance of this class, if one wasn't created ahead of time then make one and return the instance
    /// </summary>
    private static NexusManager _instance = null;
    public static NexusManager Instance
    {
        get
        {
            _instance = (NexusManager)FindObjectOfType(typeof(NexusManager));
            if (_instance == null && !_isDestroying)
            {
                _instance = (new GameObject("NexusManager")).AddComponent<NexusManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _iapManager = new IAPManager();
        _adManager = new ADManager();
        _analyticManager = new AnalyticManager();
    }
    
    private void OnDestroy()
    {
        _isDestroying = true;
    }
}
