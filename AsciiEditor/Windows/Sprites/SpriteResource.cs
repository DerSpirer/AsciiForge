using System.Text.Json;

namespace AsciiEditor.Windows.Sprites;

public class SpriteResource
{
    public bool isPlaying;
    public float clipLength;
    public int startFrame;
    public TextureResource[] textures;

    public static async Task<SpriteResource?> Read(string path)
    {
        SpriteResource? spriteResource = null;
        try
        {
            await using FileStream fileStream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read);
            spriteResource = await JsonSerializer.DeserializeAsync<SpriteResource>(fileStream);
        }
        catch (Exception exception)
        {
            // NOAM TODO Add some sort of feedback
        }
        return spriteResource;
    }
    public async Task<bool> Write(string path)
    {
        bool success = false;
        try
        {
            await using FileStream fileStream = System.IO.File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
            await JsonSerializer.SerializeAsync(fileStream, this);
            success = true;
        }
        catch (Exception exception)
        {
            // NOAM TODO Add some sort of feedback
        }
        return success;
    }
}