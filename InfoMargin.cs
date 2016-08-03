﻿
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Editor;

namespace SSMSX
{
    /// <summary>
    /// Margin's canvas and visual definition including both size and content
    /// </summary>
    internal class InfoMargin : Canvas, IWpfTextViewMargin
    {
        /// <summary>
        /// Margin name.
        /// </summary>
        public const string MarginName = "InfoMargin";

        /// <summary>
        /// A value indicating whether the object is disposed.
        /// </summary>
        private bool isDisposed;
        private Label label;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoMargin"/> class for a given <paramref name="textView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> to attach the margin to.</param>
        public InfoMargin(IWpfTextView textView)
        {
            this.Height = 24; // Margin height sufficient to have the label
            this.ClipToBounds = true;

            // Get the current themed environment background colour from VS
            System.Drawing.Color color = VSColorTheme.GetThemedColor(EnvironmentColors.StatusBarDefaultTextColorKey);
            System.Windows.Media.Color newColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
            this.Background = new SolidColorBrush(newColor);

            // Add a label 
            label = new Label
            {
                Background = this.Background,
                Content = "Fresh window, not yet edited.",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            this.Children.Add(label);

            // Hook up a handler for when an edit is performed
            textView.TextBuffer.PostChanged += TextBuffer_PostChanged;
        }

        private void TextBuffer_PostChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => label.Content = $"Last Edit : {DateTime.Now.ToLongTimeString()}");
        }

        #region IWpfTextViewMargin

        /// <summary>
        /// Gets the <see cref="Sytem.Windows.FrameworkElement"/> that implements the visual representation of the margin.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
        public FrameworkElement VisualElement
        {
            // Since this margin implements Canvas, this is the object which renders
            // the margin.
            get
            {
                this.ThrowIfDisposed();
                return this;
            }
        }

        #endregion

        #region ITextViewMargin

        /// <summary>
        /// Gets the size of the margin.
        /// </summary>
        /// <remarks>
        /// For a horizontal margin this is the height of the margin,
        /// since the width will be determined by the <see cref="ITextView"/>.
        /// For a vertical margin this is the width of the margin,
        /// since the height will be determined by the <see cref="ITextView"/>.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
        public double MarginSize
        {
            get
            {
                this.ThrowIfDisposed();

                // Since this is a horizontal margin, its width will be bound to the width of the text view.
                // Therefore, its size is its height.
                return this.ActualHeight;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the margin is enabled.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
        public bool Enabled
        {
            get
            {
                this.ThrowIfDisposed();

                // The margin should always be enabled
                return true;
            }
        }

        /// <summary>
        /// Gets the <see cref="ITextViewMargin"/> with the given <paramref name="marginName"/> or null if no match is found
        /// </summary>
        /// <param name="marginName">The name of the <see cref="ITextViewMargin"/></param>
        /// <returns>The <see cref="ITextViewMargin"/> named <paramref name="marginName"/>, or null if no match is found.</returns>
        /// <remarks>
        /// A margin returns itself if it is passed its own name. If the name does not match and it is a container margin, it
        /// forwards the call to its children. Margin name comparisons are case-insensitive.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="marginName"/> is null.</exception>
        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return string.Equals(marginName, InfoMargin.MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
        }

        /// <summary>
        /// Disposes an instance of <see cref="InfoMargin"/> class.
        /// </summary>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                GC.SuppressFinalize(this);
                this.isDisposed = true;
            }
        }

        #endregion

        /// <summary>
        /// Checks and throws <see cref="ObjectDisposedException"/> if the object is disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(MarginName);
            }
        }
    }
}
