using HR.BrightspaceConnector.Features.Common;
using HR.BrightspaceConnector.Tests.Fixture;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class RecordExtensionsTests
    {
        [TestMethod]
        public void IsToBeCreated_ReturnsTrue_ForLowerCaseC()
        {
            // Arrange
            var record = new ConcreteRecord() { SyncAction = 'c' };

            // Act
            var result = record.IsToBeCreated();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsToBeCreated_ReturnsTrue_ForUpperCaseC()
        {
            // Arrange
            var record = new ConcreteRecord() { SyncAction = 'C' };

            // Act
            var result = record.IsToBeCreated();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsToBeCreated_ReturnsFalse_ForNullChar()
        {
            // Arrange
            var record = new ConcreteRecord() { SyncAction = null };

            // Act
            var result = record.IsToBeCreated();

            // Assert
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow('u')]
        [DataRow('d')]
        [DataRow('$')]
        public void IsToBeCreated_ReturnsFalse_ForAnyOtherChar(char c)
        {
            // Arrange
            var record = new ConcreteRecord() { SyncAction = c };

            // Act
            var result = record.IsToBeCreated();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsToBeUpdated_ReturnsTrue_ForLowerCaseU()
        {
            // Arrange
            var record = new ConcreteRecord() { SyncAction = 'u' };

            // Act
            var result = record.IsToBeUpdated();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsToBeUpdated_ReturnsTrue_ForUpperCaseU()
        {
            // Arrange
            var record = new ConcreteRecord() { SyncAction = 'U' };

            // Act
            var result = record.IsToBeUpdated();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsToBeUpdated_ReturnsFalse_ForNullChar()
        {
            // Arrange
            var record = new ConcreteRecord() { SyncAction = null };

            // Act
            var result = record.IsToBeUpdated();

            // Assert
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow('c')]
        [DataRow('d')]
        [DataRow('$')]
        public void IsToBeUpdated_ReturnsFalse_ForAnyOtherChar(char c)
        {
            // Arrange
            var record = new ConcreteRecord() { SyncAction = c };

            // Act
            var result = record.IsToBeUpdated();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsToBeUpdated_ReturnsTrue_ForLowerCaseD()
        {
            // Arrange
            var record = new ConcreteRecord() { SyncAction = 'd' };

            // Act
            var result = record.IsToBeDeleted();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsToBeDeleted_ReturnsTrue_ForUpperCaseD()
        {
            // Arrange
            var record = new ConcreteRecord() { SyncAction = 'D' };

            // Act
            var result = record.IsToBeDeleted();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsToBeDeleted_ReturnsFalse_ForNullChar()
        {
            // Arrange
            var record = new ConcreteRecord() { SyncAction = null };

            // Act
            var result = record.IsToBeDeleted();

            // Assert
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow('c')]
        [DataRow('u')]
        [DataRow('$')]
        public void IsToBeDeleted_ReturnsFalse_ForAnyOtherChar(char c)
        {
            // Arrange
            var record = new ConcreteRecord() { SyncAction = c };

            // Act
            var result = record.IsToBeDeleted();

            // Assert
            Assert.IsFalse(result);
        }
    }
}
