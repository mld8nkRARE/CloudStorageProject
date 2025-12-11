namespace Server.Dtos.Auth
{
    public record AuthResponseDto(
        string Token,
        DateTime ExpiresAt,
        Guid UserId,
        string Nickname
    );
}
