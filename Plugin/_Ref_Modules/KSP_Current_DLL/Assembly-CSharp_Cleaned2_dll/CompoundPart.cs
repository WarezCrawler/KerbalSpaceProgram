using System;
using CompoundParts;
using UnityEngine;

public class CompoundPart : Part
{
	public enum AttachState
	{
		Detached,
		Attaching,
		Attached
	}

	public Vector3 direction;

	public Vector3 targetPosition;

	public Quaternion targetRotation;

	public Part target;

	public string targetMeshColName;

	public float maxLength = 10f;

	public AttachState attachState;

	private RaycastHit hit;

	private bool hasSaveData;

	private uint tgtId;

	private bool needsDirectionFlip;

	private CompoundPart original;

	private CompoundPartModule[] cmpModules;

	private Vector3 wTgtPos;

	private Quaternion wTgtRot;

	private bool tweakStarted;

	private bool tweakEnded = true;

	private ICMTweakTarget tweakTargetModule;

	public bool isTweakingTarget
	{
		get
		{
			if (tweakTargetModule == null)
			{
				return false;
			}
			return tweakTargetModule.TweakingTarget;
		}
		set
		{
			if (tweakTargetModule != null)
			{
				tweakTargetModule.TweakingTarget = value;
			}
		}
	}

	protected override void onCopy(Part original, bool asSymCPart)
	{
		hasSaveData = false;
		if (symmetryCounterparts.Contains(EditorLogic.SelectedPart))
		{
			direction = Vector3.zero;
		}
		else if (asSymCPart && EditorLogic.fetch.symmetryMethod == SymmetryMethod.Mirror)
		{
			this.original = (CompoundPart)original;
			needsDirectionFlip = true;
		}
	}

	protected override void onPartAwake()
	{
		hasSaveData = false;
		cmpModules = GetComponents<CompoundPartModule>();
		ICMTweakTarget iCMTweakTarget = null;
		int count = base.Modules.Count;
		for (int i = 0; i < count; i++)
		{
			if (base.Modules[i] is ICMTweakTarget iCMTweakTarget2)
			{
				tweakTargetModule = iCMTweakTarget2;
			}
		}
		GameEvents.onEditorPartEvent.Add(OnEditorEvent);
		compund = true;
	}

	public override void OnSave(ConfigNode node)
	{
		node.AddValue("tgt", tgtId);
		node.AddValue("pos", KSPUtil.WriteVector(targetPosition));
		node.AddValue("rot", KSPUtil.WriteQuaternion(targetRotation));
		node.AddValue("dir", KSPUtil.WriteVector(direction));
		if (!string.IsNullOrEmpty(targetMeshColName))
		{
			node.AddValue("col", targetMeshColName);
		}
	}

	public override void OnLoad(ConfigNode node)
	{
		hasSaveData = node.HasData;
		node.TryGetValue("tgt", ref tgtId);
		node.TryGetValue("pos", ref targetPosition);
		node.TryGetValue("dir", ref direction);
		node.TryGetValue("rot", ref targetRotation);
		node.TryGetValue("col", ref targetMeshColName);
	}

	protected override void onStartComplete()
	{
		if (customPartData != string.Empty)
		{
			OnLoad(ParseCustomPartData(customPartData));
			customPartData = "";
		}
		attachState = AttachState.Detached;
		target = null;
		Part part = null;
		if (!hasSaveData)
		{
			return;
		}
		if (HighLogic.LoadedSceneIsFlight)
		{
			part = vessel.parts.Find((Part p) => p.craftID == tgtId && p.missionID == missionID);
		}
		if (HighLogic.LoadedSceneIsEditor)
		{
			part = EditorLogic.fetch.ship.parts.Find((Part p) => p.craftID == tgtId);
		}
		if ((bool)part)
		{
			target = part;
			SetTarget(part, targetMeshColName);
			GameEvents.onEditorCompoundPartLinked.Fire(this);
		}
		else if (HighLogic.LoadedSceneIsEditor)
		{
			if (direction != Vector3.zero)
			{
				Debug.LogWarning("[CompoundPart]: No target found with craftID " + craftID + ". Attempting to find it at direction [" + KSPUtil.WriteVector(direction) + "].", base.gameObject);
				StartCoroutine(CallbackUtil.DelayedCallback(1, schedule_raycast));
			}
		}
		else
		{
			Debug.Log("[CompoundPart]: Part: " + partName + " craftID: " + craftID + " No target found with craftID: " + tgtId);
		}
	}

