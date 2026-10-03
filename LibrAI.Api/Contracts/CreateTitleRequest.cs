namespace LibrAI.Api.Contracts
{
    public record CreateTitleRequest(string Name, string ISBN, string Author, string? Description, string? Publisher);
}