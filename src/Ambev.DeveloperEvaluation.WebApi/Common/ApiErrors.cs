using Ambev.DeveloperEvaluation.Common.Errors;

namespace Ambev.DeveloperEvaluation.WebApi.Common
{
    public static class ApiErrors
    {
        public static Error ServerError => new("API.ServerError", "The server encoutered an internal error.");
    }
}
