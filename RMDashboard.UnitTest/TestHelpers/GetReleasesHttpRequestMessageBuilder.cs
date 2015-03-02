using System.Net.Http;

namespace RMDashboard.UnitTest.TestHelpers
{
    class GetReleasesHttpRequestMessageBuilder
    {
        private HttpRequestMessage _httpRequestMessage;

        public GetReleasesHttpRequestMessageBuilder()
        {
            _httpRequestMessage = new HttpRequestMessage();
        }

        public GetReleasesHttpRequestMessageBuilder WithIncludedReleasePathIdsHeader(string includedReleasePathIds)
        {
            _httpRequestMessage.Headers.Add("includedReleasePathIds", includedReleasePathIds);
            return this;
        }

        public GetReleasesHttpRequestMessageBuilder WithReleaseCountHeader(int releaseCount)
        {
            _httpRequestMessage.Headers.Add("releaseCount", releaseCount.ToString());
            return this;
        }

        public GetReleasesHttpRequestMessageBuilder WithShowComponentsHeader(bool showComponents)
        {
            _httpRequestMessage.Headers.Add("showComponents", showComponents.ToString());
            return this;
        }

        public HttpRequestMessage Build()
        {
            return _httpRequestMessage;
        }
    }
}
