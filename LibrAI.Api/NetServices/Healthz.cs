namespace LibrAI.Api.NetServices
{

    public static class HealthzService
    {
        public static HealthzResponse GetHealthz()
        {
            return new HealthzResponse("ok", DateTime.UtcNow);
        }
    }

    public record HealthzResponse(string Status, DateTime CheckedAt);
}