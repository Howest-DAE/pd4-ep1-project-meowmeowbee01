using Assets.Scripts.Dodgeball.Network;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Dodgeball.Presenter.GameUI.TeamSelection
{
	[RequireComponent(typeof(UIDocument))]
	public class TeamSelectionUI : MonoBehaviour
	{
		[SerializeField]
		private GameUIPresenter _gameUIPresenter;

		[SerializeField]
		private SelectTeamSync _sync;

		private SelectTeamPresenter _teamSelection;

		private void Start()
		{
			var uiDoc = GetComponent<UIDocument>();
			_teamSelection = new SelectTeamPresenter(uiDoc, _gameUIPresenter.Model.TeamSelection, _sync);
			_teamSelection.RegisterCallbacks();
		}
		private void OnEnable()
		{
			_teamSelection?.RegisterCallbacks();
		}
		private void OnDisable()
		{
			_teamSelection?.UnregisterCallbacks();
		}
	}
}