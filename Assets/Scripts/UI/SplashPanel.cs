using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashPanel : MonoBehaviour
{
    [SerializeField] private Button buttonSetProfile;
    [SerializeField] private TMP_InputField inputProfile;

    [SerializeField] private string levelOne = "";

    private void Start()
    {
        inputProfile.onValueChanged.AddListener(OnProfileInputChange);
        buttonSetProfile.onClick.AddListener(StartGame);
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
        AuthenticationService.Instance.SignOut();
        AuthenticationService.Instance.SwitchProfile(inputProfile.text);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        await AuthenticationService.Instance.UpdatePlayerNameAsync(inputProfile.text);

        Debug.Log($"Signed in with name: {AuthenticationService.Instance.PlayerName}");
    }
}
