using HR.BrightspaceConnector.Features.Enrollments;
using HR.BrightspaceConnector.Features.Enrollments.Decorators;
using HR.BrightspaceConnector.Features.Enrollments.Queries;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class EnsureExternallyIdentifiableTests
    {
        [DataTestMethod]
        [DataRow(2113, 1402, "4468284")]
        [DataRow(1334, 1891, "3577215")]
        [DataRow(476, 1092, "1192940")]
        public async Task GivenEnrollmentForCreateAction_SetsExternalKeyFromOrgUnitIdAndUserId(int orgUnitId, int userId, string externalKey)
        {
            // Arrange
            var nextEnrollment = new EnrollmentRecord
            {
                SyncAction = 'c',
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncExternalKey = null,
                SyncInternalKey = Random.Shared.Next(1, int.MaxValue).ToString(),
                OrgUnitId = orgUnitId,
                UserId = userId,
                RoleId = 102
            };

            EnsureExternallyIdentifiable handler = new();

            // Act
            var result = await handler.HandleAsync(
                query: new GetNextEnrollment(),
                next: () => Task.FromResult<EnrollmentRecord?>(nextEnrollment),
                cancellationToken: default);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(externalKey, result.SyncExternalKey);
        }

        [DataTestMethod]
        [DataRow(2798, 1516, "7833118")]
        [DataRow(1322, 625, "1749631")]
        [DataRow(555, 824, "679531")]
        public async Task GivenEnrollmentForDeleteAction_SetsOrgUnitIdAndUserIdFromExternalKey(int orgUnitId, int userId, string externalKey)
        {
            // Arrange
            var nextEnrollment = new EnrollmentRecord
            {
                SyncAction = 'd',
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncExternalKey = externalKey,
                SyncInternalKey = Random.Shared.Next(1, int.MaxValue).ToString(),
                OrgUnitId = null,
                UserId = null,
                RoleId = null
            };

            EnsureExternallyIdentifiable handler = new();

            // Act
            var result = await handler.HandleAsync(
                query: new GetNextEnrollment(),
                next: () => Task.FromResult<EnrollmentRecord?>(nextEnrollment),
                cancellationToken: default);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(orgUnitId, result.OrgUnitId);
            Assert.AreEqual(userId, result.UserId);
            Assert.IsNull(result.RoleId);
        }
    }
}
