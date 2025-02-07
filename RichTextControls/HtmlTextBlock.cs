﻿using RichTextControls.Generators;
using System;
using System.Collections.Generic;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace RichTextControls
{
    public partial class HtmlTextBlock : Control
    {
        private Border _rootElement;

        /// <summary>
        /// Gets or sets the style for the blockquote Border
        /// </summary>
        public Style BlockquoteBorderStyle
        {
            get { return (Style)GetValue(BlockquoteBorderStyleProperty); }
            set { SetValue(BlockquoteBorderStyleProperty, value); }
        }

        /// <summary>
        /// Gets the dependency property for <see cref="BlockquoteBorderStyle"/>.
        /// </summary>
        public static readonly DependencyProperty BlockquoteBorderStyleProperty = DependencyProperty.Register(
            nameof(BlockquoteBorderStyle),
            typeof(Style),
            typeof(HtmlTextBlock),
            new PropertyMetadata(null, OnRenderingPropertyChanged)
        );

        /// <summary>
        /// Gets or sets the style for the preformatted block Border
        /// </summary>
        public Style PreformattedBorderStyle
        {
            get { return (Style)GetValue(PreformattedBorderStyleProperty); }
            set { SetValue(PreformattedBorderStyleProperty, value); }
        }

        /// <summary>
        /// Gets the dependency property for <see cref="PreformattedBorderStyle"/>.
        /// </summary>
        public static readonly DependencyProperty PreformattedBorderStyleProperty = DependencyProperty.Register(
            nameof(PreformattedBorderStyle),
            typeof(Style),
            typeof(HtmlTextBlock),
            new PropertyMetadata(null, OnRenderingPropertyChanged)
        );

        /// <summary>
        /// Gets or sets the html to be rendered.
        /// </summary>
        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        /// <summary>
        /// Gets the dependency property for <see cref="Html"/>.
        /// </summary>
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.Register(
            nameof(Html),
            typeof(string),
            typeof(HtmlTextBlock),
            new PropertyMetadata("", OnRenderingPropertyChanged)
        );

        /// <summary>
        /// Fired when an image element in the markdown was tapped.
        /// </summary>
        public event EventHandler<Windows.UI.Xaml.Input.TappedRoutedEventArgs> ImageTapped;

        /// <summary>
        /// Holds a list of hyperlinks we are listening to.
        /// </summary>
        public static readonly List<object> _listeningHyperlinks = new List<object>();

        /// <summary>
        /// Gets or sets a custom Html to Xaml generator.
        /// </summary>
        public IHtmlXamlGenerator CustomGenerator
        {
            get { return (IHtmlXamlGenerator)GetValue(CustomGeneratorProperty); }
            set { SetValue(CustomGeneratorProperty, value); }
        }

        /// <summary>
        /// Gets the dependency property for <see cref="CustomGenerator"/>.
        /// </summary>
        public static readonly DependencyProperty CustomGeneratorProperty = DependencyProperty.Register(
            nameof(CustomGenerator),
            typeof(Dictionary<string, Func<string, Inline>>),
            typeof(HtmlTextBlock),
            new PropertyMetadata(null, OnRenderingPropertyChanged)
        );

        /// <summary>
        /// The root child containing the parsed and rendered HTML.
        /// </summary>
        public UIElement Child
        {
            get
            {
                return _rootElement;
            }
        }

        /// <summary>
        /// Re-renders the HTML document when the property changes
        /// </summary>
        private static void OnRenderingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as HtmlTextBlock;
            if (control == null)
                return;
            
            control.RenderDocument();
        }

        public HtmlTextBlock()
        {
            DefaultStyleKey = typeof(HtmlTextBlock);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _rootElement = GetTemplateChild("RootElement") as Border;

            RenderDocument();
        }

        private void RenderDocument()
        {
            UnhookListeners();

            _listeningHyperlinks.Clear();

            if (_rootElement == null || String.IsNullOrEmpty(Html))
                return;

            try
            {
                var generator = CustomGenerator ?? new HtmlXamlGenerator(Html);

                generator.BlockquoteBorderStyle = BlockquoteBorderStyle;
                generator.PreformattedBorderStyle = PreformattedBorderStyle;

                var parsedHtml = generator.Generate();

                HookListeners();

                _rootElement.Child = parsedHtml;
            }
            catch (Exception ex)
            {
                _rootElement.Child = new TextBlock() { Text = $"Unable to parse this document. The error was: {ex.Message}" };
            }
        }

        private void UnhookListeners()
        {
            // Unhook any hyper link events if we have any
            foreach (object link in _listeningHyperlinks)
            {
                if (link is Hyperlink hyperlink)
                {
                    hyperlink.Click -= Hyperlink_Click;
                }
                else if (link is Image image)
                {
                    image.Tapped -= NewImagelink_Tapped;
                }
            }
        }
        private void HookListeners()
        {
            // Unhook any hyper link events if we have any
            foreach (object link in _listeningHyperlinks)
            {
                if (link is Hyperlink hyperlink)
                {
                    hyperlink.Click += Hyperlink_Click;
                }
                else if (link is Image image)
                {
                    image.Tapped += NewImagelink_Tapped;
                }
            }
        }
    }
}
