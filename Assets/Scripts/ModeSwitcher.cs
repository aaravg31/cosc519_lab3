using UnityEngine;
using UnityEngine.UI;

public class ModeSwitcher : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button appleModeButton;
    public Button treeModeButton;

    [Header("Mode Objects")]
    public GameObject appleLauncherObject;  // The object that has AppleLauncher.cs
    public GameObject tapToPlaceTreeObject; // The object that has TapToPlaceTree.cs

    [Header("Button Colors")]
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.white;

    private enum Mode { Apple, Tree }
    private Mode currentMode = Mode.Apple;

    private void Start()
    {
        appleModeButton.onClick.AddListener(() => SetMode(Mode.Apple));
        treeModeButton.onClick.AddListener(() => SetMode(Mode.Tree));

        // Set initial mode
        SetMode(Mode.Apple);
    }

    private void SetMode(Mode mode)
    {
        currentMode = mode;

        bool appleActive = (mode == Mode.Apple);
        bool treeActive = (mode == Mode.Tree);

        // Toggle objects
        if (appleLauncherObject != null) appleLauncherObject.SetActive(appleActive);
        if (tapToPlaceTreeObject != null) tapToPlaceTreeObject.SetActive(treeActive);

        // Update button colors
        UpdateButtonColors();
    }

    private void UpdateButtonColors()
    {
        ColorBlock appleColors = appleModeButton.colors;
        ColorBlock treeColors = treeModeButton.colors;

        // Change the "normalColor" of the button
        appleColors.normalColor = (currentMode == Mode.Apple) ? activeColor : inactiveColor;
        treeColors.normalColor = (currentMode == Mode.Tree) ? activeColor : inactiveColor;

        appleModeButton.colors = appleColors;
        treeModeButton.colors = treeColors;
    }
}