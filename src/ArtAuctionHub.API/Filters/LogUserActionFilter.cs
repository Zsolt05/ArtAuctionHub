using Microsoft.AspNetCore.Mvc.Filters;

namespace ArtAuctionHub.API.Filters
{
    /// <summary>
    /// An action filter that logs user actions before and after an action method is executed.
    /// </summary>
    public class LogUserActionFilter(ILogger<LogUserActionFilter> logger) : IActionFilter
    {
        /// <summary>
        /// Called before the action method is executed.
        /// </summary>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("Action {Action} executing for user.", context.ActionDescriptor.DisplayName);
        }

        /// <summary>
        /// Called after the action method is executed.
        /// </summary>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation("Action {Action} executed.", context.ActionDescriptor.DisplayName);
        }
    }
}
