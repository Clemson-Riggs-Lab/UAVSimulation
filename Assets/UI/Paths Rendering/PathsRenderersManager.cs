using System.Collections.Generic;
using Modules.Navigation;
using Modules.Navigation.Channels.ScriptableObjects;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UI.Paths_Rendering.Settings.ScriptableObjects;
using UnityEngine;
using static HelperScripts.Enums;
using static HelperScripts.Enums.UavCondition;

namespace UI.Paths_Rendering
{
	public class PathsRenderersManager:MonoBehaviour
	{
		private UavEventChannelSO _uavCreatedEventChannel;
		private UavEventChannelSO _uavDestroyedEventChannel;
		private UavPathEventChannelSO _uavStartedNewPathEventChannel;
		private UavPathEventChannelSO _uavArrivedAtDestinationEventChannel;
		private UavPathEventChannelSO _uavReroutedEventChannel;
		private UavPathEventChannelSO _uavReroutePreviewEventChannel;
		private UavConditionEventChannelSO _uavConditionChangedEventChannel;
		private PathsRenderingSettingsSO _pathsRenderingSettings;
		private GameObject _uavPathRendererPrefab;
		private Dictionary<Uav, PathRenderer> _uavPathRenderers=new();
		
		private void Start()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
		}

		private void PreviewPath(Uav uav, Path path)
		{
			if (!_uavPathRenderers.ContainsKey(uav)) return;
			if (uav.currentPath == path && path!=null)
			{
				_uavPathRenderers[uav].SetLineRenderer(path,false); // sending the current path as a preview path means that we want to cancel the preview
			}
			
			
			_uavPathRenderers[uav].SetLineRenderer(path,true);
		}

		private void RenderNewPath(Uav uav, Path path)
		{
			if (_uavPathRenderers.ContainsKey(uav))
			{
				_uavPathRenderers[uav].SetLineRenderer(path,false);
			}
		}

		private void OnUavDestroyed(Uav uav)
		{
			//search for pathrenderer in dictionary, and destroy it if found
			if (_uavPathRenderers.ContainsKey(uav))
			{
				Destroy(_uavPathRenderers[uav].gameObject);
				_uavPathRenderers.Remove(uav);
			}
		}

		private void OnUavCreated(Uav uav)
		{
			var pathRendererGo = Instantiate(_uavPathRendererPrefab, uav.transform);
			pathRendererGo.gameObject.layer = LayerMask.NameToLayer("LineRenderers");
			var pathRenderer = pathRendererGo.GetComponent<PathRenderer>();
			pathRenderer.Initialize(uav);
			_uavPathRenderers.Add(uav, pathRenderer);
		}
		

		private void OnUavConditionChanged(Uav uav, UavCondition condition) // hide the line rendered if the uav is lost or hidden and the settings instruct to hide the render when the uav is hidden
		{
			if (!_uavPathRenderers.ContainsKey(uav)) return;
			
			if (condition is  Lost || ((condition is Hidden or EnabledForTargetDetectionOnly) && !_pathsRenderingSettings.showPathWhenUavIsHidden))
				_uavPathRenderers[uav].ShowLineRenderers(false);
			else
				_uavPathRenderers[uav].ShowLineRenderers(true);
		}

		private void OnDestroy()
		{
			UnsubscribeFromChannels();
		}

		private void SubscribeToChannels()
		{
			if(_uavCreatedEventChannel != null)
				_uavCreatedEventChannel.Subscribe(OnUavCreated);
			
			if(_uavDestroyedEventChannel != null)
				_uavDestroyedEventChannel.Subscribe(OnUavDestroyed);
			
			if(_uavStartedNewPathEventChannel != null)
				_uavStartedNewPathEventChannel.Subscribe(RenderNewPath);
			
			if(_uavArrivedAtDestinationEventChannel != null)
				_uavArrivedAtDestinationEventChannel.Subscribe(RenderNewPath);
			
			if(_uavReroutedEventChannel != null)
				_uavReroutedEventChannel.Subscribe(RenderNewPath);
			
			if(_uavReroutePreviewEventChannel != null)
				_uavReroutePreviewEventChannel.Subscribe(PreviewPath);
			
			if(_uavConditionChangedEventChannel != null)
				_uavConditionChangedEventChannel.Subscribe(OnUavConditionChanged);
			
		
		}
		private void UnsubscribeFromChannels()
		{
			if(_uavCreatedEventChannel != null)
				_uavCreatedEventChannel.Unsubscribe(OnUavCreated);
			
			if(_uavDestroyedEventChannel != null)
				_uavDestroyedEventChannel.Unsubscribe(OnUavDestroyed);
			
			if(_uavStartedNewPathEventChannel != null)
				_uavStartedNewPathEventChannel.Unsubscribe(RenderNewPath);
			
			if(_uavArrivedAtDestinationEventChannel != null)
				_uavArrivedAtDestinationEventChannel.Unsubscribe(RenderNewPath);
			
			if(_uavReroutedEventChannel != null)
				_uavReroutedEventChannel.Unsubscribe(RenderNewPath);
			
			if(_uavReroutePreviewEventChannel != null)
				_uavReroutePreviewEventChannel.Unsubscribe(PreviewPath);
			
		}
		
		private void GetReferencesFromGameManager()
		{
			_uavStartedNewPathEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavStartedNewPathEventChannel;
			_uavArrivedAtDestinationEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavArrivedAtDestinationEventChannel;
			_uavCreatedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			_uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			_uavReroutedEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavReroutedEventChannel;
			_uavReroutePreviewEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavReroutePreviewEventChannel;
			_uavPathRendererPrefab = GameManager.Instance.prefabsDatabase.uavPathRendererPrefab;
			_pathsRenderingSettings = GameManager.Instance.settingsDatabase.pathsRenderingSettings;
			_uavConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;
		}
	}
}