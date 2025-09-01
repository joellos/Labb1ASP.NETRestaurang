//using Labb1ASP.NETDatabas.Extensions;
//using System.Diagnostics;

//namespace Labb1ASP.NETDatabas.Middleware
//{
//    public class RequestLoggingMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly RequestLoggingService _loggingService;

//        public RequestLoggingMiddleware(RequestDelegate next, RequestLoggingService loggingService)
//        {
//            _next = next;
//            _loggingService = loggingService;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            var stopwatch = Stopwatch.StartNew();

//            try
//            {
//                await _next(context);
//            }
//            finally
//            {
//                stopwatch.Stop();
//                _loggingService.LogRequest(context, stopwatch.Elapsed);
//            }
//        }
//    }
//}