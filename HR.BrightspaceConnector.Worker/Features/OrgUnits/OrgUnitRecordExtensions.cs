namespace HR.BrightspaceConnector.Features.OrgUnits
{
    public static class OrgUnitRecordExtensions
    {
        public static OrgUnitCreateData ToOrgUnitCreateData(this OrgUnitRecord orgUnitRecord)
        {
            var orgUnitCreateData = new OrgUnitCreateData
            {
                Code = orgUnitRecord.Code,
                Name = orgUnitRecord.Name,
                Type = orgUnitRecord.Type
            };

            if (orgUnitRecord.Parent != null)
            {
                orgUnitCreateData.Parents = new[] { (int)orgUnitRecord.Parent };
            }
            return orgUnitCreateData;
        }
    }
}
