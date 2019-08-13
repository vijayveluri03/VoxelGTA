using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{

	[System.Serializable]
	public class UIPanelInfo
	{
		public enum Type { Screen, Popup, Alert, GamePlay }

		public string panelName;
		public string objectPath;
		public Vector3 localPosition;
		public Type type;
		public bool keepInMemory = true;

		[HideInInspector]
		public UnityEngine.Object uiObjectReference;
	}

	[System.Serializable]
	public class UIPanelInstance
	{
		public UIPanelInfo panelInfo;
		public GameObject instance;
		public UIPanel uiPanel;
	}
	[System.Serializable] public class GamePlayScreenPanelInstance : UIPanelInstance { }
	[System.Serializable] public class ScreenPanelInstance : UIPanelInstance { }
	[System.Serializable] public class PopupPanelInstance : UIPanelInstance { }
	[System.Serializable] public class AlertPanelInstance : UIPanelInstance { }

	public class UIManager : MonoBehaviour
	{

		public UIPanel ShowGamePlayScreen(string screenName, bool visible)
		{
			UIPanelInstance panelInstance = GetPanelInstance(screenName);

			if (panelInstance == null)
			{
				UIPanelInfo panelInfo = GetPanelReference(screenName);
				if (panelInfo == null) QLogger.LogErrorAndThrowException("No screen found with name:" + screenName);
				if (panelInfo.type != UIPanelInfo.Type.GamePlay) QLogger.LogErrorAndThrowException("This panel is not a screen type");
				panelInstance = CreatePanelInstanceAndAddToListIfNess(panelInfo);
				if (panelInstance == null) QLogger.LogErrorAndThrowException("Panel instance was not created!");
			}
			if (!(panelInstance is GamePlayScreenPanelInstance)) QLogger.LogErrorAndThrowException("This panel is not screen type - 2");
			GamePlayScreenPanelInstance screenInstance = panelInstance as GamePlayScreenPanelInstance;

			if (!gamePlayScreenPanels.Contains(screenInstance))
				gamePlayScreenPanels.Add(screenInstance);

			panelInstance.instance.SetActive(visible);
			return panelInstance.uiPanel;
		}

		public string TopScreenName { get { if (screenPanelStack.Count > 0) return screenPanelStack.Peek().panelInfo.panelName; return null; } }
		public UIPanel PushScreen(string screenName, bool visible)
		{
			UIPanelInstance panelInstance = GetPanelInstance(screenName);

			if (panelInstance == null)
			{
				UIPanelInfo panelInfo = GetPanelReference(screenName);
				if (panelInfo == null) QLogger.LogErrorAndThrowException("No screen found with name:" + screenName);
				if (panelInfo.type != UIPanelInfo.Type.Screen) QLogger.LogErrorAndThrowException("This panel is not a screen type");
				panelInstance = CreatePanelInstanceAndAddToListIfNess(panelInfo);
				if (panelInstance == null) QLogger.LogErrorAndThrowException("Panel instance was not created!");
			}
			if (!(panelInstance is ScreenPanelInstance)) QLogger.LogErrorAndThrowException("This panel is not screen type - 2");
			ScreenPanelInstance screenInstance = panelInstance as ScreenPanelInstance;

			if (screenPanelStack.Count > 0)
			{
				screenPanelStack.Peek().uiPanel.OnPusedBack();
				screenPanelStack.Peek().instance.SetActive(false);
			}
			screenPanelStack.Push(screenInstance);
			panelInstance.instance.SetActive(visible);
			return panelInstance.uiPanel;
		}
		public UIPanel PopScreen()
		{
			if (screenPanelStack.Count > 0)
			{
				screenPanelStack.Peek().uiPanel.OnExit();
				screenPanelStack.Peek().instance.SetActive(false);
				CloseUiPanelAndRemoveFromListIfNess(screenPanelStack.Pop());
			}

			if (screenPanelStack.Count > 0)
			{
				screenPanelStack.Peek().uiPanel.OnPushedToFront();
				screenPanelStack.Peek().instance.SetActive(true);
				return screenPanelStack.Peek().uiPanel;
			}
			return null;
		}
		public UIPanel PopToScreen(string screenName)
		{
			while (screenPanelStack.Count > 0)
			{
				if (screenPanelStack.Peek().panelInfo.panelName == screenName)
					break;
				screenPanelStack.Peek().uiPanel.OnExit();
				screenPanelStack.Peek().instance.SetActive(false);
				CloseUiPanelAndRemoveFromListIfNess(screenPanelStack.Pop());
			}

			if (screenPanelStack.Count > 0)
			{
				screenPanelStack.Peek().uiPanel.OnPushedToFront();
				screenPanelStack.Peek().instance.SetActive(true);
				return screenPanelStack.Peek().uiPanel;
			}
			return null;
		}

		public void ShowLoadingScreen(bool show)
		{
			QLogger.LogError("NYI");
		}

		public UIPanelInstance GetPanelInstance(string screenName)
		{
			for (int i = 0; i < panelsInMemory.Count; i++)
			{
				if (panelsInMemory[i].panelInfo.panelName == screenName)
					return panelsInMemory[i];
			}
			return null;
		}
		public UIPanelInfo GetPanelReference(string screenName)
		{
			for (int i = 0; i < panels.Length; i++)
			{
				if (panels[i].panelName == screenName)
					return panels[i];
			}
			return null;
		}
		public UIPanelInstance CreatePanelInstanceAndAddToListIfNess(UIPanelInfo panel)
		{
			if (GetPanelInstance(panel.panelName) != null)
				return GetPanelInstance(panel.panelName);

			if (panel.uiObjectReference == null)
				panel.uiObjectReference = ResourceManager.Instance.LoadAsset<UnityEngine.Object>(panel.objectPath);

			if (panel.uiObjectReference == null) QLogger.LogErrorAndThrowException("Panel object not found!");

			GameObject instance = GameObject.Instantiate(panel.uiObjectReference, new Vector3(9999, 9999, 9999), Quaternion.identity, transform) as GameObject;
			instance.SetActive(false);
			instance.transform.localPosition = panel.localPosition;
			UIPanelInstance panelInstance = null;

			if (panel.type == UIPanelInfo.Type.Screen) panelInstance = new ScreenPanelInstance();
			else if (panel.type == UIPanelInfo.Type.Popup) panelInstance = new PopupPanelInstance();
			else if (panel.type == UIPanelInfo.Type.Alert) panelInstance = new AlertPanelInstance();
			else if (panel.type == UIPanelInfo.Type.GamePlay) panelInstance = new GamePlayScreenPanelInstance();
			else QLogger.LogErrorAndThrowException("Should not be here !");

			panelInstance.panelInfo = panel;
			panelInstance.instance = instance;
			panelInstance.uiPanel = instance.GetComponent<UIPanel>();
			if (panelInstance.uiPanel == null) QLogger.LogErrorAndThrowException("Ui panel instance not found ");
			panelsInMemory.Add(panelInstance);
			return panelInstance;
		}
		public void CloseUiPanelAndRemoveFromListIfNess(UIPanelInstance instance)
		{
			if (instance == null) { QLogger.LogErrorAndThrowException("panel instance is null"); return; }

			if (instance.panelInfo.keepInMemory)
			{
				instance.uiPanel.Reset();
				instance.instance.SetActive(false);
			}
			else
			{
				GameObject.Destroy(instance.instance);
				panelsInMemory.Remove(instance);
			}
		}

		public void Start() { }
		public void DoUpdate()
		{
			if (screenPanelStack.Count > 0)
				screenPanelStack.Peek().uiPanel.DoUpdate();
		}

		[SerializeField] private UIPanelInfo[] panels;
		private List<UIPanelInstance> panelsInMemory = new List<UIPanelInstance>();
		private Stack<ScreenPanelInstance> screenPanelStack = new Stack<ScreenPanelInstance>();
		private Queue<PopupPanelInstance> popupPanelQueue = new Queue<PopupPanelInstance>();
		private Queue<AlertPanelInstance> alertPanelQueue = new Queue<AlertPanelInstance>();
		private List<GamePlayScreenPanelInstance> gamePlayScreenPanels = new List<GamePlayScreenPanelInstance>();
	}
}