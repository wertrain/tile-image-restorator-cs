using System;
using System.Collections;
using System.Drawing;

namespace ImageComparison
{
    public abstract class ImageComparisonContext
    {
        public abstract IImageComparable FromImage(Image image);
    }

    public interface IImageComparable
    {
        double GetDistanceFrom(IImageComparable imageComparable);
    }
}
