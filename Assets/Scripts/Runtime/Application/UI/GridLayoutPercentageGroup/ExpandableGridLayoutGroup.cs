using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ExpandableGridLayoutGroup : LayoutGroup
{
    public enum FitType { Uniform, FixedColumns, FixedRows }
    public FitType fitType = FitType.Uniform;
    public int constraintCount = 2;

    public enum HorizontalDirection { LeftToRight, RightToLeft }
    public HorizontalDirection horizontalDirection = HorizontalDirection.LeftToRight;

    public enum VerticalDirection { TopToBottom, BottomToTop }
    public VerticalDirection verticalDirection = VerticalDirection.TopToBottom;

    public enum AlignmentMode { Start, Center, End }
    [SerializeField] private AlignmentMode incompleteAlignment = AlignmentMode.Center;

    public enum ExpandMode { None, ExpandWidth, ExpandHeight }
    [SerializeField] private ExpandMode expandMode = ExpandMode.None;

    [Range(0f, 1f)] public float spacingPercentX = 0.05f;
    [Range(0f, 1f)] public float spacingPercentY = 0.05f;
    [Range(0f, 0.5f)] public float leftPaddingPercent = 0.05f;
    [Range(0f, 0.5f)] public float rightPaddingPercent = 0.05f;
    [Range(0f, 0.5f)] public float topPaddingPercent = 0.05f;
    [Range(0f, 0.5f)] public float bottomPaddingPercent = 0.05f;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        Arrange();
    }

    public override void CalculateLayoutInputVertical()
    {
        Arrange();
    }

    public override void SetLayoutHorizontal() { }
    public override void SetLayoutVertical() { }

    void Arrange()
    {
        int childCount = rectChildren.Count;
        if (childCount == 0) return;

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        float spacingX = width * spacingPercentX;
        float spacingY = height * spacingPercentY;

        float leftPadding = width * leftPaddingPercent;
        float rightPadding = width * rightPaddingPercent;
        float topPadding = height * topPaddingPercent;
        float bottomPadding = height * bottomPaddingPercent;

        int cols = 1, rows = 1;

        if (fitType == FitType.Uniform)
        {
            float sqrRt = Mathf.Sqrt(childCount);
            cols = Mathf.CeilToInt(sqrRt);
            rows = Mathf.CeilToInt(sqrRt);
        }
        else if (fitType == FitType.FixedColumns || expandMode == ExpandMode.ExpandHeight)
        {
            cols = constraintCount;
            rows = Mathf.CeilToInt((float)childCount / cols);
        }
        else if (fitType == FitType.FixedRows || expandMode == ExpandMode.ExpandWidth)
        {
            rows = constraintCount;
            cols = Mathf.CeilToInt((float)childCount / rows);
        }

        float contentWidth = width - leftPadding - rightPadding - spacingX * (cols - 1);
        float contentHeight = height - topPadding - bottomPadding - spacingY * (rows - 1);

        float itemWidth = contentWidth / cols;
        float itemHeight = contentHeight / rows;

        float maxItemHeight = 0f;

        for (int i = 0; i < childCount; i++)
        {
            var child = rectChildren[i];
            float childItemHeight = itemHeight;

            var aspectFitter = child.GetComponent<AspectRatioFitter>();
            if (aspectFitter != null && aspectFitter.aspectMode == AspectRatioFitter.AspectMode.WidthControlsHeight)
            {
                childItemHeight = itemWidth / aspectFitter.aspectRatio;
            }

            if (childItemHeight > maxItemHeight)
                maxItemHeight = childItemHeight;
        }

        if (maxItemHeight > 0f)
            itemHeight = maxItemHeight;

        float totalItemWidth = itemWidth * cols + spacingX * (cols - 1);
        float totalItemHeight = itemHeight * rows + spacingY * (rows - 1);

        float targetWidth = totalItemWidth + leftPadding + rightPadding;
        float targetHeight = totalItemHeight + topPadding + bottomPadding;

        if (expandMode == ExpandMode.ExpandHeight)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
        }
        else if (expandMode == ExpandMode.ExpandWidth)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
        }

        for (int i = 0; i < childCount; i++)
        {
            int row, col;
            if (fitType == FitType.FixedColumns || expandMode == ExpandMode.ExpandHeight)
            {
                row = i / cols;
                col = i % cols;
            }
            else
            {
                col = i / rows;
                row = i % rows;
            }

            int itemsInLine = fitType == FitType.FixedColumns || expandMode == ExpandMode.ExpandHeight
                ? (row == rows - 1 ? Mathf.Min(childCount - row * cols, cols) : cols)
                : (col == cols - 1 ? Mathf.Min(childCount - col * rows, rows) : rows);

            float lineOffset = GetAlignmentOffset(
                fitType == FitType.FixedColumns || expandMode == ExpandMode.ExpandHeight ? cols : rows,
                itemsInLine,
                (fitType == FitType.FixedColumns || expandMode == ExpandMode.ExpandHeight ? itemWidth + spacingX : itemHeight + spacingY),
                incompleteAlignment);

            if (horizontalDirection == HorizontalDirection.RightToLeft)
                col = cols - 1 - col;

            if (verticalDirection == VerticalDirection.TopToBottom)
                row = rows - 1 - row;

            float xPos = leftPadding + col * (itemWidth + spacingX);
            float yPos;

            height = rectTransform.rect.height;
            topPadding = height * topPaddingPercent;
            bottomPadding = height * bottomPaddingPercent;

            if (verticalDirection == VerticalDirection.BottomToTop)
            {
                yPos = -topPadding - row * (itemHeight + spacingY);
            }
            else
            {
                yPos = -topPadding + (rows - 1 - row) * (itemHeight + spacingY);
            }


            if (fitType == FitType.FixedColumns || expandMode == ExpandMode.ExpandHeight)
                xPos += lineOffset;
            else
                yPos -= lineOffset;

            bool layoutControlsWidth = fitType == FitType.FixedColumns || fitType == FitType.Uniform || expandMode == ExpandMode.ExpandHeight;
            bool layoutControlsHeight = fitType == FitType.FixedRows || fitType == FitType.Uniform || expandMode == ExpandMode.ExpandWidth;

            if (layoutControlsWidth)
                SetChildAlongAxis(rectChildren[i], 0, xPos, itemWidth);
            else
                SetChildAlongAxis(rectChildren[i], 0, xPos);

            if (layoutControlsHeight)
                SetChildAlongAxis(rectChildren[i], 1, yPos, itemHeight);
            else
                SetChildAlongAxis(rectChildren[i], 1, yPos);
        }
    }

    float GetAlignmentOffset(int fullCount, int actualCount, float cellWithSpacing, AlignmentMode mode)
    {
        int missing = fullCount - actualCount;
        switch (mode)
        {
            case AlignmentMode.Center: return missing * cellWithSpacing / 2f;
            case AlignmentMode.End: return missing * cellWithSpacing;
            default: return 0f;
        }
    }
}
