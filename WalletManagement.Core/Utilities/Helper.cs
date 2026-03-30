using Microsoft.Extensions.Logging;
using WalletManagement.Core.Constants;
using WalletManagement.Core.Domain.Services.Communication;

namespace WalletManagement.Core.Utilities
{
    public class Helper : IHelper
    {
        private readonly ILogger<Helper> _logger;
        private readonly IGlobalConfiguration _globalConfiguration;
        private readonly MessageConstants Constants;
        private readonly IMessageLocalizer _messageLocalizer;

        public Helper(ILogger<Helper> logger,
            IGlobalConfiguration globalConfiguration,
            IMessageLocalizer messageLocalizer)
        {
            _logger = logger;
            _globalConfiguration = globalConfiguration;

            _logger.LogDebug("-->Helper");

            var errorConfiguration = _globalConfiguration.GetErrorConfiguration();
            if (null == errorConfiguration)
            {
                _logger.LogError("Get Error Configuration failed");
                throw new NullReferenceException();
            }

            Constants = errorConfiguration.Constants;
            if (null == Constants)
            {
                _logger.LogError("Get Error Configuration failed");
                throw new NullReferenceException();
            }

            _logger.LogDebug("<--Helper");
            _messageLocalizer = messageLocalizer;
        }

        public string GetErrorMsg(uint code, string message = null)
        {
            // Initialize Error Message
            var errorMessage = string.Empty;

            if (null == message)
                message = _messageLocalizer.GetMessage(Constants.InternalError);

            try
            {
                errorMessage = String.Format("{0}(Code:{1})", message, code);
            }
            catch (Exception ex)
            {
                _logger.LogError("String.Format() failed: {0}", ex.Message);
                errorMessage = _messageLocalizer.GetMessage(Constants.InternalError);
            }

            return errorMessage;
        }

        public string GetRedisErrorMsg(int code, uint default_code)
        {
            // Initialize Error Message
            var errorMessage = string.Empty;
            uint errorCode;

            if (code == CacheCodes.TimeoutException)
            {
                errorCode = ErrorCodes.REDIS_TIMEOUT_EXCEPTION;
            }
            else if (code == CacheCodes.ConnectionException)
            {
                errorCode = ErrorCodes.REDIS_CONNECTION_EXCEPTION;
            }
            else if (code == CacheCodes.CommandException)
            {
                errorCode = ErrorCodes.REDIS_COMMAND_EXCEPTION;
            }
            else
            {
                errorCode = default_code;
            }

            errorMessage = GetErrorMsg(errorCode);
            return errorMessage;
        }

    }
}
