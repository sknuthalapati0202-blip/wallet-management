using System.Text.Json.Serialization;

namespace WalletManagement.Core.Domain.Services.Communication
{
    // It's good practice to add these attributes to your base response DTOs as well.
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class APIResponse
    {
        public bool Success { get; set; }

        // FIX: Marked as nullable to match the schema and modern .NET practices.
        public string? Message { get; set; }

        // FIX: Marked as nullable. This is the key change that will fix the "Case Generation Failed" errors.
        public object? Result { get; set; }

        public APIResponse(string message)
        {
            this.Success = false;
            this.Message = message;
            this.Result = null;
        }

        public APIResponse()
        {
        }
    }

    // No changes needed below this line, but included for context.
    public class BooleanResponse : BaseResponse<bool>
    {
        public BooleanResponse(bool category) : base(category) { }
        public BooleanResponse(string message) : base(message) { }
    }
}