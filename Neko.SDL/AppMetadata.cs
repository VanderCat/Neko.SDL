namespace Neko.Sdl;

/// <summary>
/// You can optionally provide metadata about your app to SDL. This is not required, but strongly encouraged.
/// <br/><br/>
/// There are several locations where SDL can make use of metadata (an "About" box in the macOS menu bar, the name
/// of the app can be shown on some audio mixers, etc). Any piece of metadata can be left out, if a specific detail
/// doesn't make sense for the app.
/// <br/><br/>
/// This function should be called as early as possible, before <see cref="NekoSDL.Init"/>. Multiple calls to this function are allowed,
/// but various state might not change once it has been set up with a previous call to this function.
/// </summary>
public static class AppMetadata {
    
    /// <summary>
    /// This is a simplified interface for the most important information.
    /// You can supply significantly more detailed metadata with Properties
    /// </summary>
    /// <param name="title">The name of the application ("My Game 2: Bad Guy's Revenge!")</param>
    /// <param name="version"> 	The version of the application ("1.0.0beta5" or a git hash, or whatever makes sense)</param>
    /// <param name="id">A unique string in reverse-domain format that identifies this app ("com.example.mygame2")</param>
    /// <remarks>
    /// This function should be called as early as possible, before <see cref="NekoSDL.Init"/>. Multiple calls to this function are allowed,
    /// but various state might not change once it has been set up with a previous call to this function.
    /// <br/><br/>
    /// Passing a NULL removes any previous metadata.
    /// <br/><br/>
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static void Set(string? title, string? version, string? id) =>
        SDL_SetAppMetadata(title, version, id);
    
    /// <summary>
    /// The human-readable name of the application, like "My Game 2: Bad Guy's Revenge!".
    /// This will show up anywhere the OS shows the name of the application separately from window titles,
    /// such as volume control applets, etc. This defaults to "SDL Application".
    /// </summary>
    public static string? Name {
        get => SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_NAME_STRING);
        set => SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_NAME_STRING, value);
    }
    
    /// <summary>
    /// The version of the app that is running; there are no rules on format, so "1.0.3beta2" and "April 22nd, 2024"
    /// and a git hash are all valid options. This has no default.
    /// </summary>
    public static string? Version {
        get => SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_VERSION_STRING);
        set => SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_VERSION_STRING, value);
    }
    
    /// <summary>
    /// A unique string that identifies this app. This must be in reverse-domain format, like "com.example.mygame2".
    /// This string is used by desktop compositors to identify and group windows together, as well as match applications
    /// with associated desktop settings and icons. If you plan to package your application in a container such as
    /// Flatpak, the app ID should match the name of your Flatpak container as well. This has no default.
    /// </summary>
    public static string? Identifier {
        get => SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_IDENTIFIER_STRING);
        set => SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_IDENTIFIER_STRING, value);
    }
    
    /// <summary>
    /// The human-readable name of the creator/developer/maker of this app, like "MojoWorkshop, LLC"
    /// </summary>
    public static string? Creator {
        get => SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_CREATOR_STRING);
        set => SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_CREATOR_STRING, value);
    }
    
    /// <summary>
    /// The human-readable copyright notice, like "Copyright (c) 2024 MojoWorkshop, LLC" or whatnot.
    /// Keep this to one line, don't paste a copy of a whole software license in here. This has no default.
    /// </summary>
    public static string? Copyright {
        get => SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_COPYRIGHT_STRING);
        set => SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_COPYRIGHT_STRING, value);
    }
    
    /// <summary>
    /// A URL to the app on the web. Maybe a product page, or a storefront, or even a GitHub repository, for user's
    /// further information This has no default.
    /// </summary>
    public static string? Url {
        get => SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_URL_STRING);
        set => SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_URL_STRING, value);
    }
    
    /// <summary>
    /// The type of application this is. Currently this string can be "game" for a video game, "mediaplayer"
    /// for a media player, or generically "application" if nothing else applies. Future versions of SDL might add
    /// new types. This defaults to "application".
    /// </summary>
    public static string? Type {
        get => SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_TYPE_STRING);
        set => SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_TYPE_STRING, value);
    }
}