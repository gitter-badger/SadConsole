namespace SadConsole.ConsolesNS
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// Represents the ability to render cell data to the screen.
    /// </summary>
    public interface IRender
    {
        /// <summary>
        /// The SpriteBatch used when rendering cell data.
        /// </summary>
        SpriteBatch Batch { get; }

        /// <summary>
        /// Renders the cell data to the screen.
        /// </summary>
        void Render(CellSurfaceSubset cells, Point position, bool usePixelPositioning = false);

        /// <summary>
        /// Renders the cell data to the screen.
        /// </summary>
        void Render(CellSurfaceSubset cells, Matrix renderingMatrix);
    }

    public interface IConsoleDataRenderData
    {
        Rectangle AbsoluteArea { get; }
        Rectangle Area { get; }
        Rectangle[] RenderRects { get; }
        Cell[] RenderCells { get; }

        Font2 Font { get; }
        Color DefaultBackground { get; }
        Color DefaultForeground { get; }
        Color Tint { get; }
    }

    public class CellSurfaceSubset: IConsoleDataRenderData
    {
        private ConsoleData data;

        public Rectangle Area { get; private set; }
        public Rectangle[] RenderRects { get; private set; }
        public Cell[] RenderCells { get; private set; }
        public Rectangle AbsoluteArea { get; private set; }

        public Font2 Font { get { return data.Font; } }
        public Color DefaultBackground { get { return data.DefaultBackground; } }
        public Color DefaultForeground { get { return data.DefaultForeground; } }

        public Color Tint { get { return data.Tint; } }


        public CellSurfaceSubset(ConsoleData surface, Rectangle area)
        {
            data = surface;

            if (area.Width > surface.Width)
                throw new ArgumentOutOfRangeException("area", "The area is too wide for the surface.");
            if (area.Height > surface.Height)
                throw new ArgumentOutOfRangeException("area", "The area is too tall for the surface.");

            if (area.X < 0)
                throw new ArgumentOutOfRangeException("area", "The left of the area cannot be less than 0.");
            if (area.Y < 0)
                throw new ArgumentOutOfRangeException("area", "The top of the area cannot be less than 0.");

            if (area.X + area.Width > surface.Width)
                throw new ArgumentOutOfRangeException("area", "The area x + width is too wide for the surface.");
            if (area.Y + area.Height > surface.Height)
                throw new ArgumentOutOfRangeException("area", "The area y + height is too tal for the surface.");

            RenderRects = new Rectangle[area.Width * area.Height];
            RenderCells = new Cell[area.Width * area.Height];
            Area = area;

            int index = 0;

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    RenderRects[index] = new Rectangle(x * Font.FontSize.X, y * Font.FontSize.Y, Font.FontSize.X, Font.FontSize.Y);
                    RenderCells[index] = surface[(y + area.Top) * surface.Width + (x + area.Left)];
                    index++;
                }
            }

            AbsoluteArea = new Rectangle(0, 0, area.Width * Font.FontSize.X, area.Height * Font.FontSize.Y);
        }

        public CellSurfaceSubset() { }
    }

}
