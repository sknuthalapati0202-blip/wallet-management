using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using WalletManagement.Core.Domain.Services;
using WalletManagement.Core.Domain.Services.Communication;
using WalletManagement.Core.Utilities;

namespace WalletManagement.Controllers
{
    [Route("api/[controller]")]
    public class CategoryController : BaseApiController
    {
        private readonly ICategoryService _categoryService;
        private readonly IConfiguration Configuration;
        private readonly ILogger<CategoryController> _logger;
        private readonly IGlobalConfiguration _globalConfiguration;
        private readonly MessageConstants Constants;
        private readonly OIDCConstants OIDCConstants;
        private readonly IMessageLocalizer _messageLocalizer;

        public CategoryController(ICategoryService categoryService, IConfiguration configuration,
            ILogger<CategoryController> logger, IGlobalConfiguration globalConfiguration, IMessageLocalizer messageLocalizer)
        {
            _categoryService = categoryService;
            _logger = logger;
            Configuration = configuration;
            _messageLocalizer = messageLocalizer;
            _globalConfiguration = globalConfiguration;

            var errorConfiguration = _globalConfiguration.GetErrorConfiguration();
            if (null == errorConfiguration)
            {
                _logger.LogError("Get Error Configuration failed");
                throw new ArgumentNullException(nameof(errorConfiguration)); // Changed to ArgumentNullException for better practice
            }

            Constants = errorConfiguration.Constants;
            if (null == Constants)
            {
                _logger.LogError("Get Error Configuration failed");
                throw new ArgumentNullException(nameof(Constants));
            }

            OIDCConstants = errorConfiguration.OIDCConstants;
            if (null == OIDCConstants)
            {
                _logger.LogError("Get Error Configuration failed");
                throw new ArgumentNullException(nameof(OIDCConstants));
            }
        }

        [Route("GetCategoryList")]
        [HttpGet]
        // OVERRIDE: Tell Swagger that this specific endpoint returns ErrorResponseDTO for 401, not the default ProblemDetails
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCategoryList()
        {
            var authHeaderName = Configuration["AccessTokenHeaderName"] ?? "Authorization";
            var authHeader = Request.Headers[authHeaderName];

            if (string.IsNullOrEmpty(authHeader))
            {
                ErrorResponseDTO errResponse = new()
                {
                    error = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken),
                    error_description = _messageLocalizer.GetMessage(OIDCConstants.InvalidAuthZHeader)
                };
                return Unauthorized(errResponse);
            }

            // FIX: Use TryParse to prevent 500 Exceptions when Schemathesis fuzzes the Auth header with garbage strings
            if (!AuthenticationHeaderValue.TryParse(authHeader, out var authHeaderVal) ||
                string.IsNullOrEmpty(authHeaderVal.Scheme) ||
                string.IsNullOrEmpty(authHeaderVal.Parameter))
            {
                _logger.LogError("Invalid scheme or parameter in Authorization header");
                ErrorResponseDTO errResponse = new()
                {
                    error = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken),
                    error_description = _messageLocalizer.GetMessage(OIDCConstants.InvalidAuthZHeader)
                };
                return Unauthorized(errResponse);
            }

            // Check the authorization is of Bearer type
            if (!authHeaderVal.Scheme.Equals("bearer", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError($"Token is not Bearer token type. Received {authHeaderVal.Scheme} type");
                ErrorResponseDTO errResponse = new()
                {
                    error = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken),
                    error_description = _messageLocalizer.GetMessage(OIDCConstants.InvalidAuthZHeader)
                };
                return Unauthorized(errResponse);
            }

            APIResponse response = new APIResponse();
            try
            {
                var response1 = await _categoryService.GetCategoryListAsync();
                if (response1 == null)
                {
                    response.Success = false;
                    response.Message = _messageLocalizer.GetMessage(OIDCConstants.InternalError);
                    return Ok(response);
                }
                response.Success = response1.Success;
                response.Message = response1.Message;
                response.Result = response1.Resource;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching category list."); // Good practice to log the actual exception
                response.Success = false;
                response.Message = _messageLocalizer.GetMessage(OIDCConstants.InternalError);
                return Ok(response);
            }
        }

        [Route("GetCategoryNameAndIdList")]
        [HttpGet]
        public async Task<IActionResult> GetCategoryNameAndIdList()
        {
            var result = await _categoryService.GetCategoryNameAndIdListAsync();
            var apiResponse = new APIResponse()
            {
                Success = result.Success,
                Message = result.Message,
                Result = result.Resource
            };
            return Ok(apiResponse);
        }

        [Route("GetCategoryNamebyUId/{categoryId:guid}")]
        [HttpGet]
        public async Task<IActionResult> GetCategoryNamebyUId(Guid categoryId)
        {
            // FIX: Removed if(!ModelState.IsValid). [ApiController] handles malformed GUIDs automatically 
            // by returning a 400 ValidationProblemDetails, which matches our BaseApiController definition.

            var result = await _categoryService.GetCategoryNamebyUIdAsync(categoryId.ToString());
            if (result == null)
            {
                var apiResponse1 = new APIResponse()
                {
                    Success = false,
                    Message = $"No category present with {categoryId} ID",
                    Result = null
                };
                return Ok(apiResponse1);
            }
            else
            {
                var apiResponse = new APIResponse()
                {
                    Success = true,
                    Message = "Successfully retrieved category name",
                    Result = result
                };
                return Ok(apiResponse);
            }
        }
    }
}