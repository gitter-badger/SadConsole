using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace SadConsole
{
    public class Font
    {
        public string Name { get; set; }

        public string FilePath { get; set; }

        public int CellHeight { get; set; }

        public int CellWidth { get; set; }

        public int CellPadding { get; set; }

        public bool IsDefault { get; set; }

        [IgnoreDataMember]
        public int Rows { get { return Image.Height / (CellHeight + CellPadding); } }

        [IgnoreDataMember]
        public Texture2D Image { get; private set; }

        [IgnoreDataMember]
        public Rectangle[] CharacterIndexRects;

        #region Constructors
        public Font() { }

        public Font(string name, XElement fontXmlNode, GraphicsDevice device)
        {
            XAttribute xwidth = fontXmlNode.Attribute("width");
            XAttribute xheight = fontXmlNode.Attribute("height");
            string filename = fontXmlNode.Value;

            if (xwidth == null || xheight == null)
                throw new Exception("Width or Height attribute for font is missing");

            int width;
            int height;

            if (!int.TryParse(xwidth.Value, out width))
                throw new Exception("Width value is invalid: " + xwidth.Value);
            if (!int.TryParse(xheight.Value, out height))
                throw new Exception("Height value is invalid: " + xheight.Value);

            FilePath = filename;
            Name = name;
            CellWidth = width;
            CellHeight = height;

            Generate();
        }
        #endregion

        #region Methods
        /// <summary>
        /// After the font has been loaded, (with the FilePath, CellHeight, and CellWidth fields filled out) this method will create the actual texture.
        /// </summary>
        public void Generate()
        {
            using (System.IO.Stream fontStream = System.IO.File.OpenRead(FilePath))
           {
                Image = Texture2D.FromStream(Engine.Device, fontStream);
            }

            ConfigureRects();
        }

        public void ConfigureRects()
        {
            CharacterIndexRects = new Rectangle[Rows * Engine.FontColumns];

            for (int i = 0; i < CharacterIndexRects.Length; i++)
            {
                var cx = i % Engine.FontColumns;
                var cy = i / Engine.FontColumns;

                if (CellPadding != 0)
                    CharacterIndexRects[i] = new Rectangle((cx * CellWidth) + ((cx + 1) * CellPadding),
                                                           (cy * CellHeight) + ((cy + 1) * CellPadding), CellWidth, CellHeight);
                else
                    CharacterIndexRects[i] = new Rectangle(cx * CellWidth, cy * CellHeight, CellWidth, CellHeight);
            }
        }

        private void GetImageMask()
        {
            Texture2D texture = new Texture2D(Engine.Device, Image.Width, Image.Height,
                                                false, SurfaceFormat.Color);
            Color[] newPixels = new Color[texture.Width * texture.Height];
            Color[] oldPixels = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(newPixels);
            Image.GetData<Color>(oldPixels);
        }

        /// <summary>
        /// Resizes the graphics device manager to this font cell size.
        /// </summary>
        /// <param name="manager">Graphics device manager to resize.</param>
        /// <param name="width">The width in cell count.</param>
        /// <param name="height">The height in cell count.</param>
        /// <param name="additionalWidth">Additional pixel width to add to the resize.</param>
        /// <param name="additionalHeight">Additional pixel height to add to the resize.</param>
        public void ResizeGraphicsDeviceManager(GraphicsDeviceManager manager, int width, int height, int additionalWidth, int additionalHeight)
        {
            manager.PreferredBackBufferWidth = (CellWidth * width) + additionalWidth;
            manager.PreferredBackBufferHeight = (CellHeight * height) + additionalHeight;
            manager.ApplyChanges();

            Engine.WindowWidth = manager.PreferredBackBufferWidth;
            Engine.WindowHeight = manager.PreferredBackBufferHeight;
        }

        [OnDeserialized]
        private void AfterDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            var specificFont = Engine.Fonts[Name, CellWidth, CellHeight];

            if (specificFont != null)
            {
                Image = specificFont.Image;
                ConfigureRects();
            }
            else
                foreach (var font in Engine.Fonts[Name])
                {
                    Image = font.Image;
                    ConfigureRects();
                    break;
                }

            // Existing font was not found, try to load the one specified by this font.
            if (Image == null)
                Generate();
        }
        #endregion
    }

    public sealed class Font2
    {
        public Texture2D FontImage { get; private set; }

        public Point FontSize { get; private set; }

        public int MaxCharacter { get; private set; }

        public Rectangle[] CharacterIndexRects { get; private set; }

        internal Font2(FontMaster masterFont, int fontMultiple)
        {
            FontImage = masterFont.Image;
            MaxCharacter = masterFont.Rows * Engine.FontColumns - 1;
            FontSize = new Point(masterFont.CellWidth * fontMultiple, masterFont.CellHeight * fontMultiple);
            CharacterIndexRects = new Rectangle[masterFont.CharacterIndexRects.Length];
            masterFont.CharacterIndexRects.CopyTo(CharacterIndexRects, 0);
        }
    }

    public class FontMaster
    {
        public string Name { get; set; }

        public string FilePath { get; set; }

        public int CellHeight { get; set; }

        public int CellWidth { get; set; }

        public int CellPadding { get; set; }

        public bool IsDefault { get; set; }

        [IgnoreDataMember]
        public int Rows { get { return Image.Height / (CellHeight + CellPadding); } }

        [IgnoreDataMember]
        public Texture2D Image { get; private set; }

        [IgnoreDataMember]
        public Rectangle[] CharacterIndexRects;

        #region Methods
        /// <summary>
        /// After the font has been loaded, (with the FilePath, CellHeight, and CellWidth fields filled out) this method will create the actual texture.
        /// </summary>
        public void Generate()
        {
            using (System.IO.Stream fontStream = System.IO.File.OpenRead(FilePath))
            {
                Image = Texture2D.FromStream(Engine.Device, fontStream);
            }

            ConfigureRects();
        }

        public void ConfigureRects()
        {
            CharacterIndexRects = new Rectangle[Rows * Engine.FontColumns];

            for (int i = 0; i < CharacterIndexRects.Length; i++)
            {
                var cx = i % Engine.FontColumns;
                var cy = i / Engine.FontColumns;

                if (CellPadding != 0)
                    CharacterIndexRects[i] = new Rectangle((cx * CellWidth) + ((cx + 1) * CellPadding),
                                                           (cy * CellHeight) + ((cy + 1) * CellPadding), CellWidth, CellHeight);
                else
                    CharacterIndexRects[i] = new Rectangle(cx * CellWidth, cy * CellHeight, CellWidth, CellHeight);
            }
        }

        /// <summary>
        /// Gets a sized font.
        /// </summary>
        /// <param name="multiple">How much to multiple the font size by.</param>
        /// <returns>A font.</returns>
        public Font2 GetFont(int multiple = 1)
        {
            return new Font2(this, multiple);
        }


        private void GetImageMask()
        {
            Texture2D texture = new Texture2D(Engine.Device, Image.Width, Image.Height,
                                                false, SurfaceFormat.Color);
            Color[] newPixels = new Color[texture.Width * texture.Height];
            Color[] oldPixels = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(newPixels);
            Image.GetData<Color>(oldPixels);
        }

        /// <summary>
        /// Resizes the graphics device manager to this font cell size.
        /// </summary>
        /// <param name="manager">Graphics device manager to resize.</param>
        /// <param name="width">The width in cell count.</param>
        /// <param name="height">The height in cell count.</param>
        /// <param name="additionalWidth">Additional pixel width to add to the resize.</param>
        /// <param name="additionalHeight">Additional pixel height to add to the resize.</param>
        public void ResizeGraphicsDeviceManager(GraphicsDeviceManager manager, int width, int height, int additionalWidth, int additionalHeight)
        {
            manager.PreferredBackBufferWidth = (CellWidth * width) + additionalWidth;
            manager.PreferredBackBufferHeight = (CellHeight * height) + additionalHeight;
            manager.ApplyChanges();

            Engine.WindowWidth = manager.PreferredBackBufferWidth;
            Engine.WindowHeight = manager.PreferredBackBufferHeight;
        }

        [OnDeserialized]
        private void AfterDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            var specificFont = Engine.Fonts[Name, CellWidth, CellHeight];

            if (specificFont != null)
            {
                Image = specificFont.Image;
                ConfigureRects();
            }
            else
                foreach (var font in Engine.Fonts[Name])
                {
                    Image = font.Image;
                    ConfigureRects();
                    break;
                }

            // Existing font was not found, try to load the one specified by this font.
            if (Image == null)
                Generate();
        }
        #endregion
    }
}
