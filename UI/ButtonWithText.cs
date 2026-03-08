using UnityEngine;
using UnityEngine.UI;
[AddComponentMenu("UI/Button With Text")]
public class ButtonWithText : Button {
    [SerializeField] TMPro.TextMeshProUGUI buttonText;
    public void SetText(string text) => buttonText.SetText(text);
}