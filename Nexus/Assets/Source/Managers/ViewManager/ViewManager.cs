using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ViewType
{
    Screen,
    Modal
}

class ViewManager : MonoBehaviour
{
    private Dictionary<string, View> _viewStack = null;

    private GameObject _screenObject = null;
    private GameObject _modalObject = null;

    public void Initialize(GameObject screenObject, GameObject modalObject)
    {
        _screenObject = screenObject;
        _modalObject = modalObject;
        _viewStack = new Dictionary<string, View>();
    }

    public View PushView(string name, ViewType type)
    {
        if (_viewStack != null && _viewStack.Count > 0)
        {
            if (_viewStack.ContainsKey(name))
            {
                Logger.LogError("[ViewManager]", "View already exists");
                return null;
            }
        }
        var stackObject = new GameObject(name);

        switch (type)
        {
            case ViewType.Modal:
                stackObject.transform.SetParent(_modalObject.transform);
                break;
            case ViewType.Screen:
                stackObject.transform.SetParent(_screenObject.transform);
                break;
            default:
                break;
        }
        var rectTransform = stackObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        var stack = stackObject.AddComponent<View>();

        _viewStack.Add(name, stack as View);

        return stack as View;
    }

    public bool PopView(string name)
    {
        View view = null;

        if (!_viewStack.TryGetValue(name, out view))
            return false;

        Destroy(view.gameObject);
        _viewStack.Remove(name);

        return true;
    }
}