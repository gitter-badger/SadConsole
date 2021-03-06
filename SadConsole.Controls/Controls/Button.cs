﻿namespace SadConsole.Controls
{
    using Microsoft.Xna.Framework;
    using SadConsole.Themes;
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class Button: ControlBase
    {
        /// <summary>
        /// Raised when the button is clicked.
        /// </summary>
        public event EventHandler ButtonClicked;

        [DataMember(Name="Theme")]
        protected ButtonTheme _theme;
        [DataMember(Name = "Text")]
        protected string _text;
        protected bool _isMouseDown;
        [DataMember(Name = "TextAlignment")]
        protected System.Windows.HorizontalAlignment _textAlignment = System.Windows.HorizontalAlignment.Center;
        protected CellAppearance _currentAppearance;

        //public int Margin = 0;

        /// <summary>
        /// The theme of this control. If the theme is not explicitly set, the theme is taken from the library.
        /// </summary>
        public virtual ButtonTheme Theme
        {
            get
            {
                if (_theme == null)
                    return Library.Default.ButtonTheme;
                else
                    return _theme;
            }
            set
            {
                _theme = value;
            }
        }

        /// <summary>
        /// The text displayed on the control.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; Compose(true); }
        }

        /// <summary>
        /// The alignment of the text, left, center, or right.
        /// </summary>
        public System.Windows.HorizontalAlignment TextAlignment
        {
            get { return _textAlignment; }
            set { _textAlignment = value; Compose(true); }
        }

        /// <summary>
        /// Creates an instance of the button control with the specified width and height.
        /// </summary>
        /// <param name="width">Width of the control.</param>
        /// <param name="height">Height of the control.</param>
        public Button(int width, int height)
            : base()
        {
            base.Resize(width, height);

            DetermineAppearance();
        }

        /// <summary>
        /// Raises the <see cref="ButtonClicked"/> event.
        /// </summary>
        public virtual void Click()
        {
            if (ButtonClicked != null)
                ButtonClicked(this, new EventArgs());
        }

        /// <summary>
        /// Sets the appearance of the control depending on the current state of the control.
        /// </summary>
        public override void DetermineAppearance()
        {
            CellAppearance currentappearance = _currentAppearance;

            if (!_isEnabled)
                _currentAppearance = Theme.Disabled;

            else if (!_isMouseDown && _isMouseOver)
                _currentAppearance = Theme.MouseOver;

            else if (!_isMouseDown && !_isMouseOver && IsFocused && Engine.ActiveConsole == _parent)
                _currentAppearance = Theme.Focused;

            else if (_isMouseDown && _isMouseOver)
                _currentAppearance = Theme.MouseClicking;

            else
                _currentAppearance = Theme.Normal;

            if (currentappearance != _currentAppearance)
                IsDirty = true;
        }

        /// <summary>
        /// Detects if the SPACE and ENTER keys are pressed and calls the <see cref="Click"/> method.
        /// </summary>
        /// <param name="info"></param>
        public override bool ProcessKeyboard(Input.KeyboardInfo info)
        {
            if (info.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Space) || info.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                Click();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called when the mouse is in the control area.
        /// </summary>
        /// <param name="info">The mouse state.</param>
        protected override void OnMouseIn(Input.MouseInfo info)
        {
            _isMouseDown = info.LeftButtonDown;

            base.OnMouseIn(info);
        }

        /// <summary>
        /// Called when the mouse leaves the control area.
        /// </summary>
        /// <param name="info">The mouse state.</param>
        protected override void OnMouseExit(Input.MouseInfo info)
        {
            _isMouseDown = false;

            base.OnMouseExit(info);
        }

        /// <summary>
        /// Called when the left-mouse button is clicked.
        /// </summary>
        /// <param name="info">The mouse state.</param>
        protected override void OnLeftMouseClicked(Input.MouseInfo info)
        {
            base.OnLeftMouseClicked(info);

            if (_isEnabled)
                Click();
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        public override void Compose()
        {
            if (this.IsDirty)
            {
                // Redraw the control
                this.Fill(_currentAppearance.Foreground, _currentAppearance.Background, _currentAppearance.CharacterIndex, null);
                this.Print(0, 0, Text.Align(TextAlignment, this.Width));

                this.IsDirty = false;
            }
        }

        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            DetermineAppearance();
            Compose(true);
        }
    }
}
