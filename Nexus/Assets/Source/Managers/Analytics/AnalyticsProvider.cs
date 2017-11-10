using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class that all Analytics Providers should inherit from. This will allow us to customize each analytics provider with how they expect their data
/// </summary>
/// <remarks>>
/// Author: Tyler Furness, July 2017
/// </remarks>
public abstract class AnalyticsProvider : MonoBehaviour
{
    /// <summary>
    /// Fires an event to the AnalyticsProvider attached.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="parameters">Parameters.</param>
    public abstract void LogMetric(string key);

    /// <summary>
    /// Fires an event to the AnalyticsProvider attached with a single param.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="param">Parameter.</param>
    public abstract void LogMetric(string key, string param);

    /// <summary>
    /// Fires an event to the AnalyticsProvider attached with a dictionary of params.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="dict">Dict.</param>
    public abstract void LogMetric(string key, Dictionary<string, string> dict);
}
