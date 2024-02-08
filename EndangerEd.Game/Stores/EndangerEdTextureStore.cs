using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;

namespace EndangerEd.Game.Stores;

/// <inheritdoc />
/// <summary>
/// A store for store texture using <see cref="T:osu.Framework.Graphics.Textures.LargeTextureStore" /> due to the large size of the texture.
/// </summary>
public class EndangerEdTextureStore : LargeTextureStore
{
    public EndangerEdTextureStore(IRenderer renderer, IResourceStore<TextureUpload> store)
        : base(renderer, store)
    {
    }
}
