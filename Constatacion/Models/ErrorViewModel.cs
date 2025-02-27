using Microsoft.AspNetCore.Mvc;

namespace Constatacion.Models
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public string MensajeError { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public bool ShowDetail { get; set; } = true;
        private ILogger logger;

        public ErrorViewModel(ILoggerFactory log)
        {
            // logger
            logger = log.CreateLogger(typeof(ErrorViewModel));
        }

    }
}