using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class ToggleSettingsController : MonoBehaviour
{
    [SerializeField] private InputActionReference ToggleSettingsAction;
    [SerializeField] private GameObject settingsMenu;

    // Update is called once per frame
    public void ToggleSettings(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool isActive = settingsMenu.activeSelf;
            settingsMenu.SetActive(!isActive);
        }
    }
}