	protected override void onPartAttach(Part parent)
	{
		if (EditorLogic.SelectedPart == this)
		{
			lockEditor();
			attachState = AttachState.Attaching;
			return;
		}
		attachState = AttachState.Detached;
		if (direction != Vector3.zero)
		{
			if (needsDirectionFlip)
			{
				Vector3 vector = original.transform.TransformDirection(original.direction);
				vector = new Vector3(0f - vector.x, vector.y, vector.z);
				direction = base.transform.InverseTransformDirection(vector);
				needsDirectionFlip = false;
			}
			StartCoroutine(CallbackUtil.DelayedCallback(1, schedule_raycast));
		}
	}

	protected override void onPartDetach()
	{
		if (EditorLogic.SelectedPart == this || ((bool)target && target.localRoot != base.localRoot))
		{
			DumpTarget();
			attachState = AttachState.Detached;
			if (EditorLogic.SelectedPart == this)
			{
				direction = Vector3.zero;
			}
		}
	}

	protected override void onPartDestroy()
	{
		DumpTarget();
		unlockEditor();
		GameEvents.onEditorPartEvent.Remove(OnEditorEvent);
	}

	public override void onEditorStartTweak()
	{
		if (tweakTargetModule == null)
		{
			return;
		}
		if (!tweakStarted)
		{
			if (tweakTargetModule != null)
			{
				tweakTargetModule.SelectTweakTarget(Input.mousePosition);
			}
			tweakStarted = true;
		}
		if (tweakEnded)
		{
			tweakEnded = false;
			ToggleSymmetryCounterpartsTweak(isTweakingTarget);
		}
	}

	public override void onEditorEndTweak()
	{
		isTweakingTarget = false;
		ToggleSymmetryCounterpartsTweak(toggleValue: false);
		tweakEnded = true;
		tweakStarted = false;
		Debug.LogFormat("[CompoundPart] onEditorEndTweak");
	}

	private void ToggleSymmetryCounterpartsTweak(bool toggleValue)
	{
		for (int i = 0; i < symmetryCounterparts.Count; i++)
		{
			if (symmetryCounterparts[i] != null)
			{
				CompoundPart compoundPart = symmetryCounterparts[i] as CompoundPart;
				if (compoundPart != null)
				{
					compoundPart.isTweakingTarget = toggleValue;
				}
			}
		}
	}

	public override Transform GetReferenceTransform()
	{
		if (tweakTargetModule != null)
		{
			return tweakTargetModule.GetReferenceTransform();
		}
		return partTransform;
	}

	public override Part GetReferenceParent()
	{
		if (!isTweakingTarget)
		{
			return parent;
		}
		return target;
	}

	public override void SetSymmetryValues(Vector3 newPosition, Quaternion newRotation)
	{
		if (tweakTargetModule == null || !tweakTargetModule.SetSymmetryValues(newPosition, newRotation))
		{
			base.transform.position = newPosition;
			base.transform.rotation = newRotation;
		}
	}

	public void ToggleTweakTarget(bool tweakTargetValue)
	{
		isTweakingTarget = tweakTargetValue;
	}

	public override Collider[] GetPartColliders()
	{
		if (HighLogic.LoadedSceneIsEditor && tweakTargetModule != null)
		{
			return tweakTargetModule.GetSelectedColliders();
		}
		return partTransform.Find("model").GetComponentsInChildren<Collider>();
	}

	private void onTargetDetach()
	{
		if ((bool)this)
		{
			UnsetLink();
			attachState = AttachState.Detached;
		}
	}

	private void onTargetDestroy()
	{
		if ((bool)this)
		{
			UnsetLink();
			attachState = AttachState.Detached;
		}
	}

