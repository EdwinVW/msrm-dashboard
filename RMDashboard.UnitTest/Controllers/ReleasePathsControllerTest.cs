using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RMDashboard.Controllers;
using RMDashboard.Models;
using RMDashboard.Repositories;
using RMDashboard.UnitTest.TestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace RMDashboard.UnitTest.Controllers
{
    [TestClass]
    public class ReleasePathsControllerTest
    {
        #region Private Fields

        private Mock<IReleaseRepository> _releaseRepositoryMock;
        private ReleasePathsController _sut;

        #endregion

        #region Test Setup & Teardown

        [TestInitialize]
        public void TestInitialize()
        {
            _releaseRepositoryMock = new Mock<IReleaseRepository>();
            _sut = new ReleasePathsController(_releaseRepositoryMock.Object);
            _sut.Request = new HttpRequestMessage();
        }

        #endregion

        #region Method Get

        [TestMethod]
        public void Get_ErrorOccursRetrievingReleasePaths_InternalServerErrorReturned()
        {
            //Arrange
            _releaseRepositoryMock.Setup((stub) => stub.GetReleasePaths())
                .Throws<Exception>();

            //Act
            var result = _sut.Get();

            //Assert
            AssertionHelper.AssertHttpResponseMessageStatus(result, HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void Get_MultipleReleasePathsFound_ReleasePathsReturned()
        {
            //Arrange
            var expectedMultipleReleasePaths = new ReleasePathsBuilder().WithMany().Build();

            _releaseRepositoryMock.Setup((stub) => stub.GetReleasePaths())
                .Returns(expectedMultipleReleasePaths);

            //Act
            var result = _sut.Get();

            //Assert
            AssertReleasePathCollectionsAreEqual(expectedMultipleReleasePaths, result);
        }

        [TestMethod]
        public void Get_NoReleasePathsFound_EmptyReleasePathsCollectionReturned()
        {
            //Arrange
            var expectedEmptyReleasePaths = new ReleasePathsBuilder().Empty().Build();

            _releaseRepositoryMock.Setup((stub) => stub.GetReleasePaths())
                .Returns(expectedEmptyReleasePaths);

            //Act
            var result = _sut.Get();

            //Assert
            AssertReleasePathCollectionsAreEqual(expectedEmptyReleasePaths, result);
        }

        #endregion

        #region Private Methods

        internal static void AssertReleasePathCollectionsAreEqual(List<ReleasePath> expectedReleasePaths, object actualReleasePaths)
        {
            Assert.IsNotNull(actualReleasePaths, "Collection with actual release paths is null");
            Assert.IsInstanceOfType(actualReleasePaths, typeof(IEnumerable<dynamic>), "Unexpected type");

            var releasePaths = ((IEnumerable<dynamic>)actualReleasePaths).ToList();
            Assert.AreEqual(expectedReleasePaths.Count, releasePaths.Count, "Unexpected number of release paths");

            for (int i = 0; i < expectedReleasePaths.Count; i++)
            {
                var expectedReleasePath = expectedReleasePaths[i];
                dynamic releasePath = releasePaths[i];

                Assert.AreEqual(expectedReleasePath.Id, releasePath.id, "Unexpected id");
                Assert.AreEqual(expectedReleasePath.Name, releasePath.name, "Unexpected name for release path with id {0}", expectedReleasePath.Id);
                Assert.AreEqual(expectedReleasePath.Description, releasePath.description, "Unexpected name for release path with id {0}", expectedReleasePath.Id);
            }
        }

        #endregion
    }
}
