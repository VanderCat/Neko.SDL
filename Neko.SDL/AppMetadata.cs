namespace Neko.Sdl;

public static class AppMetadata {
    public static void Set(string? title, string? version, string? id) =>
        SDL_SetAppMetadata(title, version, id);
    
    public static string? Name {
        get => SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_NAME_STRING);
        set => SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_NAME_STRING, value);
    }
    public static string? Version {
        get => SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_VERSION_STRING);
        set => SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_VERSION_STRING, value);
    }
    public static string? Identifier {
        get => SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_IDENTIFIER_STRING);
        set => SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_IDENTIFIER_STRING, value);
    }
    public static string? Creator {
        get => SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_CREATOR_STRING);
        set => SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_CREATOR_STRING, value);
    }
    public static string? Copyright {
        get => SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_COPYRIGHT_STRING);
        set => SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_COPYRIGHT_STRING, value);
    }
    public static string? Url {
        get => SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_URL_STRING);
        set => SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_URL_STRING, value);
    }
    public static string? Type {
        get => SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_TYPE_STRING);
        set => SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_TYPE_STRING, value);
    }
}