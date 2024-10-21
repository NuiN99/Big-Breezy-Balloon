using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashPanel : MonoBehaviour
{
    [SerializeField] private Button buttonSetProfile;
    [SerializeField] private TMP_InputField inputProfile;

    [SerializeField] private string levelOne = "";

    private async void Start()
    {
        inputProfile.onValueChanged.AddListener(OnProfileInputChange);
        buttonSetProfile.onClick.AddListener(StartGame);

        await UnityServices.Instance.InitializeAsync();
        if (AuthenticationService.Instance.IsSignedIn)
            AuthenticationService.Instance.SignOut();
    }

    private void OnProfileInputChange(string playerName)
    {
        buttonSetProfile.interactable = string.IsNullOrEmpty(playerName) == false;
    }

    public async void StartGame()
    {
        await SetPlayer();
        SceneManager.LoadScene(levelOne);
    }

    private async Task SetPlayer()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            AuthenticationService.Instance.SwitchProfile(inputProfile.text);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        await AuthenticationService.Instance.UpdatePlayerNameAsync(inputProfile.text);

        Debug.Log($"Signed in with name: {AuthenticationService.Instance.PlayerName}");
    }
}
