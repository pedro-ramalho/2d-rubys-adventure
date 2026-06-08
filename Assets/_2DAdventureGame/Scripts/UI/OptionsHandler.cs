using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class OptionsHandler : MonoBehaviour
{
    public event Action Opened;
    public event Action Closed;

    private VisualElement optionsRoot;

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        optionsRoot = root.Q<VisualElement>("OptionsRoot");
        Button backButton = root.Q<Button>("OptionsBackButton");
        backButton.clicked += Close;
    }

    public void Open()
    {
        optionsRoot.style.display = DisplayStyle.Flex;
        Opened?.Invoke();
    }

    public void Close()
    {
        optionsRoot.style.display = DisplayStyle.None;
        Closed?.Invoke();
    }
}
