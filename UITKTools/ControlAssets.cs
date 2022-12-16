using System;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

public class ControlAssets
{
    public enum Control
    {
        BoolProp,
        TextProp,
        EnumProp,
        Dropdown,
        DropdownItem,
        Button,
        Toggle,
        Tabs,
    }
    
    public static string getControlKey(Control control)
    {
        switch (control)
        {
            case Control.BoolProp:
                return "";
            case Control.TextProp:
                return "ToolShed/UITKTools/PropertyControls/TextProp/TextProp.uxml";
            case Control.EnumProp:
                return "ToolShed/UITKTools/PropertyControls/EnumProp/EnumProp.uxml";
            case Control.Dropdown:
                return "ToolShed/UITKTools/BasicControls/Dropdown/Dropdown.uxml";
            case Control.DropdownItem:
                return "ToolShed/UITKTools/BasicControls/Dropdown/Item.uxml";
            case Control.Button:
                return "ToolShed/UITKTools/BasicControls/Button/Button.uxml";
            case Control.Toggle:
                return "ToolShed/UITKTools/BasicControls/Toggle/Toggle.uxml";
            case Control.Tabs:
                return "ToolShed/UITKTools/BasicControls/Tabs/Tabs.uxml";
            default:
                throw new ArgumentOutOfRangeException(nameof(control), control, null);
        }
    }

    public static T getAsset<T>(string key, out Action release)
    {
        T asset = Addressables.LoadAssetAsync<T>(key).WaitForCompletion();

        release = () => Addressables.Release(asset);
        return asset;
    }
    
    public static VisualTreeAsset getControl(Control control, out Action release)
    {
        VisualTreeAsset asset = getAsset<VisualTreeAsset>(getControlKey(control), out Action releaseAction);

        release = releaseAction;
        return asset;
    }
}