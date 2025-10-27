using UnityEngine;
using UnityEngine.UI;

public class ModeSwitcher : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button appleModeButton;
    public Button treeModeButton;

    [Header("Mode Scripts")]
    public AppleLauncher appleLauncher;       // On AppleLauncher GameObject
    public ThrowDarts throwDarts;             // On AR Camera (under XR Origin)
    public TapToPlaceTree tapToPlaceTree;     // On Tree placement GameObject

    [Header("Button Colors")]
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.white;

    private enum Mode { Apple, Tree }
    private Mode currentMode = Mode.Apple;

    private void Start()
    {
        appleModeButton.onClick.AddListener(() => SetMode(Mode.Apple));
        treeModeButton.onClick.AddListener(() => SetMode(Mode.Tree));

        SetMode(Mode.Apple); // Default
    }

    private void SetMode(Mode mode)
    {
        currentMode = mode;

        bool appleActive = (mode == Mode.Apple);
        bool treeActive = (mode == Mode.Tree);

        // ✅ Toggle scripts, not objects
        if (appleLauncher != null)
            appleLauncher.enabled = appleActive;

        if (throwDarts != null)
            throwDarts.enabled = appleActive;

        if (tapToPlaceTree != null)
            tapToPlaceTree.enabled = treeActive;

        UpdateButtonColors();
    }

    private void UpdateButtonColors()
    {
        // Instantly recolor buttons without delay
        appleModeButton.image.color = (currentMode == Mode.Apple) ? activeColor : inactiveColor;
        treeModeButton.image.color = (currentMode == Mode.Tree) ? activeColor : inactiveColor;
    }
}