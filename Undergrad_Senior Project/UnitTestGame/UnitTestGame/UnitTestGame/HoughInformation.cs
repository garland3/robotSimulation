using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Attempt_7
{
    
    public class HoughInformation
    {
        private int NumberOfLines;

        private Array LineList;
       
        public HoughInformation(int numberOfLines)
        {
            this.NumberOfLines = numberOfLines;
            this.LineList = new Line[NumberOfLines];
        }
    }
}
