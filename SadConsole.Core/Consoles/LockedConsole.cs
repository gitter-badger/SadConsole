using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Input;

namespace SadConsole.Core.Consoles
{
    /// <summary>
    /// A highly effecient rendering console. Useful for when you have a big mostly transparent console that has immutable data. For example, a border console sitting around another console.
    /// </summary>
    class LockedConsole : IConsole
    {
        private Point cellSize;
        private Cell[] cellsRendered;
        private CellSurface originalSurface;

        public SpriteBatch Batch { get; protected set; }

        public bool CanFocus { get { return false; } set { } }

        public bool CanUseKeyboard { get { return false; } set { } }

        public bool CanUseMouse { get { return false; } set { } }

        public CellSurface CellData { get { return originalSurface; } set { originalSurface = value; CalculateRenderArea(); } }

        public Point CellSize
        {
            get
            {
                return cellSize;
            }
            set
            {
                cellSize = value;
                CalculateRenderArea();
            }
        }

        public bool ExclusiveFocus { get { return false; } set { } }

        public Font Font
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsFocused { get { return false; } set { } }

        public bool IsVisible
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IParentConsole Parent
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Point Position
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Matrix? Transform
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool UseAbsolutePositioning
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Rectangle ViewArea
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public SadConsole.Consoles.Console.Cursor VirtualCursor { get { return null; } set { } }


        public LockedConsole(CellSurface originalData) : this(originalData, Engine.DefaultFont) { }

        public LockedConsole(CellSurface originalData, Font font)
        {
            // Cycle through the cell data
            // If any cell has information on it, copy it into the list
            //
            Font = font;
            cellSize = new Point(font.CellWidth, font.CellHeight);
            CalculateRenderArea();
        }

        public bool ProcessKeyboard(KeyboardInfo info)
        {
            return false;
        }

        public bool ProcessMouse(MouseInfo info)
        {
            return false;
        }

        public void Render()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
