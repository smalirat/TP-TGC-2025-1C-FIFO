using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.TP.FIFO.Texturas;

public static class TextureManager
{
    public const string ContentFolderTextures = "Textures/";

    public static TextureCube CloudySkyBoxTexture { get; private set; }
    public static Texture2D WoodBox1Texture { get; private set; }
    public static Texture2D WoodBox1NormalMapTexture { get; private set; }
    public static Texture2D WoodBox2Texture { get; private set; }
    public static Texture2D WoodBox2NormalMapTexture { get; private set; }
    public static Texture2D WoodBox3Texture { get; private set; }
    public static Texture2D WoodBox3NormalMapTexture { get; private set; }
    public static Texture2D StonesTexture { get; private set; }
    public static Texture2D StonesNormalMapTexture { get; private set; }
    public static Texture2D DirtTexture { get; private set; }
    public static Texture2D DirtNormalMapTexture { get; private set; }
    public static Texture2D RubberBallTexture { get; private set; }
    public static Texture2D RubberBallNormalMapTexture { get; private set; }
    public static Texture2D MetalBallTexture { get; private set; }
    public static Texture2D MetalBallNormalMapTexture { get; private set; }
    public static Texture2D RockyBallTexture { get; private set; }
    public static Texture2D RockyBallNormalMapTexture { get; private set; }

    public static void Load(ContentManager content)
    {
        CloudySkyBoxTexture = LoadTextureCube(content, "darlingSkybox");

        WoodBox1Texture = LoadTexture2D(content, "wooden-box-1/wooden-box-1");
        WoodBox1NormalMapTexture = LoadTexture2D(content, "wooden-box-1/wooden-box-1-normal");

        WoodBox2Texture = LoadTexture2D(content, "wooden-box-2/wooden-box-2");
        WoodBox2NormalMapTexture = LoadTexture2D(content, "wooden-box-2/wooden-box-2-normal");

        WoodBox3Texture = LoadTexture2D(content, "wooden-box-3/wooden-box-3");
        WoodBox3NormalMapTexture = LoadTexture2D(content, "wooden-box-3/wooden-box-3-normal");

        StonesTexture = LoadTexture2D(content, "stones-wall/stones");
        StonesNormalMapTexture = LoadTexture2D(content, "stones-wall/stones_normal");

        DirtTexture = LoadTexture2D(content, "dirt-floor/dirt");
        DirtNormalMapTexture = LoadTexture2D(content, "dirt-floor/dirt-normal");

        RubberBallTexture = LoadTexture2D(content, "rubber-ball/rubber-ball");
        RubberBallNormalMapTexture = LoadTexture2D(content, "rubber-ball/rubber-ball-normal");

        MetalBallTexture = LoadTexture2D(content, "metal-ball/metal-ball");
        MetalBallNormalMapTexture = LoadTexture2D(content, "metal-ball/metal-ball-normal");

        RockyBallTexture = LoadTexture2D(content, "rocky-ball/rocky-ball");
        RockyBallNormalMapTexture = LoadTexture2D(content, "rocky-ball/rocky-ball-normal");
    }

    private static Texture2D LoadTexture2D(ContentManager content, string path)
    {
        return content.Load<Texture2D>(ContentFolderTextures + path);
    }

    private static TextureCube LoadTextureCube(ContentManager content, string path)
    {
        return content.Load<TextureCube>(ContentFolderTextures + path);
    }
}