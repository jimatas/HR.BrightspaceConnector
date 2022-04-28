using HR.BrightspaceConnector.Security;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class AsyncOutResultTests
    {
        [TestMethod]
        public void Out_ByDefault_ReturnsReturnValue()
        {
            var asyncOutResult = new AsyncOutResult<bool, string>(true, "true");

            var returnValue = asyncOutResult.Out(out _);

            Assert.IsTrue(returnValue);
        }

        [TestMethod]
        public void Out_ByDefault_AssignsOutParamValue()
        {
            var asyncOutResult = new AsyncOutResult<bool, string>(true, "true");

            _ = asyncOutResult.Out(out var paramValue);

            Assert.AreEqual("true", paramValue);
        }

        [TestMethod]
        public void ImplicitOperator_GivenTuple_InitializesNewInstance()
        {
            AsyncOutResult<bool, string> asyncOutResult = (true, "true");

            var returnValue = asyncOutResult.Out(out var paramValue);

            Assert.IsTrue(returnValue);
            Assert.AreEqual("true", paramValue);
        }
    }
}
