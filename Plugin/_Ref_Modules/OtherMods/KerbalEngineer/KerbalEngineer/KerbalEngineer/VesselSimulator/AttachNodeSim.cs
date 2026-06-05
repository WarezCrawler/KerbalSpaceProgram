namespace KerbalEngineer.VesselSimulator;

internal class AttachNodeSim
{
	private static readonly Pool<AttachNodeSim> pool = new Pool<AttachNodeSim>(Create, Reset);

	public PartSim attachedPartSim;

	public string id;

	public NodeType nodeType;

	private static AttachNodeSim Create()
	{
		return new AttachNodeSim();
	}

	public static AttachNodeSim New(PartSim partSim, string newId, NodeType newNodeType)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		AttachNodeSim attachNodeSim = pool.Borrow();
		attachNodeSim.attachedPartSim = partSim;
		attachNodeSim.nodeType = newNodeType;
		attachNodeSim.id = newId;
		return attachNodeSim;
	}

	private static void Reset(AttachNodeSim attachNodeSim)
	{
		attachNodeSim.attachedPartSim = null;
	}

	public void Release()
	{
		pool.Release(this);
	}

	public void DumpToLog(LogMsg log)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (attachedPartSim == null)
		{
			log.Append("<staged>:<n>");
		}
		else
		{
			log.Append(attachedPartSim.name, ":", attachedPartSim.partId);
		}
		log.Append<string, NodeType, string, string>("#", nodeType, ":", id);
	}
}
