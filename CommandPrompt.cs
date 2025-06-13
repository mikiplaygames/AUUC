using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MikiHeadDev.Core.Input;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MikiHeadDev.Helpers.Commands{
public class CommandPrompt : MonoBehaviour
{
    [SerializeField] string commandsClassType;
    private Canvas canvas;
    private Control control;
    private InputField commandPrompt;
    private Text oldPrompts;
    private List<string> promptArgs;
    public List<string> commandsList;
    private int lastPromptIndex = 0;
    private int tabCommandIndex = 0;
    private string orginalCommand = "";
    private Type myType;
    private List<MethodInfo> methods;
    private List<string> executionHistory;
#region UNITY CODE
    private void Awake() {
        transform.position = Vector3.zero;
        control = new();
        commandsList = new();
        executionHistory = new();
        myType = Type.GetType(commandsClassType);
        methods = myType.GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly).ToList();
        methods.ForEach(x => commandsList.Add(x.Name));
        control.HAX.CommandPrompt.performed += ToggleCommandPrompt;
    }
    private void InitCanvas()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.enabled = false;
        canvas.sortingOrder = 9999;
        var canvasScaler = gameObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.matchWidthOrHeight = 0;
        canvasScaler.referencePixelsPerUnit = 100;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.gameObject.AddComponent<GraphicRaycaster>();
    }
    private void InitFields()
    {
        commandPrompt = new GameObject("CommandPrompt", typeof(RectTransform), typeof(InputField)).GetComponent<InputField>();
        commandPrompt.transform.SetParent(transform);
        commandPrompt.characterLimit = 50;
        commandPrompt.onSubmit.AddListener(SubmitPrompt);
        commandPrompt.onValueChanged.AddListener(ChangedPrompt);
        var promptRect = commandPrompt.transform as RectTransform;
        promptRect.sizeDelta = new Vector2(1920, 50);
        promptRect.localPosition = new(0,promptRect.sizeDelta.y/2,0);
        promptRect.anchorMax = new(0.5f,0);
        promptRect.anchorMin = new(0.5f,0);
        var image = commandPrompt.transform.AddComponent<Image>();
        image.type = Image.Type.Sliced;
        commandPrompt.image = image;
        var text = new GameObject("CommandPromptText", typeof(Text)).GetComponent<Text>();
        text.rectTransform.SetParent(commandPrompt.transform);
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.localPosition = Vector3.zero;
        text.rectTransform.sizeDelta = Vector2.zero;
        text.text = ">type command";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.supportRichText = false;
        text.fontSize = 40;
        text.fontStyle = FontStyle.Italic;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleLeft;
        commandPrompt.textComponent = text;
        oldPrompts = new GameObject("OldPromptsText", typeof(Text)).GetComponent<Text>();
        oldPrompts.rectTransform.SetParent(transform);
        oldPrompts.font = text.font;
        oldPrompts.supportRichText = false;
        oldPrompts.fontSize = 40;
        oldPrompts.color = Color.white;
        oldPrompts.alignment = TextAnchor.LowerLeft;
        oldPrompts.rectTransform.sizeDelta = new Vector2(image.rectTransform.sizeDelta.x, 1080 - (image.rectTransform.sizeDelta.y * 2));
        var scroll = oldPrompts.AddComponent<ScrollRect>();
        scroll.content = oldPrompts.rectTransform;
        scroll.scrollSensitivity = 5;
        scroll.horizontal = false;

        control.HAX.CommandTab.performed += CheckForCommandFill;
        control.HAX.CommandScroll.performed += ScrollPrompts;
    }
    private void OnEnable() {
        control.Enable();
    }
    private void OnDisable() {
        control.Disable();
    }
    private void ScrollPrompts(InputAction.CallbackContext context)
    {
        if (canvas != null && !canvas.enabled && commandPrompt != null)
            return;
        
        lastPromptIndex -= (int)control.HAX.CommandScroll.ReadValue<float>();
        
        if (lastPromptIndex < 0)
            lastPromptIndex = executionHistory.Count - 1;
        else if (lastPromptIndex >= executionHistory.Count)
            lastPromptIndex = 0;
        
        commandPrompt.text = executionHistory[lastPromptIndex];
        commandPrompt.caretPosition = commandPrompt.text.Length-1;
        
        commandPrompt.Select();
        commandPrompt.ActivateInputField();
    }
    private void ChangedPrompt(string arg0)
    {
        if (control.HAX.CommandTab.triggered) return;
        orginalCommand = commandPrompt.text;
        tabCommandIndex = 0;
    }
    private void CheckForCommandFill(InputAction.CallbackContext context)
    {
        if (canvas == null || !canvas.enabled || commandPrompt == null)
            return;
        var possibleCommands = commandsList.Where(x => x.ToLower().Contains(orginalCommand.ToLower())).ToList();
        possibleCommands.Add(orginalCommand);
        if (possibleCommands.Count == 0)
        {
            tabCommandIndex = 0;
            return;
        }
        commandPrompt.text = possibleCommands[tabCommandIndex];
        commandPrompt.caretPosition = commandPrompt.text.Length;
        tabCommandIndex = tabCommandIndex < possibleCommands.Count - 1 ? tabCommandIndex + 1 : 0;
    }
    private void ToggleCommandPrompt(InputAction.CallbackContext context)
    {
        BabushkaMovement.Instance.enabled = !BabushkaMovement.Instance.enabled; //stop player movement
        
        if (canvas == null)
        {
            InitCanvas();
            InitFields();
        }
        
        canvas.enabled = !canvas.enabled;
        commandPrompt.gameObject.SetActive(canvas.enabled);
        
        if (canvas.enabled)
        {
            commandPrompt.Select();
            commandPrompt.ActivateInputField();
        }
    }
    private void SubmitPrompt(string prompt)
    {
        if (!canvas.enabled)
            return;
        WriteLine(prompt);
        lastPromptIndex = 0;
        
        executionHistory.Add(prompt);
        RunCommand(prompt);

        commandPrompt.text = "";
        commandPrompt.Select();
        commandPrompt.ActivateInputField();
    }
    private void RunCommand(string prompt)
    {
        promptArgs = prompt.Split(" ").ToList();
        MethodInfo method = myType.GetMethod(promptArgs[0]);
        if (method == null)
        {
            WriteLine($"Command {promptArgs[0]} not found");
            return;
        }
        promptArgs.RemoveAt(0);

        List<object> arguments = new();
        ParameterInfo[] parameters = method.GetParameters();

        for (int i = 0; i < parameters.Length; i++)
        {
            arguments.Add(i < promptArgs.Count ? Convert.ChangeType(promptArgs[i], parameters[i].ParameterType) : parameters[i].DefaultValue);
        }
        method.Invoke(this, arguments.ToArray());
    }
    private void WriteLine(string text) {
        Write($"\n{text}");
    }
    private void Write(string text)
    {
        oldPrompts.text += text;
        if (oldPrompts.text.Count(x => x == '\n') > 50)
            oldPrompts.text = oldPrompts.text[(oldPrompts.text.IndexOf('\n') + 1)..];
    }
    public void Help()
    {
        string args = "";
        int i = 0;
        ParameterInfo[] parameters;
        foreach (var item in methods)
        {
            parameters = item.GetParameters();
            args = "";
            for (i = 0; i < parameters.Length; i++)
            { 
                args += $"[{parameters[i].Name}: ({parameters[i].ParameterType.ToString()[7..]})] ";
            }
            WriteLine($"{item.Name} {args}");
        }
    }
    public void LoadScene(string sceneName)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName).buildIndex == -1)
        {
            WriteLine($"Scene {sceneName} not found");
            return;
        }
        WriteLine($"Loading {sceneName} scene");
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    public void LimitFps(int fps = 60)
    {
        Application.targetFrameRate = fps;
        WriteLine($"Set target fps to {fps}");
    }
    public void Vsync(int vsync = 1)
    {
        QualitySettings.vSyncCount = vsync;
        WriteLine($"Set VSync to {vsync}");
    }
    public void SetUnityTimeScale(float scale = 1)
    {
        Time.timeScale = scale;
        WriteLine($"Set unity time scale to {scale}");
    }
#endregion
}
}