	private void onTargetReattach()
	{
		if (!this)
		{
			return;
		}
		StartCoroutine(CallbackUtil.DelayedCallback(1, schedule_raycast));
		if (EditorLogic.fetch.symmetryMethod == SymmetryMethod.Radial)
		{
			for (int i = 0; i < symmetryCounterparts.Count; i++)
			{
				CompoundPart compoundPart = (CompoundPart)symmetryCounterparts[i];
				compoundPart.direction = direction;
				compoundPart.StartCoroutine(CallbackUtil.DelayedCallback(1, compoundPart.schedule_raycast));
			}
		}
		else if (EditorLogic.fetch.symmetryMethod == SymmetryMethod.Mirror)
		{
			Vector3 vector = base.transform.TransformDirection(direction);
			vector = new Vector3(0f - vector.x, vector.y, vector.z);
			for (int j = 0; j < symmetryCounterparts.Count; j++)
			{
				CompoundPart compoundPart2 = (CompoundPart)symmetryCounterparts[j];
				compoundPart2.direction = compoundPart2.transform.InverseTransformDirection(vector);
				compoundPart2.StartCoroutine(CallbackUtil.DelayedCallback(1, compoundPart2.schedule_raycast));
			}
		}
	}

	protected override void onEditorUpdate()
	{
		AttachState attachState = this.attachState;
		if (attachState == AttachState.Attaching)
		{
			onAttachUpdate();
		}
	}

	public override void LateUpdate()
	{
		if (HighLogic.LoadedSceneIsFlight && attachState == AttachState.Attached)
		{
			int num = cmpModules.Length;
			while (num-- > 0)
			{
				cmpModules[num].OnTargetUpdate();
			}
		}
	}

	protected override void onPartFixedUpdate()
	{
		if (attachState == AttachState.Attached && (target == null || target.vessel != vessel))
		{
			DumpTarget();
		}
	}

	private void onAttachUpdate()
	{
		if (direction != Vector3.zero)
		{
			targetPosition = base.transform.InverseTransformPoint(hit.point);
			targetRotation = Quaternion.FromToRotation(Vector3.right, base.transform.InverseTransformDirection(hit.normal));
			PreviewAttachment(direction, targetPosition, targetRotation);
			if (Input.GetMouseButtonUp(0) && Vector3.Distance(base.transform.position, hit.point) <= maxLength)
			{
				EndPreview();
				raycastTarget(direction);
				if (symMethod == SymmetryMethod.Radial)
				{
					for (int i = 0; i < symmetryCounterparts.Count; i++)
					{
						((CompoundPart)symmetryCounterparts[i]).raycastTarget(direction);
					}
				}
				else if (symMethod == SymmetryMethod.Mirror)
				{
					Vector3 vector = base.transform.TransformDirection(direction);
					vector = new Vector3(0f - vector.x, vector.y, vector.z);
					for (int j = 0; j < symmetryCounterparts.Count; j++)
					{
						CompoundPart obj = (CompoundPart)symmetryCounterparts[j];
						obj.raycastTarget(obj.transform.InverseTransformDirection(vector));
					}
				}
				if ((bool)target)
				{
					EditorLogic.fetch.ResetBackup();
					EditorLogic.fetch.GetComponent<AudioSource>().PlayOneShot(EditorLogic.fetch.attachClip);
					StartCoroutine(CallbackUtil.DelayedCallback(1, unlockEditor));
					GameEvents.onEditorCompoundPartLinked.Fire(this);
					return;
				}
			}
		}
		direction = findTargetDirection();
		if (Input.GetKeyDown(KeyCode.Delete))
		{
			DumpTarget();
			for (int k = 0; k < symmetryCounterparts.Count; k++)
			{
				((CompoundPart)symmetryCounterparts[k]).DumpTarget();
			}
			unlockEditor();
		}
	}

