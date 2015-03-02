using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMDashboard.Validators;
using System.Data;
using System;

namespace RMDashboard.UnitTest.Validators
{
    [TestClass]
    public class IncludedReleasePathIdsValidatorTest
    {
        #region Properties

        public TestContext TestContext { get; set; }

        #endregion

        #region Method IsValidIncludedReleasePathIds

        [TestMethod]
        public void IsValidIncludedReleasePathIds_IncludedReleasePathIdsIsNull_TrueReturned()
        {
            var result = IncludedReleasePathIdsValidator.IsValidIncludedReleasePathIds(null);
            Assert.IsTrue(result, "Unexpected result");
        }

        [TestMethod]
        [DeploymentItem(@"Validators\IsValidIncludedReleasePathIdsTestCases.csv")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", @"IsValidIncludedReleasePathIdsTestCases.csv", "IsValidIncludedReleasePathIdsTestCases#csv", DataAccessMethod.Random)]
        public void IsValidIncludedReleasePathIds_DataDriven_DataDriven()
        {
            //Arrange
            var includedReleasePathIds = TestContext.DataRow["IncludedReleasePathIds"].ToString();
            var expectedResult = Boolean.Parse(TestContext.DataRow["ExpectedResult"].ToString());

            //Act
            var result = IncludedReleasePathIdsValidator.IsValidIncludedReleasePathIds(includedReleasePathIds);

            //Assert
            Assert.AreEqual(expectedResult, result, "Unexpected result");
        }

        #endregion
    }
}
