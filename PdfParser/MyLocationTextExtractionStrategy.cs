using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.parser.LocationTextExtractionStrategy;

namespace PdfParser
{
    public class MyLocationTextExtractionStrategy : ITextExtractionStrategy
    {    // Fields
        private int _UndercontentCharacterSpacing = 0;
        private int _UndercontentHorizontalScaling = 0;
        private SortedList<string, DocumentFont> ThisPdfDocFonts = new SortedList<string, DocumentFont>();
        public static bool DUMP_STATE = false;
        private List<TextChunk> locationalResult = new List<TextChunk>();

        // Methods
        public virtual void BeginTextBlock()
        {
        }

        private void DumpState()
        {
            IEnumerator<TextChunk> enumerator = null;
            try
            {
                enumerator = this.locationalResult.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    enumerator.Current.PrintDiagnostics();
                    Console.WriteLine();
                }
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        private bool EndsWithSpace(string str)
        {
            if (str.Length == 0)
            {
                return false;
            }
            return (str[str.Length - 1] == ' ');
        }

        public virtual void EndTextBlock()
        {
        }

        private Rectangle GetRectangleFromText(TextChunk firstChunk, TextChunk lastChunk, string pSearchString, string sTextinChunks, int iFromChar, int iToChar, StringComparison pStrComp)
        {
            string str, str2; 
            float num = lastChunk.PosRight - firstChunk.PosLeft;
            float num2 = this.GetStringWidth(sTextinChunks, lastChunk.curFontSize, lastChunk.charSpaceWidth, this.ThisPdfDocFonts.Values.ElementAt<DocumentFont>(lastChunk.FontIndex));
            float num3 = num / num2;
            int index = sTextinChunks.IndexOf(pSearchString, pStrComp);
            int num5 = (index + pSearchString.Length) - 1;
            if (index == 0)
            {
                str = null;
            }
            else
            {
                str = sTextinChunks.Substring(0, index);
            }
            if (num5 == (sTextinChunks.Length - 1))
            {
                str2 = null;
            }
            else
            {
                str2 = sTextinChunks.Substring(num5 + 1, (sTextinChunks.Length - num5) - 1);
            }
            float num6 = 0f;
            if (index > 0)
            {
                num6 = this.GetStringWidth(str, lastChunk.curFontSize, lastChunk.charSpaceWidth, this.ThisPdfDocFonts.Values.ElementAt<DocumentFont>(lastChunk.FontIndex)) * num3;
            }
            float num7 = 0f;
            if (num5 < (sTextinChunks.Length - 1))
            {
                num7 = this.GetStringWidth(str2, lastChunk.curFontSize, lastChunk.charSpaceWidth, this.ThisPdfDocFonts.Values.ElementAt<DocumentFont>(lastChunk.FontIndex)) * num3;
            }
            float llx = firstChunk.distParallelStart + num6;
            return new Rectangle(llx, firstChunk.PosBottom, lastChunk.distParallelEnd - num7, firstChunk.PosTop);
        }

        public virtual string GetResultantText()
        {
            if (DUMP_STATE)
            {
                this.DumpState();
            }
            //this.locationalResult.Sort();
            StringBuilder builder = new StringBuilder();
            TextChunk a = null;
            foreach (TextChunk chunk2 in this.locationalResult)
            {
                if (a == null)
                {
                    builder.Append(chunk2.text);
                }
                else if (chunk2.SameLine(a))
                {
                    float num = chunk2.DistanceFromEndOf(a);
                    if (num < -chunk2.charSpaceWidth)
                    {
                        builder.Append(' ');
                    }
                    else if (((num > (chunk2.charSpaceWidth / 2f)) && !this.StartsWithSpace(chunk2.text)) && !this.EndsWithSpace(a.text))
                    {
                        builder.Append(' ');
                    }
                    builder.Append(chunk2.text);
                }
                else
                {
                    builder.Append('\n');
                    builder.Append(chunk2.text);
                }
                a = chunk2;
            }
            return builder.ToString();
        }

        private float GetStringWidth(string str, float curFontSize, float pSingleSpaceWidth, DocumentFont pFont)
        {
            char[] chArray = str.ToCharArray();
            float left = 0f;
            float num3 = 0f;
            foreach (char ch in chArray)
            {
                num3 = (float)(((double)pFont.GetWidth(Convert.ToString(ch))) / 1000.0);
                left += Convert.ToSingle(num3 * curFontSize);
            }
            return left;
        }

        public List<Rectangle> GetTextLocations(string pSearchString, StringComparison pStrComp)
        {
            List<Rectangle> list2 = new List<Rectangle>();
            StringBuilder builder = new StringBuilder();
            List<TextChunk> source = new List<TextChunk>();
            TextChunk firstChunk = null;
            TextChunk lastChunk = null;
            string sTextinChunks = null;
            foreach (TextChunk chunk3 in this.locationalResult)
            {
                if ((source.Count > 0) && !chunk3.SameLine(source.Last<TextChunk>()))
                {
                    if (builder.ToString().IndexOf(pSearchString, pStrComp) > -1)
                    {
                        string str2 = builder.ToString();
                        int num = 0;
                        for (int i = str2.IndexOf(pSearchString, 0, pStrComp); i > -1; i = str2.IndexOf(pSearchString, i, pStrComp))
                        {
                            num++;
                            if ((i + pSearchString.Length) > str2.Length)
                            {
                                break;
                            }
                            i += pSearchString.Length;
                        }
                        int startIndex = 0;
                        int num4 = num;
                        for (int j = 1; j <= num4; j++)
                        {
                            int iFromChar = str2.IndexOf(pSearchString, startIndex, pStrComp);
                            startIndex = iFromChar;
                            int iToChar = (iFromChar + pSearchString.Length) - 1;
                            string str3 = null;
                            sTextinChunks = null;
                            firstChunk = null;
                            lastChunk = null;
                            bool flag = false;
                            bool flag2 = false;
                            foreach (TextChunk chunk4 in source)
                            {
                                str3 = str3 + chunk4.text;
                                if (!flag && ((str3.Length - 1) >= iFromChar))
                                {
                                    firstChunk = chunk4;
                                    flag = true;
                                }
                                if (flag & !flag2)
                                {
                                    sTextinChunks = sTextinChunks + chunk4.text;
                                }
                                if (!flag2 && ((str3.Length - 1) >= iToChar))
                                {
                                    lastChunk = chunk4;
                                    flag2 = true;
                                }
                                if (flag & flag2)
                                {
                                    list2.Add(this.GetRectangleFromText(firstChunk, lastChunk, pSearchString, sTextinChunks, iFromChar, iToChar, pStrComp));
                                    startIndex += pSearchString.Length;
                                    flag = false;
                                    flag2 = false;
                                    break;
                                }
                            }
                        }
                    }
                    builder.Clear();
                    source.Clear();
                }
                source.Add(chunk3);
                builder.Append(chunk3.text);
            }
            return list2;
        }

        public void RenderImage(ImageRenderInfo renderInfo)
        {
        }

        public virtual void RenderText(TextRenderInfo renderInfo)
        {
            LineSegment baseline = renderInfo.GetBaseline();
            TextChunk item = new TextChunk(renderInfo.GetText(), baseline.GetStartPoint(), baseline.GetEndPoint(), renderInfo.GetSingleSpaceWidth());
            TextChunk chunk2 = item;
            Debug.Print(renderInfo.GetText());
            chunk2.PosLeft = renderInfo.GetDescentLine().GetStartPoint()[0];
            chunk2.PosRight = renderInfo.GetAscentLine().GetEndPoint()[0];
            chunk2.PosBottom = renderInfo.GetDescentLine().GetStartPoint()[1];
            chunk2.PosTop = renderInfo.GetAscentLine().GetEndPoint()[1];
            chunk2.curFontSize = chunk2.PosTop - baseline.GetStartPoint()[1];
            string key = renderInfo.GetFont().PostscriptFontName + chunk2.curFontSize.ToString();
            if (!this.ThisPdfDocFonts.ContainsKey(key))
            {
                this.ThisPdfDocFonts.Add(key, renderInfo.GetFont());
            }
            chunk2.FontIndex = this.ThisPdfDocFonts.IndexOfKey(key);
            chunk2 = null;
            this.locationalResult.Add(item);
        }

        private bool StartsWithSpace(string str)
        {
            if (str.Length == 0)
            {
                return false;
            }
            return (str[0] == ' ');
        }

        // Properties
        public object UndercontentCharacterSpacing
        {
            get { return this._UndercontentCharacterSpacing; }
            set
            {
                this._UndercontentCharacterSpacing = (int)RuntimeHelpers.GetObjectValue(value);
            }
        }

        public object UndercontentHorizontalScaling
        {
            get { return this._UndercontentHorizontalScaling; }
            set { this._UndercontentHorizontalScaling = (int)RuntimeHelpers.GetObjectValue(value); }
        }

        // Nested Types
        public class TextChunk : IComparable<MyLocationTextExtractionStrategy.TextChunk>
        {
            // Fields
            internal string text;
            internal Vector startLocation;
            internal Vector endLocation;
            internal Vector orientationVector;
            internal int orientationMagnitude;
            internal int distPerpendicular;
            internal float distParallelStart;
            internal float distParallelEnd;
            internal float charSpaceWidth;
            private float _PosLeft;
            private float _PosRight;
            private float _PosTop;
            private float _PosBottom;
            private float _curFontSize;
            private int _FontIndex;

            // Methods
            public TextChunk(string str, Vector startLocation, Vector endLocation, float charSpaceWidth)
            {
                this.text = str;
                this.startLocation = startLocation;
                this.endLocation = endLocation;
                this.charSpaceWidth = charSpaceWidth;
                Vector vector = endLocation.Subtract(startLocation);
                if (vector.Length == 0f)
                {
                    vector = new Vector(1f, 0f, 0f);
                }
                this.orientationVector = vector.Normalize();
                this.orientationMagnitude = (int)Math.Round(Math.Truncate((double)(Math.Atan2((double)this.orientationVector[1], (double)this.orientationVector[0]) * 1000.0)));
                Vector v = new Vector(0f, 0f, 1f);
                this.distPerpendicular = (int)Math.Round((double)startLocation.Subtract(v).Cross(this.orientationVector)[2]);
                this.distParallelStart = this.orientationVector.Dot(startLocation);
                this.distParallelEnd = this.orientationVector.Dot(endLocation);
            }

            private static int CompareInts(int int1, int int2) =>
                ((int1 == int2) ? 0 : ((int1 < int2) ? -1 : 1));

            public int CompareTo(MyLocationTextExtractionStrategy.TextChunk rhs)
            {
                if (this == rhs)
                {
                    return 0;
                }
                int num2 = CompareInts(this.orientationMagnitude, rhs.orientationMagnitude);
                if (num2 > 0)
                {
                    return num2;
                }
                num2 = CompareInts(this.distPerpendicular, rhs.distPerpendicular);
                if (num2 > 0)
                {
                    return num2;
                }
                return ((this.distParallelStart < rhs.distParallelStart) ? -1 : 1);
            }

            public float DistanceFromEndOf(MyLocationTextExtractionStrategy.TextChunk other) =>
                (this.distParallelStart - other.distParallelEnd);

            public void PrintDiagnostics()
            {
                Console.WriteLine("Text (@" + Convert.ToString(this.startLocation) + " -> " + Convert.ToString(this.endLocation) + "): " + this.text);
                Console.WriteLine("orientationMagnitude: " + Convert.ToString(this.orientationMagnitude));
                Console.WriteLine("distPerpendicular: " + Convert.ToString(this.distPerpendicular));
                Console.WriteLine("distParallel: " + Convert.ToString(this.distParallelStart));
            }

            public bool SameLine(MyLocationTextExtractionStrategy.TextChunk a)
            {
                if (this.orientationMagnitude != a.orientationMagnitude)
                {
                    return false;
                }
                if (this.distPerpendicular != a.distPerpendicular)
                {
                    return false;
                }
                return true;
            }

            // Properties
            public int FontIndex
            {
                get
                {
                    return
              this._FontIndex;
                }
                set { this._FontIndex = value; }
            }

            public float PosLeft
            {
                get
                {
                    return
              this._PosLeft;
                }
                set { this._PosLeft = value; }
            }

            public float PosRight
            {
                get { return this._PosRight; }
                set { this._PosRight = value; }
            }

            public float PosTop
            {
                get
                {
                    return
              this._PosTop;
                }
                set { this._PosTop = value; }
            }

            public float PosBottom
            {
                get
                {
                    return
              this._PosBottom;
                }
                set { this._PosBottom = value; }
            }

            public float curFontSize
            {
                get { return this._curFontSize; }
                set { this._curFontSize = value; }
            }
        }
    }
}