	private Vector3 findTargetDirection()
	{
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, LayerUtil.DefaultEquivalent) && FlightGlobals.GetPartUpwardsCached(hit.collider.gameObject) != null)
		{
			return base.transform.InverseTransformPoint(hit.point).normalized;
		}
		return Vector3.zero;
	}

	private void lockEditor()
	{
		InputLockManager.SetControlLock(ControlTypes.EDITOR_SOFT_LOCK | ControlTypes.EDITOR_EDIT_STAGES, "CompoundPart_Placement");
	}

	private void unlockEditor()
	{
		InputLockManager.RemoveControlLock("CompoundPart_Placement");
	}

	private void schedule_raycast()
	{
		raycastTarget(direction);
	}

	public bool raycastTarget(Vector3 dir)
	{
		direction = dir;
		bool result = false;
		Debug.DrawRay(base.transform.position, base.transform.TransformDirection(dir), Color.yellow, 3f);
		int layer = base.gameObject.layer;
		base.gameObject.SetLayerRecursive(2);
		if (Physics.Raycast(base.transform.position, base.transform.TransformDirection(dir), out hit, maxLength, EditorLogic.LayerMask))
		{
			Part partUpwardsCached = FlightGlobals.GetPartUpwardsCached(hit.collider.gameObject);
			string tgtColName = hit.collider.name;
			if (partUpwardsCached != null && !partUpwardsCached.frozen)
			{
				targetPosition = base.transform.InverseTransformPoint(hit.point);
				targetRotation = Quaternion.FromToRotation(Vector3.right, base.transform.InverseTransformDirection(hit.normal));
				result = true;
				SetTarget(partUpwardsCached, tgtColName);
			}
		}
		base.gameObject.SetLayerRecursive(layer);
		return result;
	}

	private bool SetTarget(Part tgt, string tgtColName)
	{
		if (target != null && HighLogic.LoadedSceneIsEditor)
		{
			DumpTarget();
		}
		target = tgt;
		targetMeshColName = tgtColName;
		if (target != null)
		{
			if (target.frozen)
			{
				target = null;
				return false;
			}
			if (HighLogic.LoadedSceneIsEditor)
			{
				Part part = target;
				part.OnEditorDetach = (Callback)Delegate.Combine(part.OnEditorDetach, new Callback(onTargetDetach));
				Part part2 = target;
				part2.OnEditorDestroy = (Callback)Delegate.Combine(part2.OnEditorDestroy, new Callback(onTargetDestroy));
				Part part3 = target;
				part3.OnEditorAttach = (Callback)Delegate.Combine(part3.OnEditorAttach, new Callback(onTargetReattach));
			}
			tgtId = target.craftID;
			UpdateWorldValues();
			SetLink();
			attachState = AttachState.Attached;
			return true;
		}
		return false;
	}

	public void UpdateWorldValues()
	{
		wTgtPos = target.transform.InverseTransformPoint(base.transform.TransformPoint(targetPosition));
		wTgtRot = Quaternion.Inverse(target.transform.rotation) * base.transform.rotation * targetRotation;
	}

	private void DumpTarget()
	{
		if (target != null)
		{
			Part part = target;
			part.OnEditorDetach = (Callback)Delegate.Remove(part.OnEditorDetach, new Callback(onTargetDetach));
			Part part2 = target;
			part2.OnEditorDestroy = (Callback)Delegate.Remove(part2.OnEditorDestroy, new Callback(onTargetDestroy));
			Part part3 = target;
			part3.OnEditorAttach = (Callback)Delegate.Remove(part3.OnEditorAttach, new Callback(onTargetReattach));
		}
		onEditorEndTweak();
		wTgtPos = Vector3.zero;
		wTgtRot = Quaternion.identity;
		UnsetLink();
		target = null;
		tgtId = 0u;
		attachState = AttachState.Detached;
	}

	private void SetLink()
	{
		int num = cmpModules.Length;
		while (num-- > 0)
		{
			cmpModules[num].OnTargetSet(target);
		}
	}

	private void UnsetLink()
	{
		int num = cmpModules.Length;
		while (num-- > 0)
		{
			cmpModules[num].OnTargetLost();
		}
	}

	private void PreviewAttachment(Vector3 rDir, Vector3 rPos, Quaternion rRot)
	{
		int num = cmpModules.Length;
		while (num-- > 0)
		{
			cmpModules[num].OnPreviewAttachment(rDir, rPos, rRot);
		}
		if (symMethod == SymmetryMethod.Radial)
		{
			for (int i = 0; i < symmetryCounterparts.Count; i++)
			{
				CompoundPart compoundPart = (CompoundPart)symmetryCounterparts[i];
				int num2 = compoundPart.cmpModules.Length;
				while (num2-- > 0)
				{
					compoundPart.cmpModules[num2].OnPreviewAttachment(rDir, rPos, rRot);
				}
			}
		}
		if (symMethod != SymmetryMethod.Mirror)
		{
			return;
		}
		Vector3 vector = base.transform.TransformDirection(direction);
		vector = new Vector3(0f - vector.x, vector.y, vector.z);
		for (int j = 0; j < symmetryCounterparts.Count; j++)
		{
			CompoundPart compoundPart2 = (CompoundPart)symmetryCounterparts[j];
			Vector3 vector2 = compoundPart2.transform.InverseTransformDirection(vector);
			Vector3 rPos2 = vector2 * rPos.magnitude;
			Quaternion anchorRot = compoundPart2.getAnchorRot(vector2, compoundPart2.targetRotation);
			int num3 = compoundPart2.cmpModules.Length;
			while (num3-- > 0)
			{
				compoundPart2.cmpModules[num3].OnPreviewAttachment(vector2, rPos2, anchorRot);
			}
		}
	}

	private void EndPreview()
	{
		int num = cmpModules.Length;
		while (num-- > 0)
		{
			cmpModules[num].OnPreviewEnd();
		}
		for (int i = 0; i < symmetryCounterparts.Count; i++)
		{
			CompoundPart compoundPart = (CompoundPart)symmetryCounterparts[i];
			int num2 = compoundPart.cmpModules.Length;
			while (num2-- > 0)
			{
				compoundPart.cmpModules[num2].OnPreviewEnd();
			}
		}
	}

	private Quaternion getAnchorRot(Vector3 rDir, Quaternion defaultRot)
	{
		int layer = base.gameObject.layer;
		base.gameObject.SetLayerRecursive(2);
		Quaternion result = defaultRot;
		if (Physics.Raycast(base.transform.position, base.transform.TransformDirection(rDir), out hit, maxLength, EditorLogic.LayerMask))
		{
			Part partUpwardsCached = FlightGlobals.GetPartUpwardsCached(hit.collider.gameObject);
			if (partUpwardsCached != null && !partUpwardsCached.frozen)
			{
				result = Quaternion.FromToRotation(Vector3.right, base.transform.InverseTransformDirection(hit.normal));
			}
		}
		base.gameObject.SetLayerRecursive(layer);
		return result;
	}

	private void OnEditorEvent(ConstructionEventType evt, Part selPart)
	{
		if ((uint)(evt - 10) <= 3u)
		{
			UpdateTargetCoords();
		}
	}

	public void UpdateTargetCoords()
	{
		if (!(target != null))
		{
			return;
		}
		targetPosition = base.transform.InverseTransformPoint(target.transform.TransformPoint(wTgtPos));
		targetRotation = Quaternion.Inverse(base.transform.rotation) * target.transform.rotation * wTgtRot;
		direction = targetPosition.normalized;
		if (attachState == AttachState.Attached)
		{
			int num = cmpModules.Length;
			while (num-- > 0)
			{
				cmpModules[num].OnTargetUpdate();
			}
		}
	}

	public ConfigNode ParseCustomPartData(string customPartData)
	{
		ConfigNode configNode = new ConfigNode();
		if (customPartData != string.Empty)
		{
			Debug.LogWarning("[CompoundPart]: Deprecated 'customPartData' field found. Upgrading to new format...");
			string[] array = customPartData.Split(';');
			foreach (string text in array)
			{
				if (!text.Contains(":"))
				{
					continue;
				}
				string text2 = text.Split(':')[0].Trim();
				string text3 = text.Split(':')[1].Trim();
				switch (text2)
				{
				case "rot":
					configNode.AddValue("rot", text3);
					break;
				case "pos":
					configNode.AddValue("pos", text3);
					break;
				case "dir":
					configNode.AddValue("dir", text3);
					break;
				case "tgt":
					if (text3.Contains("_"))
					{
						int index = int.Parse(text3.Split('_')[1].Trim());
						uint value = 0u;
						if (HighLogic.LoadedSceneIsEditor)
						{
							value = EditorLogic.SortedShipList[index].craftID;
						}
						else if (HighLogic.LoadedSceneIsFlight)
						{
							value = vessel.parts[index].craftID;
						}
						configNode.AddValue("tgt", value);
					}
					break;
				}
			}
		}
		return configNode;
	}
}
