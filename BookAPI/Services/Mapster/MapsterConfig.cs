namespace BookAPI.Services.Mapster;

public class MapsterConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<Review, ReviewResponse>.NewConfig()
            .Map(dest => dest.UserName, src => src.User.UserName)
            .Map(dest => dest.BookTitle, src => src.Book.Title);
    }
}