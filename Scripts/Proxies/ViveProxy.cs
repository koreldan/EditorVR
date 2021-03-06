﻿using UnityEngine.VR.Utilities;

namespace UnityEngine.VR.Proxies
{
	public class ViveProxy : TwoHandedProxyBase
	{
		private ViveInputToEvents m_InputToEvents;

		private SteamVR_RenderModel m_RightModel;
		private SteamVR_RenderModel m_LeftModel;

		public override bool active
		{
			get
			{
				return m_InputToEvents.active;
			}
		}

		public override void Awake()
		{
			base.Awake();
			m_InputToEvents = U.Object.AddComponent<ViveInputToEvents>(gameObject);
		}

		public override void Start()
		{
			base.Start();
			SteamVR_Render.instance.transform.parent = gameObject.transform;
			m_LeftModel = m_LeftHand.GetComponentInChildren<SteamVR_RenderModel>(); // TODO: AddComponent at runtime and remove it from the prefab (requires the steam device model loading to work properly in editor)            
			m_RightModel = m_RightHand.GetComponentInChildren<SteamVR_RenderModel>();
		}

		public override void Update()
		{
			//If proxy is not mapped to a physical input device, check if one has been assigned
			if ((int)m_LeftModel.index == -1 && m_InputToEvents.steamDevice[0] != -1)
			{
				// HACK set device index individually instead of calling SetDeviceIndex because loading device mesh dynamically does not work in editor. Prefab has Model Override set and mesh generated, calling SetDeviceIndex clears the model.
				m_LeftModel.index = (SteamVR_TrackedObject.EIndex) m_InputToEvents.steamDevice[0];
			}
			if ((int)m_RightModel.index == -1 && m_InputToEvents.steamDevice[1] != -1)
			{
				m_RightModel.index = (SteamVR_TrackedObject.EIndex) m_InputToEvents.steamDevice[1];
			}

			base.Update();
		}
	}
}