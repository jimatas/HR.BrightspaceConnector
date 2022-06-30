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

        /// <summary>
        /// This async overload looks up the correct orgunit type ID to return in the <see cref="OrgUnitCreateData"/> object, by using the type code that is provided in addition to the (possibly erroneous) type ID in the source <see cref="OrgUnitRecord"/> object.
        /// </summary>
        /// <param name="orgUnitRecord"></param>
        /// <param name="apiClient"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// This async overload looks up the correct orgunit type ID to return in the <see cref="OrgUnitProperties"/> object, by using the type code that is provided in addition to the (possibly erroneous) type ID in the source <see cref="OrgUnitRecord"/> object.
        /// </summary>
        /// <param name="orgUnitRecord"></param>
        /// <param name="apiClient"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<OrgUnitProperties> ToOrgUnitPropertiesAsync(this OrgUnitRecord orgUnitRecord, IApiClient apiClient, CancellationToken cancellationToken = default)
        {
            var orgUnitProperties = orgUnitRecord.ToOrgUnitProperties();
            if (orgUnitProperties.Type is not null)
            {
                var orgUnitTypes = await apiClient.GetOrgUnitTypes(cancellationToken).WithoutCapturingContext();
                var orgUnitType = orgUnitTypes.SingleOrDefault(type => string.Equals(type.Code, orgUnitRecord.TypeCode, StringComparison.OrdinalIgnoreCase));
                if (orgUnitType is not null && (orgUnitProperties.Type.Id != orgUnitType.Id || !string.Equals(orgUnitProperties.Type.Code, orgUnitType.Code, StringComparison.Ordinal)))
                {
                    orgUnitProperties.Type.Id = orgUnitType.Id;
                    orgUnitProperties.Type.Code = orgUnitType.Code;
                }
            }
            return orgUnitProperties;
        }
    }
}
