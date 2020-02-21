using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;

namespace ShopifySharp
{
    /// <summary>
    /// An exception thrown when an API call has reached Shopify's rate limit.
    /// </summary>
    public class ShopifyRateLimitException : ShopifyException
    {
        public int BucketFill { get; set; }

        public int BucketCapacity { get; set; }

        public ShopifyRateLimitException() : base() { }

        public ShopifyRateLimitException(string message): base(message) { }

        public ShopifyRateLimitException(HttpStatusCode httpStatusCode, Dictionary<string, IEnumerable<string>> errors, string message, string jsonError, string requestId, HttpResponseHeaders headers) : base(httpStatusCode, errors, message, jsonError, requestId)
        {
            var apiCallLimitHeaderValue = headers.FirstOrDefault(kvp => kvp.Key == "X-Shopify-Shop-Api-Call-Limit").Value?.FirstOrDefault();
            if (apiCallLimitHeaderValue == null)
            {
                BucketFill = -1;
                BucketCapacity = -1;
                return;
            }

            var split = apiCallLimitHeaderValue.Split('/');
            if (split.Length == 2 &&
                int.TryParse(split[0], out int currentFillLevel) &&
                int.TryParse(split[1], out int capacity))
            {
                BucketFill = currentFillLevel;
                BucketCapacity = capacity;
            }
        }
    }
}
