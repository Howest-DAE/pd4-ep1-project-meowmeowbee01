using Assets.Scripts.HttpHandlers;
using PlayFab;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Assets.PlayFab.Scripts.LoginSystem
{
	/// <summary>
	/// Main controller for the login UI system.
	/// Manages panel switching and coordinates between login and registration flows.
	/// </summary>
	public class LoginUI : MonoBehaviour
	{
		#region Fields
		private LoginPanelView _loginPanelView;
		private RegistrationPanelView _registrationPanelView;
		#endregion

		#region Lifecycle
		private void OnEnable()
		{
			InitializePanels();
			RegisterCallbacks();
			ShowLoginPanel();
		}

		private void OnDisable()
		{
			UnregisterCallbacks();
		}
		#endregion

		#region Initialization
		private void InitializePanels()
		{
			var root = GetComponent<UIDocument>().rootVisualElement;

			_loginPanelView = new LoginPanelView(root);
			_registrationPanelView = new RegistrationPanelView(root);
		}

		private void RegisterCallbacks()
		{
			_loginPanelView.OnLoginRequested += OnLoginRequested;
			_loginPanelView.OnRegisterRequested += OnLoginToRegisterRequested;
			_registrationPanelView.OnRegisterRequested += OnRegisterRequested;
			_registrationPanelView.OnBackRequested += OnRegistrationBackRequested;

			_loginPanelView.RegisterCallbacks();
			_registrationPanelView.RegisterCallbacks();
		}

		private void UnregisterCallbacks()
		{
			_loginPanelView.OnLoginRequested -= OnLoginRequested;
			_loginPanelView.OnRegisterRequested -= OnLoginToRegisterRequested;
			_registrationPanelView.OnRegisterRequested -= OnRegisterRequested;
			_registrationPanelView.OnBackRequested -= OnRegistrationBackRequested;

			_loginPanelView.UnregisterCallbacks();
			_registrationPanelView.UnregisterCallbacks();
		}
		#endregion

		#region Panel Management
		private void ShowLoginPanel()
		{
			_loginPanelView.Show();
			_registrationPanelView.Hide();
		}

		private void ShowRegistrationPanel()
		{
			_loginPanelView.Hide();
			_registrationPanelView.Show();
		}
		#endregion

		#region Event Handlers
		/// Handles login request, validates input, and triggers PlayFab login.
		private void OnLoginRequested(object sender, EventArgs e)
		{
			string email = _loginPanelView.Email;
			string password = _loginPanelView.Password;

			if (!ValidateInputs(email, password))
			{
				_loginPanelView.ShowError("Email and password required");
				return;
			}

			_loginPanelView.HideError();
			Debug.Log($"Login attempt: {email}");
			//DONE: call PlayFab Login Attempt

			PlayFabClientAPI.LoginWithEmailAddress
			(
				new()
				{
					Email = email,
					Password = password
				},
				async r =>
				{
					Debug.Log("login success");
					PlayFabPlayer.Instance.PlayfabId = r.PlayFabId;
					PlayFabPlayer.Instance.FetchDisplayName(async () => await BackendHandler.PostPlayerAsync(new() { PlayFabId = PlayFabPlayer.Instance.PlayfabId, DisplayName = PlayFabPlayer.Instance.DisplayName }));
					await SceneManager.LoadSceneAsync("LobbyScene");
				},
				e =>
				{
					Debug.LogError($"login failed: {e}");
					string message = "Register failed:";
					foreach (var pair in e.ErrorDetails) foreach (var msg in pair.Value) message += $"\n{msg}";
					_loginPanelView.ShowError(message);
				}
			);
		}

		/// Handles registration request, validates input, and triggers PlayFab registration.
		private void OnRegisterRequested(object sender, EventArgs e)
		{
			string email = _registrationPanelView.Email;
			string password = _registrationPanelView.Password;
			string displayName = _registrationPanelView.DisplayName;

			if (!ValidateInputs(email, password))
			{
				_registrationPanelView.ShowError("Username and password required");
				return;
			}

			Debug.Log($"Register attempt: {email}");
			_registrationPanelView.HideError();
			// DONE: Call PlayFab register method

			PlayFabClientAPI.RegisterPlayFabUser
			(
				new()
				{
					Email = email,
					Password = password,
					Username = displayName,
					DisplayName = displayName
				},
				r => Debug.Log($"register succes. id: {r.PlayFabId}"),
				e =>
				{
					Debug.LogError($"register failed: {e}");
					string message = "Register failed:";
					foreach (var pair in e.ErrorDetails) foreach (var msg in pair.Value) message += $"\n{msg}";
					_loginPanelView.ShowError(message);
				}
			);
		}

		//Register buttton pressed in Login panel
		private void OnLoginToRegisterRequested(object sender, EventArgs e)
		{
			ShowRegistrationPanel();
		}

		//Backbutton pressed in Register panel
		private void OnRegistrationBackRequested(object sender, EventArgs e)
		{
			ShowLoginPanel();
		}
		#endregion

		#region Validation
		private bool ValidateInputs(string username, string password)
		{
			return !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);
		}
		#endregion
	}
}