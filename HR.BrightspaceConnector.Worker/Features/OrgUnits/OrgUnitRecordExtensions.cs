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

        public static OrgUnitProperties ToOrgUnitProperties(this OrgUnitRecord orgUnitRecord)
        {
            return new OrgUnitProperties
            {
                Code = orgUnitRecord.Code,
                Identifier = orgUnitRecord.SyncExternalKey is null ? null : Convert.ToInt32(orgUnitRecord.SyncExternalKey),
                Name = orgUnitRecord.Name,
                Type = new OrgUnitTypeInfo { Id = orgUnitRecord.Type }
            };
        }
    }
}
