using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.OrgUnits
{
    public static class OrgUnitRecordExtensions
    {
        public static OrgUnitCreateData ToOrgUnitCreateData(this OrgUnitRecord orgUnitRecord)
        {
            return new OrgUnitCreateData
            {
                Code = orgUnitRecord.Code,
                Name = orgUnitRecord.Name,
                Type = orgUnitRecord.Type,
                Parents = orgUnitRecord.Parents
            };
        }

        public static async Task<OrgUnitCreateData> ToOrgUnitCreateDataAsync(this OrgUnitRecord orgUnitRecord, IApiClient apiClient, CancellationToken cancellationToken = default)
        {
            var orgUnitCreateData = orgUnitRecord.ToOrgUnitCreateData();
            var orgUnitTypes = await apiClient.GetOrgUnitTypes(cancellationToken).WithoutCapturingContext();
            var orgUnitType = orgUnitTypes.SingleOrDefault(type => string.Equals(type.Code, orgUnitRecord.TypeCode, StringComparison.OrdinalIgnoreCase));
            if (orgUnitType is not null && orgUnitCreateData.Type != orgUnitType.Id)
            {
                orgUnitCreateData.Type = orgUnitType.Id;
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
                Type = new OrgUnitTypeInfo
                {
                    Id = orgUnitRecord.Type,
                    Code = orgUnitRecord.TypeCode
                }
            };
        }
    }
}
