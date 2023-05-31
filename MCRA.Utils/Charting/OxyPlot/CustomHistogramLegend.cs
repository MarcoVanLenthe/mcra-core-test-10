using MCRA.Utils.ExtensionMethods;
using OxyPlot;
using OxyPlot.Legends;
using OxyPlot.Series;

namespace MCRA.Utils.Charting.OxyPlot {
    public class CustomHistogramLegend : LegendBase {

        private OxyRect legendBox;
        /// <summary>
        /// Initializes a new insance of the Legend class.
        /// </summary>
        public CustomHistogramLegend() {
            IsLegendVisible = true;
            legendBox = new();
            Key = null;
            GroupNameFont = null;
            GroupNameFontWeight = FontWeights.Normal;
            GroupNameFontSize = double.NaN;

            LegendTitleFont = null;
            LegendTitleFontSize = double.NaN;
            LegendTitleFontWeight = FontWeights.Bold;
            LegendFontSize = 10;
            LegendFontWeight = FontWeights.Normal;
            LegendSymbolLength = 23;
            LegendSymbolWidth = 9;
            LegendSymbolMargin = 2;
            LegendPadding = 8;
            LegendColumnSpacing = 8;
            LegendItemSpacing = 24;
            LegendLineSpacing = 0;
            LegendMargin = 5;

            LegendBackground = OxyColors.White;
            LegendBorder = OxyColors.White;
            LegendBorderThickness = 1;

            LegendTextColor = OxyColors.Black;
            LegendTitleColor = OxyColors.Automatic;

            LegendMaxWidth = double.NaN;
            LegendMaxHeight = double.NaN;
            LegendPlacement = LegendPlacement.Outside;
            LegendPosition = LegendPosition.RightTop;
            LegendOrientation = LegendOrientation.Vertical;
            LegendItemOrder = LegendItemOrder.Normal;
            LegendItemAlignment = HorizontalAlignment.Left;
            LegendSymbolPlacement = LegendSymbolPlacement.Left;

            ShowInvisibleSeries = true;

            SeriesInvisibleTextColor = OxyColor.FromAColor(64, LegendTextColor);

            SeriesPosMap = new();

            Selectable = true;
            SelectionMode = SelectionMode.Single;
        }

        public double LegendSymbolWidth { get; set; }

