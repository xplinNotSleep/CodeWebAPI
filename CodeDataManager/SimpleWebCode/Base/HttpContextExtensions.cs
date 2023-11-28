﻿#if !NET45
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
namespace AGSpatialDataCheck.Web.Base
{
    public static class HttpContextExtensions
    {
        #region 拓展方法
        private const string NoCache = "no-cache";
        private const string NoCacheMaxAge = "no-cache,max-age=";
        private const string NoStore = "no-store";
        private const string NoStoreNoCache = "no-store,no-cache";
        private const string PublicMaxAge = "public,max-age=";
        private const string PrivateMaxAge = "private,max-age=";

        /// <summary>
        /// Adds the Cache-Control and Pragma HTTP headers by applying the specified cache profile to the HTTP context.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="cacheProfile">The cache profile.</param>
        /// <returns>The HTTP context</returns>
        /// <exception cref="ArgumentNullException">context or cacheProfile.</exception>
        public static HttpContext ApplyCacheProfile(this HttpContext context, CacheProfile cacheProfile)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (cacheProfile == null)
            {
                throw new ArgumentNullException(nameof(cacheProfile));
            }

            var headers = context.Response.Headers;

            if (!string.IsNullOrEmpty(cacheProfile.VaryByHeader))
            {
                headers[HeaderNames.Vary] = cacheProfile.VaryByHeader;
            }

            if (cacheProfile.NoStore == true)
            {
                // Cache-control: no-store, no-cache is valid.
                if (cacheProfile.Location == ResponseCacheLocation.None)
                {
                    headers[HeaderNames.CacheControl] = NoStoreNoCache;
                    headers[HeaderNames.Pragma] = NoCache;
                }
                else
                {
                    headers[HeaderNames.CacheControl] = NoStore;
                }
            }
            else
            {
                string cacheControlValue = null;
                var duration = cacheProfile.Duration.GetValueOrDefault().ToString(CultureInfo.InvariantCulture);
                switch (cacheProfile.Location)
                {
                    case ResponseCacheLocation.Any:
                        cacheControlValue = PublicMaxAge + duration;
                        break;
                    case ResponseCacheLocation.Client:
                        cacheControlValue = PrivateMaxAge + duration;
                        break;
                    case ResponseCacheLocation.None:
                        cacheControlValue = NoCacheMaxAge + duration;
                        headers[HeaderNames.Pragma] = NoCache;
                        break;
                    default:
                        var exception = new NotImplementedException($"Unknown {nameof(ResponseCacheLocation)}: {cacheProfile.Location}");
                        Debug.Fail(exception.ToString());
                        throw exception;
                }

                headers[HeaderNames.CacheControl] = cacheControlValue;
            }

            return context;
        }
        public static Uri GetUrl(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            if (string.IsNullOrWhiteSpace(request.Scheme))
                throw new ArgumentException("Http request Scheme is not specified");
            if (!request.Host.HasValue)
                throw new ArgumentException("Http request Host is not specified");
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(request.Scheme).Append("://").Append(request.Host);
            if (request.Path.HasValue)
                stringBuilder.Append(request.Path.Value);
            if (request.QueryString.HasValue)
                stringBuilder.Append(request.QueryString);
            return new Uri(stringBuilder.ToString());
        }

        public static string GetAbsoluteUri(this HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
        }
        public static IHeaderDictionary GetServerVariables(this HttpRequest request)
        {
            return request.Headers;
        }
        public static string GetUserIp(this HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            if (string.IsNullOrEmpty(ip))
                return "127.0.0.1";
            return ip;


        }

        public static string GetTraceId(this HttpContext context)
        {
            return context.TraceIdentifier;

        }
        #endregion


    }

}
#endif