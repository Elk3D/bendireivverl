public class InspectDataVO
{
	public string ActionLabel;

	public ArchivesInspectID ArchivesTitleID;

	public ArchivesInspectID ArchivesDescriptionID;

	public InspectDataVO(string actionLabel)
	{
		ActionLabel = actionLabel;
	}

	public InspectDataVO(ArchivesInspectID archivesTitleID, ArchivesInspectID archivesDescriptionID)
	{
		ArchivesTitleID = archivesTitleID;
		ArchivesDescriptionID = archivesDescriptionID;
	}
}
