using System;
using System.Collections.Generic;
using ScriptableObjects.EventChannels;
using ScriptableObjects.UAVs.Navigation;
using Unity.VisualScripting;
using UnityEngine;

namespace UAVs.Sub_Modules.Navigation.Paths_Rendering
{
	public class PathsRenderersManager:MonoBehaviour
	{
		private UavEventChannelSO uavCreatedEventChannel;
		private UavEventChannelSO uavDestroyedEventChannel;
		private UavPathEventChannelSO uavStartedNewPathEventChannel;
		private UavPathEventChannelSO uavArrivedAtDestinationEventChannel;
		private UavPathEventChannelSO uavReroutedEventChannel;
		private UavPathEventChannelSO uavReroutePreviewEventChannel;
		private GameObject uavPathRendererPrefab;
		private Dictionary<Uav, PathRenderer> uavPathRenderers=new();
		
		private void Start()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
		}

		private void PreviewPath(Uav uav, Path path)
		{
			if (uavPathRenderers.ContainsKey(uav))
			{
				uavPathRenderers[uav].SetLineRenderer(path,true);
			}
		}

		private void RenderNewPath(Uav uav, Path path)
		{
			if (uavPathRenderers.ContainsKey(uav))
			{
				uavPathRenderers[uav].SetLineRenderer(path,false);
			}
		}

		private void OnUavDestroyed(Uav uav)
		{
			//search for pathrenderer in dictionary, and destroy it if found
			var pathRenderer = uavPathRenderers[uav];
			if(pathRenderer != null)
			{
				Destroy(pathRenderer.gameObject);
				uavPathRenderers.Remove(uav);
			}
		}

		private void OnUavCreated(Uav uav)
		{
			var pathRendererGO = Instantiate(uavPathRendererPrefab, uav.transform);
			pathRendererGO.gameObject.layer = LayerMask.NameToLayer("LineRenderers");
			var pathRenderer = pathRendererGO.GetComponent<PathRenderer>();
			pathRenderer.Initialize(uav);
			uavPathRenderers.Add(uav, pathRenderer);
		}

	

		private void OnDestroy()
		{
			UnsubscribeFromChannels();
		}

		private void SubscribeToChannels()
		{
			if(uavCreatedEventChannel != null)
				uavCreatedEventChannel.Subscribe(OnUavCreated);
			if(uavDestroyedEventChannel != null)
				uavDestroyedEventChannel.Subscribe(OnUavDestroyed);
			
			if(uavStartedNewPathEventChannel != null)
				uavStartedNewPathEventChannel.Subscribe(RenderNewPath);
			if(uavArrivedAtDestinationEventChannel != null)
				uavArrivedAtDestinationEventChannel.Subscribe(RenderNewPath);
			
			if(uavReroutedEventChannel != null)
				uavReroutedEventChannel.Subscribe(RenderNewPath);
			if(uavReroutePreviewEventChannel != null)
				uavReroutePreviewEventChannel.Subscribe(PreviewPath);
			
		
		}
		private void UnsubscribeFromChannels()
		{
			if(uavCreatedEventChannel != null)
				uavCreatedEventChannel.Unsubscribe(OnUavCreated);
			if(uavDestroyedEventChannel != null)
				uavDestroyedEventChannel.Unsubscribe(OnUavDestroyed);
			
			if(uavStartedNewPathEventChannel != null)
				uavStartedNewPathEventChannel.Unsubscribe(RenderNewPath);
			if(uavArrivedAtDestinationEventChannel != null)
				uavArrivedAtDestinationEventChannel.Unsubscribe(RenderNewPath);
			
			if(uavReroutedEventChannel != null)
				uavReroutedEventChannel.Unsubscribe(RenderNewPath);
			if(uavReroutePreviewEventChannel != null)
				uavReroutePreviewEventChannel.Unsubscribe(PreviewPath);
			
		}
		
		private void GetReferencesFromGameManager()
		{
			uavStartedNewPathEventChannel = GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.uavStartedNewPathEventChannel;
			uavArrivedAtDestinationEventChannel = GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.uavArrivedAtDestinationEventChannel;
			
			uavCreatedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			
			uavReroutedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.uavReroutedEventChannel;
			uavReroutePreviewEventChannel = GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.uavReroutePreviewEventChannel;
			
			uavPathRendererPrefab = GameManager.Instance.prefabsDatabase.uavPathRendererPrefab;
		}
	}
}