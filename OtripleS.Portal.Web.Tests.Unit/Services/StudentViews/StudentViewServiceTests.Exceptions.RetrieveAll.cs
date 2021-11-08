﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE AS LONG AS SOFTWARE FUNDS ARE DONATED TO THE POOR
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using OtripleS.Portal.Web.Models.Students;
using OtripleS.Portal.Web.Models.Students.Exceptions;
using OtripleS.Portal.Web.Models.StudentViews;
using OtripleS.Portal.Web.Models.StudentViews.Exceptions;
using OtripleS.Portal.Web.Models.TeacherViews.Exceptions;
using OtripleS.Portal.Web.Models.TeacherViews;
using Xunit;

namespace OtripleS.Portal.Web.Tests.Unit.Services.StudentViews
{
    public partial class StudentViewServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowStudentViewDependencyExceptionIfDependecyErrorOccursAndLogItAsync(
           Exception dependencyException)
        {
            // given
            var expectedStudentViewDependencyException =
                new StudentViewDependencyException(dependencyException);

            this.studentServiceMock.Setup(service =>
                service.RetrieveAllStudentsAsync())
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<List<StudentView>> retrieveAllStudentsTask =
                this.studentViewService.RetrieveAllStudentsViewAsync();

            // then
            await Assert.ThrowsAsync<StudentViewDependencyException>(() =>
                retrieveAllStudentsTask.AsTask());

            this.studentServiceMock.Verify(service =>
                service.RetrieveAllStudentsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStudentViewDependencyException))),
                        Times.Once);

            this.studentServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
