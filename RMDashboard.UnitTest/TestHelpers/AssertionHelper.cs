using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Http;

namespace RMDashboard.UnitTest.TestHelpers
{
    static class AssertionHelper
    {
        internal static void AssertHttpResponseMessageStatus(object responseMessage, HttpStatusCode expectedStatus)
        {
            Assert.IsNotNull(responseMessage, "Response messages should not be null");
            Assert.IsInstanceOfType(responseMessage, typeof(HttpResponseMessage), "Unexpected type");

            var httpResponseMessage = (HttpResponseMessage)responseMessage;
            Assert.AreEqual(expectedStatus, httpResponseMessage.StatusCode, "Unexpected status");
        }

        internal static void AssertDateTimeIsNow(dynamic expectedDateTime)
        {
            Assert.IsInstanceOfType(expectedDateTime, typeof(DateTime), "Unexpected type for date time");

            DateTime dateTime = (DateTime)expectedDateTime;
            //check that date time is within the range between (now - 1 minute) and now
            Assert.IsTrue(dateTime >= DateTime.Now.AddMinutes(-1), "Date time {0} is to early to be now", dateTime);
            Assert.IsTrue(dateTime <= DateTime.Now, "Date time {0} is to late to be now", dateTime);
        }
    }
}
