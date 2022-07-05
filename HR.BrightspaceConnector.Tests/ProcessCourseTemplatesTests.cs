using HR.BrightspaceConnector.Features.Courses;
using HR.BrightspaceConnector.Features.Courses.Commands;
using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Commands;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using System;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class ProcessCourseTemplatesTests : DependencyInjectedTestsBase
    {
        private readonly Mock<IDatabase> mockedDatabase = new();
        private readonly Mock<IApiClient> mockedApiClient = new();

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task GivenNullCourseTemplateInAnyContext_DoesNothing(bool isDeleteContext)
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextCourseTemplateAsync(default)).ReturnsAsync(null as CourseTemplateRecord);

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessCourseTemplates(batchSize: 1, isDeleteContext));

            // Assert
            mockedDatabase.Verify(database => database.GetNextCourseTemplateAsync(default), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();
            mockedApiClient.VerifyNoOtherCalls();
        }
    }
}
