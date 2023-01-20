using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PopupHelperEditor
{
    [MenuItem("Window/Popup")]
    public static void Show()
    {
        NativePopupHelper.ShowTestPopup();
    }
}