        /// <summary>
        /// Override for legend hit test.
        /// </summary>
        /// <param name="args">Arguments passe to the hit test</param>
        /// <returns>The hit test results.</returns>
        protected override HitTestResult LegendHitTest(HitTestArguments args) {
            ScreenPoint point = args.Point;
            if (IsPointInLegend(point)) {
                if (SeriesPosMap != null && SeriesPosMap.Count > 0) {
                    foreach (KeyValuePair<Series, OxyRect> kvp in SeriesPosMap) {
                        if (kvp.Value.Contains(point)) {
                            if (ShowInvisibleSeries) {
                                kvp.Key.IsVisible = !kvp.Key.IsVisible;
                                PlotModel.InvalidatePlot(false);
                                break;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets or sets the group name font.
        /// </summary>
        public string GroupNameFont {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the group name font size.
        /// </summary>
        public double GroupNameFontSize {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the group name font weight.
        /// </summary>
        public double GroupNameFontWeight {
            get;
            set;
        }

        private Dictionary<Series, OxyRect> SeriesPosMap { get; set; }

        /// <summary>
        /// Gets or sets the textcolor of invisible series.
        /// </summary>
        public OxyColor SeriesInvisibleTextColor { get; set; }

        /// <summary>
        /// Checks if a screen point is within the legend boundaries.
        /// </summary>
        /// <param name="point">A screen point.</param>
        /// <returns>A value indicating whether the point is inside legend boundaries or not.</returns>
        public bool IsPointInLegend(ScreenPoint point) {
            return legendBox.Contains(point);
        }

        /// <summary>
        /// Makes the LegendOrientation property safe.
        /// </summary>
        /// <remarks>If Legend is positioned left or right, force it to vertical orientation</remarks>
        public override void EnsureLegendProperties() {
            switch (LegendPosition) {
                case LegendPosition.LeftTop:
                case LegendPosition.LeftMiddle:
                case LegendPosition.LeftBottom:
                case LegendPosition.RightTop:
                case LegendPosition.RightMiddle:
                case LegendPosition.RightBottom:
                    if (LegendOrientation == LegendOrientation.Horizontal) {
                        LegendOrientation = LegendOrientation.Vertical;
                    }

                    break;
            }
        }

        /// <summary>
        /// Renders or measures the legends.
        /// </summary>
        /// <param name="rc">The render context.</param>
        public override void RenderLegends(IRenderContext rc) {
            RenderOrMeasureLegends(rc, LegendArea);
        }

        /// <summary>
        /// Measures the legend area and gets the legend size.
        /// </summary>
        /// <param name="rc">The rendering context.</param>
        /// <param name="availableLegendArea">The area available to legend.</param>
        public override OxySize GetLegendSize(IRenderContext rc, OxySize availableLegendArea) {
            var availableLegendWidth = availableLegendArea.Width;
            var availableLegendHeight = availableLegendArea.Height;

            // Calculate the size of the legend box
            var legendSize = MeasureLegends(rc, new OxySize(Math.Max(0, availableLegendWidth), Math.Max(0, availableLegendHeight)));

            // Ensure legend size is valid
            legendSize = new OxySize(Math.Max(0, legendSize.Width), Math.Max(0, legendSize.Height));

            return legendSize;
        }

        /// <summary>
        /// Gets the rectangle of the legend box.
        /// </summary>
        /// <param name="legendSize">Size of the legend box.</param>
        /// <returns>A rectangle.</returns>
        public override OxyRect GetLegendRectangle(OxySize legendSize) {
            double top = 0;
            double left = 0;
            if (LegendPlacement == LegendPlacement.Outside) {
                switch (LegendPosition) {
                    case LegendPosition.LeftTop:
                    case LegendPosition.LeftMiddle:
                    case LegendPosition.LeftBottom:
                        left = PlotModel.PlotAndAxisArea.Left - legendSize.Width - LegendMargin;
                        break;
                    case LegendPosition.RightTop:
                    case LegendPosition.RightMiddle:
                    case LegendPosition.RightBottom:
                        left = PlotModel.PlotAndAxisArea.Right + LegendMargin;
                        break;
                    case LegendPosition.TopLeft:
                    case LegendPosition.TopCenter:
                    case LegendPosition.TopRight:
                        top = PlotModel.PlotAndAxisArea.Top - legendSize.Height - LegendMargin;
                        break;
                    case LegendPosition.BottomLeft:
                    case LegendPosition.BottomCenter:
                    case LegendPosition.BottomRight:
                        top = PlotModel.PlotAndAxisArea.Bottom + LegendMargin;
                        break;
                }

                var bounds = AllowUseFullExtent
                    ? PlotModel.PlotAndAxisArea
                    : PlotModel.PlotArea;

                switch (LegendPosition) {
                    case LegendPosition.TopLeft:
                    case LegendPosition.BottomLeft:
                        left = bounds.Left;
                        break;
                    case LegendPosition.TopRight:
                    case LegendPosition.BottomRight:
                        left = bounds.Right - legendSize.Width;
                        break;
                    case LegendPosition.LeftTop:
                    case LegendPosition.RightTop:
                        top = bounds.Top;
                        break;
                    case LegendPosition.LeftBottom:
                    case LegendPosition.RightBottom:
                        top = bounds.Bottom - legendSize.Height;
                        break;
                    case LegendPosition.LeftMiddle:
                    case LegendPosition.RightMiddle:
                        top = (bounds.Top + bounds.Bottom - legendSize.Height) * 0.5;
                        break;
                    case LegendPosition.TopCenter:
                    case LegendPosition.BottomCenter:
                        left = (bounds.Left + bounds.Right - legendSize.Width) * 0.5;
                        break;
                }
            } else {
                switch (LegendPosition) {
                    case LegendPosition.LeftTop:
                    case LegendPosition.LeftMiddle:
                    case LegendPosition.LeftBottom:
                        left = PlotModel.PlotArea.Left + LegendMargin;
                        break;
                    case LegendPosition.RightTop:
                    case LegendPosition.RightMiddle:
                    case LegendPosition.RightBottom: {
                            left = PlotModel.PlotArea.Right - legendSize.Width - LegendMargin;
                        }
                        break;
                    case LegendPosition.TopLeft:
                    case LegendPosition.TopCenter:
                    case LegendPosition.TopRight:
                        top = PlotModel.PlotArea.Top + LegendMargin;
                        break;
                    case LegendPosition.BottomLeft:
                    case LegendPosition.BottomCenter:
                    case LegendPosition.BottomRight:
                        top = PlotModel.PlotArea.Bottom - legendSize.Height - LegendMargin;
                        break;
                }

                switch (LegendPosition) {
                    case LegendPosition.TopLeft:
                    case LegendPosition.BottomLeft:
                        left = PlotModel.PlotArea.Left + LegendMargin;
                        break;
                    case LegendPosition.TopRight:
                    case LegendPosition.BottomRight:
                        left = PlotModel.PlotArea.Right - legendSize.Width - LegendMargin;
                        break;
                    case LegendPosition.LeftTop:
                    case LegendPosition.RightTop:
                        top = PlotModel.PlotArea.Top + LegendMargin;
                        break;
                    case LegendPosition.LeftBottom:
                    case LegendPosition.RightBottom:
                        top = PlotModel.PlotArea.Bottom - legendSize.Height - LegendMargin;
                        break;

                    case LegendPosition.LeftMiddle:
                    case LegendPosition.RightMiddle:
                        top = (PlotModel.PlotArea.Top + PlotModel.PlotArea.Bottom - legendSize.Height) * 0.5;
                        break;
                    case LegendPosition.TopCenter:
                    case LegendPosition.BottomCenter:
                        left = (PlotModel.PlotArea.Left + PlotModel.PlotArea.Right - legendSize.Width) * 0.5;
                        break;
                }
            }

            return new OxyRect(left, top, legendSize.Width, legendSize.Height);
        }

        /// <summary>
        /// Measures the legends.
        /// </summary>
        /// <param name="rc">The render context.</param>
        /// <param name="availableSize">The available size for the legend box.</param>
        /// <returns>The size of the legend box.</returns>
        private OxySize MeasureLegends(IRenderContext rc, OxySize availableSize) {
            return RenderOrMeasureLegends(rc, new OxyRect(0, 0, availableSize.Width, availableSize.Height), true);
        }

        /// <summary>
        /// Renders or measures the legends.
        /// </summary>
        /// <param name="rc">The render context.</param>
        /// <param name="rect">Provides the available size if measuring, otherwise it provides the position and size of the legend.</param>
        /// <param name="measureOnly">Specify if the size of the legend box should be measured only (not rendered).</param>
        /// <returns>The size of the legend box.</returns>
        private OxySize RenderOrMeasureLegends(IRenderContext rc, OxyRect rect, bool measureOnly = false) {
            var legendLabels = GetStackedHistogramSeries().LegendaLabels;
            var pallete = GetStackedHistogramSeries().Palette;

            if (!measureOnly) {
                rc.DrawRectangle(rect, LegendBackground, LegendBorder, 2.0, EdgeRenderingMode.Automatic);
            }

            var actualLegendFontSize = double.IsNaN(LegendFontSize) ? PlotModel.DefaultFontSize : LegendFontSize;
            var actualLegendFont = LegendFont ?? PlotModel.DefaultFont;

            var counter = 0;
            double availableWidth = rect.Width;
            double availableHeight = rect.Height;
            double maxItemWidth = 0;
            double totalHeight = 0;
            foreach (var label in legendLabels) {
                var textSize = rc.MeasureMathText(label, LegendFont ?? PlotModel.DefaultFont, actualLegendFontSize, LegendFontWeight);
                var width = LegendSymbolLength + LegendSymbolMargin + textSize.Width;
                var height = LegendLineSpacing + textSize.Height;

                maxItemWidth = Math.Max(maxItemWidth, width);
                totalHeight += height;

                if (!measureOnly) {
                    // Draw box
                    var xBox = rect.Left + LegendMargin;
                    var yBox = rect.Top + LegendMargin + (totalHeight - height);
                    rc.DrawRectangle(new OxyRect(xBox, yBox, LegendSymbolLength, LegendSymbolWidth), pallete.Colors[counter++], OxyColors.Undefined, 1, EdgeRenderingMode.Automatic);

                    // Draw text
                    var xTxt = rect.Left + LegendMargin + LegendSymbolLength + LegendSymbolMargin;
                    var yTxt = yBox - 0.1 * textSize.Height;
                    var maxTxtWidth = Math.Max(textSize.Width, availableWidth - LegendSymbolLength + LegendSymbolMargin - 2 * LegendMargin);
                    var charsPerWidth = label.Length / textSize.Width;
                    var maxChars = (int)(maxTxtWidth * charsPerWidth);
                    var nrOfChars = Math.Max(label.Length, maxChars);
                    var labelLimited = label.LimitTo(nrOfChars);
                    rc.DrawText(new ScreenPoint(xTxt, yTxt), labelLimited, LegendTextColor,
                                actualLegendFont, actualLegendFontSize, LegendFontWeight, 0, HorizontalAlignment.Left, VerticalAlignment.Top);
                }
            }

            maxItemWidth += 2 * LegendMargin;
            totalHeight += 2 * LegendMargin;

            var size = new OxySize(maxItemWidth, totalHeight);
            if (size.Width > 0) {
                size = new OxySize(size.Width + LegendPadding, size.Height);
            }

            if (size.Height > 0) {
                size = new OxySize(size.Width, size.Height + LegendPadding);
            }

            if (size.Width > availableWidth) {
                size = new OxySize(availableWidth, size.Height);
            }

            if (size.Height > availableHeight) {
                size = new OxySize(size.Width, availableHeight);
            }

            if (!double.IsNaN(LegendMaxWidth) && size.Width > LegendMaxWidth) {
                size = new OxySize(LegendMaxWidth, size.Height);
            }

            if (!double.IsNaN(LegendMaxHeight) && size.Height > LegendMaxHeight) {
                size = new OxySize(size.Width, LegendMaxHeight);
            }

            return size;
        }
        private StackedHistogramSeries<string> GetStackedHistogramSeries() {
            if (PlotModel.Series.Count != 1) {
                throw new InvalidOperationException();
            }
            var stackedHistogramSeries = PlotModel.Series.First() as StackedHistogramSeries<string>;
            return stackedHistogramSeries;
        }
    }
}
