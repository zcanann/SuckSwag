namespace SuckSwag.View
{
    using SuckSwag.Source.BoardFinder;
    using SuckSwag.Source.PieceFinder;
    using SuckSwag.Source.SquareViewer;
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Provides the template required to view a pane.
    /// </summary>
    internal class ViewTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewTemplateSelector" /> class.
        /// </summary>
        public ViewTemplateSelector()
        {
        }

        /// <summary>
        /// Gets or sets the template for the Square Viewer.
        /// </summary>
        public DataTemplate SquareViewerViewTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for the Board Finder.
        /// </summary>
        public DataTemplate BoardFinderViewTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for the Piece Finder.
        /// </summary>
        public DataTemplate PieceFinderViewTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for the Engine.
        /// </summary>
        public DataTemplate EngineViewTemplate { get; set; }

        /// <summary>
        /// Returns the required template to display the given view model.
        /// </summary>
        /// <param name="item">The view model.</param>
        /// <param name="container">The dependency object.</param>
        /// <returns>The template associated with the provided view model.</returns>
        public override DataTemplate SelectTemplate(Object item, DependencyObject container)
        {
            if (item is SquareViewerViewModel)
            {
                return this.SquareViewerViewTemplate;
            }
            else if (item is BoardFinderViewModel)
            {
                return this.BoardFinderViewTemplate;
            }
            else if (item is PieceFinderViewModel)
            {
                return this.PieceFinderViewTemplate;
            }
            else if (item is EngineViewModel)
            {
                return this.EngineViewTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
    //// End class
}
//// End namespace