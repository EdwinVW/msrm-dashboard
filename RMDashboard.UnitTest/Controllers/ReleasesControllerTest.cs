using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RMDashboard.Controllers;
using RMDashboard.Models;
using RMDashboard.Repositories;
using RMDashboard.UnitTest.TestHelpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace RMDashboard.UnitTest.Controllers
{
    [TestClass]
    public class ReleasesControllerTest
    {
        #region Private Fields

        private Mock<IReleaseRepository> _releaseRepositoryMock;
        private ReleasesController _sut;

        private const string DEFAULT_INCLUDE_RELEASE_PATH_IDS = null;
        private const int DEFAULT_RELEASE_COUNT = 5;

        #endregion

        #region Test Setup & Teardown

        [TestInitialize]
        public void TestInitialize()
        {
            _releaseRepositoryMock = new Mock<IReleaseRepository>();
            _sut = new ReleasesController(_releaseRepositoryMock.Object);
            _sut.Request = new HttpRequestMessage();
        }

        #endregion

        #region Method Get

        [TestMethod]
        public void Get_ErrorOccursRetrievingReleaseData_InternalServerErrorReturned()
        {
            //Arrange
            var request = new GetReleasesHttpRequestMessageBuilder().Build();

            _releaseRepositoryMock.Setup((stub) => stub.GetReleaseData(It.IsAny<string>(), It.IsAny<int>()))
                .Throws<Exception>();

            //Act
            var result = _sut.Get(request);

            //Assert
            AssertionHelper.AssertHttpResponseMessageStatus(result, HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void Get_NoHeadersSpecified_DefaultFiltersUsedToRetrieveReleaseData()
        {
            //Arrange
            var request = new GetReleasesHttpRequestMessageBuilder().Build();

            _releaseRepositoryMock.Setup((stub) => stub.GetReleaseData(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new DataModelBuilder().Build());

            //Act
            _sut.Get(request);

            //Assert
            _releaseRepositoryMock.Verify((mock) => mock.GetReleaseData(DEFAULT_INCLUDE_RELEASE_PATH_IDS, DEFAULT_RELEASE_COUNT));
        }

        [TestMethod]
        public void Get_IncludedReleasePathIdsSpecified_IncludedReleasePathIdsFilterUsedToRetrieveReleaseData()
        {
            //Arrange
            string includeReleasePathIds = "1";
            var request = new GetReleasesHttpRequestMessageBuilder()
                .WithIncludedReleasePathIdsHeader(includeReleasePathIds)
                .Build();

            _releaseRepositoryMock.Setup((stub) => stub.GetReleaseData(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new DataModelBuilder().Build());

            //Act
            _sut.Get(request);

            //Assert
            _releaseRepositoryMock.Verify((mock) => mock.GetReleaseData(includeReleasePathIds, DEFAULT_RELEASE_COUNT));
        }

        [TestMethod]
        public void Get_ReleaseCountSpecified_ReleaseCountFilterUsedToRetrieveReleaseData()
        {
            //Arrange
            var releaseCount = 123;
            var request = new GetReleasesHttpRequestMessageBuilder()
                .WithReleaseCountHeader(releaseCount)
                .Build();

            _releaseRepositoryMock.Setup((stub) => stub.GetReleaseData(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new DataModelBuilder().Build());

            //Act
            _sut.Get(request);

            //Assert
            _releaseRepositoryMock.Verify((mock) => mock.GetReleaseData(DEFAULT_INCLUDE_RELEASE_PATH_IDS, releaseCount));
        }

        [TestMethod]
        public void Get_ReleaseWithStageAndStepFound_ReleaseWithStageAndStepReturned()
        {
            //Arrange
            var request = new GetReleasesHttpRequestMessageBuilder().Build();

            var expectedRelease = new ReleaseBuilder().Build();
            var expectedEnvironment = new EnvironmentBuilder().Build();
            var expectedStage = new StageBuilder().ForEnvironment(expectedEnvironment).Build();
            var expectedStep = new StepBuilder()
                .ForRelease(expectedRelease)
                .ForStage(expectedStage)
                .Build();

            var dataModel = new DataModelBuilder()
                .WithRelease(expectedRelease)
                .WithEnvironment(expectedEnvironment)
                .WithStage(expectedStage)
                .WithStageWorkflowFor(expectedRelease, expectedStage)
                .WithStep(expectedStep)
                .Build();

            _releaseRepositoryMock.Setup((stub) => stub.GetReleaseData(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(dataModel);

            //Act
            dynamic result = _sut.Get(request);

            //Assert
            Assert.IsNotNull(result, "Unexpected result");

            Assert.IsInstanceOfType(result.releases, typeof(List<dynamic>), "Unexpected type for releases collection");
            Assert.AreEqual(1, result.releases.Count, "Unexpected number of releases");
            var actualRelease = result.releases[0];
            AssertAreReleasesEqual(expectedRelease, actualRelease);

            Assert.IsInstanceOfType(actualRelease.stages, typeof(List<dynamic>), "Unexpected type for stages collection");
            Assert.AreEqual(1, actualRelease.stages.Count, "Unexpected number of stages for release");
            var actualStage = actualRelease.stages[0];
            AssertAreStagesEqual(expectedStage, expectedEnvironment, actualStage);

            Assert.IsInstanceOfType(actualStage.steps, typeof(List<dynamic>), "Unexpected type for steps collection");
            Assert.AreEqual(1, actualStage.steps.Count, "Unexpected number of steps for stage");
            AssertAreStepsEqual(expectedStep, actualStage.steps[0]);
        }

        [TestMethod]
        public void Get_MultipleReleasesFound_MultipleReleasesReturned()
        {
            //Arrange
            var request = new GetReleasesHttpRequestMessageBuilder().Build();

            var expectedRelease1 = new ReleaseBuilder().Build();
            var expectedRelease2 = new ReleaseBuilder().Build();
            var expectedRelease3 = new ReleaseBuilder().Build();

            var dataModel = new DataModelBuilder()
                .WithRelease(expectedRelease1)
                .WithRelease(expectedRelease2)
                .WithRelease(expectedRelease3)
                .Build();

            _releaseRepositoryMock.Setup((stub) => stub.GetReleaseData(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(dataModel);

            //Act
            dynamic result = _sut.Get(request);

            //Assert
            Assert.IsNotNull(result, "Unexpected result");
            Assert.IsInstanceOfType(result.releases, typeof(List<dynamic>), "Unexpected type for releases collection");

            List<dynamic> actualReleases = (List<dynamic>)result.releases;
            Assert.AreEqual(3, actualReleases.Count, "Unexpected number of releases");
            AssertReleasesCollectionContainsRelease(actualReleases, expectedRelease1);
            AssertReleasesCollectionContainsRelease(actualReleases, expectedRelease2);
            AssertReleasesCollectionContainsRelease(actualReleases, expectedRelease3);
        }

        [TestMethod]
        public void Get_ReleaseWithMultipleStages_StagesAreOrderdByRank()
        {
            //Arrange
            var request = new GetReleasesHttpRequestMessageBuilder().Build();

            var expectedRelease = new ReleaseBuilder().Build();
            var expectedEnvironment = new EnvironmentBuilder().Build();

            var expectedStageBuilder = new StageBuilder().ForEnvironment(expectedEnvironment);
            var expectedStageWithRank1 = expectedStageBuilder.WithRank(1).Build();
            var expectedStageWithRank2 = expectedStageBuilder.WithRank(2).Build();
            var expectedStageWithRank3 = expectedStageBuilder.WithRank(3).Build();

            var dataModel = new DataModelBuilder()
                .WithRelease(expectedRelease)
                .WithEnvironment(expectedEnvironment)
                .WithStageWorkflowFor(expectedRelease, expectedStageWithRank1)
                .WithStageWorkflowFor(expectedRelease, expectedStageWithRank2)
                .WithStageWorkflowFor(expectedRelease, expectedStageWithRank3)

                //add stages in 'random' order to check sorting of stages by rank
                .WithStage(expectedStageWithRank2)
                .WithStage(expectedStageWithRank1)
                .WithStage(expectedStageWithRank3)
                .Build();

            _releaseRepositoryMock.Setup((stub) => stub.GetReleaseData(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(dataModel);

            //Act
            dynamic result = _sut.Get(request);

            //Assert
            Assert.IsNotNull(result, "Unexpected result");
            Assert.AreEqual(expectedStageWithRank1.Id, result.releases[0].stages[0].id, "Unexpected first stage");
            Assert.AreEqual(expectedStageWithRank2.Id, result.releases[0].stages[1].id, "Unexpected second stage");
            Assert.AreEqual(expectedStageWithRank3.Id, result.releases[0].stages[2].id, "Unexpected third stage");
        }

        [TestMethod]
        public void Get_StageWithMultipleSteps_StepsAreOrderdByAttemptAndThenByRank()
        {
            //Arrange
            var request = new GetReleasesHttpRequestMessageBuilder().Build();

            var expectedRelease = new ReleaseBuilder().Build();
            var expectedEnvironment = new EnvironmentBuilder().Build();
            var expectedStage = new StageBuilder().ForEnvironment(expectedEnvironment).Build();

            var expectedStepBuilder = new StepBuilder().ForRelease(expectedRelease).ForStage(expectedStage);
            var expectedStepWithAttempt1AndRank1 = expectedStepBuilder.WithAttempt(1).WithRank(1).Build();
            var expectedStepWithAttempt1AndRank2 = expectedStepBuilder.WithAttempt(1).WithRank(2).Build();
            var expectedStepWithAttempt1AndRank3 = expectedStepBuilder.WithAttempt(1).WithRank(3).Build();
            var expectedStepWithAttempt2AndRank1 = expectedStepBuilder.WithAttempt(2).WithRank(1).Build();
            var expectedStepWithAttempt2AndRank2 = expectedStepBuilder.WithAttempt(2).WithRank(2).Build();

            var dataModel = new DataModelBuilder()
                .WithRelease(expectedRelease)
                .WithEnvironment(expectedEnvironment)
                .WithStage(expectedStage)
                .WithStageWorkflowFor(expectedRelease, expectedStage)

                //add steps in 'random' order to check sorting of steps by attempt and then by rank
                .WithStep(expectedStepWithAttempt2AndRank2)
                .WithStep(expectedStepWithAttempt1AndRank2)
                .WithStep(expectedStepWithAttempt1AndRank1)
                .WithStep(expectedStepWithAttempt1AndRank3)
                .WithStep(expectedStepWithAttempt2AndRank1)
                .Build();

            _releaseRepositoryMock.Setup((stub) => stub.GetReleaseData(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(dataModel);

            //Act
            dynamic result = _sut.Get(request);

            //Assert
            Assert.IsNotNull(result, "Unexpected result");

            var stage = result.releases[0].stages[0];
            Assert.AreEqual(expectedStepWithAttempt1AndRank1.Id, stage.steps[0].id, "Unexpected first step");
            Assert.AreEqual(expectedStepWithAttempt1AndRank2.Id, stage.steps[1].id, "Unexpected second step");
            Assert.AreEqual(expectedStepWithAttempt1AndRank3.Id, stage.steps[2].id, "Unexpected third step");
            Assert.AreEqual(expectedStepWithAttempt2AndRank1.Id, stage.steps[3].id, "Unexpected fourth step");
            Assert.AreEqual(expectedStepWithAttempt2AndRank2.Id, stage.steps[4].id, "Unexpected fifth step");
        }

        [TestMethod]
        public void Get_ShowComponentsHeaderIsTrue_ComponentReturned()
        {
            //Arrange
            var requestWithShowComponentsHeaderIsTrue = new GetReleasesHttpRequestMessageBuilder()
                .WithShowComponentsHeader(true)
                .Build();

            var expectedRelease = new ReleaseBuilder().Build();
            var expectedComponent = new ComponentBuilder().ForRelease(expectedRelease).Build();

            var dataModel = new DataModelBuilder()
                .WithRelease(expectedRelease)
                .WithComponent(expectedComponent)
                .Build();

            _releaseRepositoryMock.Setup((stub) => stub.GetReleaseData(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(dataModel);

            //Act
            dynamic result = _sut.Get(requestWithShowComponentsHeaderIsTrue);

            //Assert
            Assert.IsNotNull(result, "Unexpected result");

            var actualRelease = result.releases[0];
            Assert.IsInstanceOfType(actualRelease.components, typeof(List<dynamic>), "Unexpected type for components collection");
            Assert.AreEqual(1, actualRelease.components.Count, "Unexpected number of components for release");
            Assert.AreEqual(expectedComponent.Build, actualRelease.components[0].build);
        }

        [TestMethod]
        public void Get_ReleaseWithComponentButShowComponentsHeaderIsFalse_NoComponentReturned()
        {
            //Arrange
            var requestWithShowComponentsHeaderIsFalse = new GetReleasesHttpRequestMessageBuilder()
                .WithShowComponentsHeader(false)
                .Build();

            var expectedRelease = new ReleaseBuilder().Build();
            var component = new ComponentBuilder().ForRelease(expectedRelease).Build();

            var dataModel = new DataModelBuilder()
                .WithRelease(expectedRelease)
                .WithComponent(component)
                .Build();

            _releaseRepositoryMock.Setup((stub) => stub.GetReleaseData(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(dataModel);

            //Act
            dynamic result = _sut.Get(requestWithShowComponentsHeaderIsFalse);

            //Assert
            Assert.IsNotNull(result, "Unexpected result");

            var actualRelease = result.releases[0];
            Assert.IsInstanceOfType(actualRelease.components, typeof(List<dynamic>), "Unexpected type for components collection");
            Assert.AreEqual(0, actualRelease.components.Count, "Unexpected number of components for release");
        }

        [TestMethod]
        public void Get_ReleaseWithMultipleComponents_ComponentsAreOrderdByBuildDefinition()
        {
            //Arrange
            var request = new GetReleasesHttpRequestMessageBuilder().WithShowComponentsHeader(true).Build();

            var expectedRelease = new ReleaseBuilder().Build();

            var expectedComponentBuilder = new ComponentBuilder().ForRelease(expectedRelease);
            var expectedComponentWithBuildDefinitionA = expectedComponentBuilder.WithBuildDefinition("A").Build();
            var expectedComponentWithBuildDefinitionB = expectedComponentBuilder.WithBuildDefinition("B").Build();
            var expectedComponentWithBuildDefinitionC = expectedComponentBuilder.WithBuildDefinition("C").Build();

            var dataModel = new DataModelBuilder()
                .WithRelease(expectedRelease)

                //add components in 'random' order to check sorting of components by build definition
                .WithComponent(expectedComponentWithBuildDefinitionB)
                .WithComponent(expectedComponentWithBuildDefinitionA)
                .WithComponent(expectedComponentWithBuildDefinitionC)
                .Build();

            _releaseRepositoryMock.Setup((stub) => stub.GetReleaseData(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(dataModel);

            //Act
            dynamic result = _sut.Get(request);

            //Assert
            Assert.IsNotNull(result, "Unexpected result");
            Assert.AreEqual(expectedComponentWithBuildDefinitionA.Build, result.releases[0].components[0].build, "Unexpected first component");
            Assert.AreEqual(expectedComponentWithBuildDefinitionB.Build, result.releases[0].components[1].build, "Unexpected second component");
            Assert.AreEqual(expectedComponentWithBuildDefinitionC.Build, result.releases[0].components[2].build, "Unexpected third component");
        }

        [TestMethod]
        public void Get_ValidRequest_LastRefreshIsNow()
        {
            //Arrange
            var request = new GetReleasesHttpRequestMessageBuilder().Build();
            var dataModel = new DataModelBuilder().WithDefaultDataSet().Build();

            _releaseRepositoryMock.Setup((stub) => stub.GetReleaseData(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(dataModel);

            //Act
            dynamic result = _sut.Get(request);

            //Assert
            Assert.IsNotNull(result, "Unexpected result");
            AssertionHelper.AssertDateTimeIsNow(result.lastRefresh);
        }

        [TestMethod]
        public void Get_InvalidIncludedReleasePathIdsHeaderSpecified_BadRequestStatusReturned()
        {
            //Arrange
            var invalidRequest = new GetReleasesHttpRequestMessageBuilder()
                .WithIncludedReleasePathIdsHeader("invalid")
                .Build();

            //Act
            var result = _sut.Get(invalidRequest);

            //Assert
            AssertionHelper.AssertHttpResponseMessageStatus(result, HttpStatusCode.BadRequest);
        }

        #endregion

        #region Private Methods

        internal static void AssertAreReleasesEqual(Release expectedRelease, dynamic actualRelease)
        {
            Assert.AreEqual(expectedRelease.Name, actualRelease.name, "Unexpected name for release with id {0}", expectedRelease.Id);
            Assert.AreEqual(expectedRelease.Status, actualRelease.status, "Unexpected name for release with name {0}", expectedRelease.Name);
            Assert.AreEqual(expectedRelease.CreatedOn, actualRelease.createdOn, "Unexpected createdOn for release with name {0}", expectedRelease.Name);
            Assert.AreEqual(expectedRelease.TargetStageId, actualRelease.targetStageId, "Unexpected targetStageId for release with name {0}", expectedRelease.Name);
            Assert.AreEqual(expectedRelease.ReleasePathName, actualRelease.releasePathName, "Unexpected releasePathName for release with name {0}", expectedRelease.Name);
            Assert.IsNotNull(actualRelease.stages, "Unexpected stages collection for release with name {0}", expectedRelease.Name);
            Assert.IsNotNull(actualRelease.components, "Unexpected components collection for release with name {0}", expectedRelease.Name);
        }

        internal static void AssertAreStagesEqual(Stage expectedStage, Models.Environment expectedEnvironment, dynamic actualStage)
        {
            Assert.AreEqual(expectedStage.Id, actualStage.id, "Unexpected id for stage");
            Assert.AreEqual(expectedStage.Name, actualStage.name, "Unexpected name for stage with id {0}", expectedStage.Id);
            Assert.AreEqual(expectedStage.Rank, actualStage.rank, "Unexpected rank for stage with id {0}", expectedStage.Id);
            Assert.AreEqual(expectedEnvironment.Name, actualStage.environment, "Unexpected environment for stage with id {0}", expectedStage.Id);
        }

        internal static void AssertAreStepsEqual(Step expectedStep, dynamic actualStep)
        {
            Assert.AreEqual(expectedStep.Id, actualStep.id, "Unexpected id for stage");
            Assert.AreEqual(expectedStep.Name, actualStep.name, "Unexpected name for stage with id {0}", expectedStep.Id);
            Assert.AreEqual(expectedStep.Status, actualStep.status, "Unexpected status for stage with id {0}", expectedStep.Id);
            Assert.AreEqual(expectedStep.StepRank, actualStep.rank, "Unexpected rank for stage with id {0}", expectedStep.Id);
            Assert.AreEqual(expectedStep.CreatedOn, actualStep.createdOn, "Unexpected createdOn for stage with id {0}", expectedStep.Id);
        }

        internal static void AssertReleasesCollectionContainsRelease(List<dynamic> releases, Release expectedRelease)
        {
            bool releaseIsInCollection = false;
            foreach (var release in releases)
            {
                if (release.name == expectedRelease.Name)
                {
                    releaseIsInCollection = true;
                    break;
                }
            }

            Assert.IsTrue(releaseIsInCollection, "Release with name {0} could not be found", expectedRelease.Name);
        }

        #endregion
    }
}