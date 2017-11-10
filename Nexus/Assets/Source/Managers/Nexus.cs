using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Analytics;
using UnityEngine.Purchasing;

namespace NexusManager
{
    /// <summary>
    /// Manager that holds references to the different managers
    /// </summary>
    /// <remarks>
    /// Author Tyler Furness, March 2017
    /// </remarks>
    public class Nexus : MonoBehaviour
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

        [SerializeField]
        private List<AnalyticsProvider> _analyticsProviders = null;

        /// <summary>
        /// Make sure we have a single instance of this class, if one wasn't created ahead of time then make one and return the instance
        /// </summary>
        private static Nexus _instance = null;
        public static Nexus Instance
        {
            get
            {
                _instance = (Nexus)FindObjectOfType(typeof(Nexus));
                if (_instance == null && !_isDestroying)
                {
                    _instance = (new GameObject("NexusManager")).AddComponent<Nexus>();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            _iapManager = new IAPManager();
            _adManager = new ADManager();
            _analyticManager = new AnalyticManager();
            _analyticManager.Initialize(_analyticsProviders);
        }

        private void OnDestroy()
        {
            _isDestroying = true;
        }
    }
}