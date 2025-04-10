using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using ExpennyApi.Services;
using ExpennyApi.Repositories;
using ExpennyApi.DTOs;
using System.Security.Claims;

namespace ExpennyApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly ISubscriptionRepository _repo;
        private readonly AnalyticsService _analytics;

        public AnalyticsController(ISubscriptionRepository repo, AnalyticsService analytics)
        {
            _repo = repo;
            _analytics = analytics;
        }

        private string GetUserIdOrThrow()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                throw new UnauthorizedAccessException("User ID not found in token.");
            return userId;
        }


        [HttpGet]
        public ActionResult<SubscriptionAnalyticsDTO> GetAnalytics()
        {
            try
            {
                var userId = GetUserIdOrThrow();
                var subs = _repo.GetByUserId(userId).ToList();
                var result = _analytics.CalculateMetrics(subs);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

    }
}
