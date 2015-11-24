using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetyMp.FileHandling
{
    /// <summary>
    /// Custom Double converter for FileHandler
    /// to make sure it handles empty strings right.
    /// 
    /// It does not completely handle them with the complexity
    /// that FileHelpers does, but for our use case in this application
    /// it is basically fine :)
    /// </summary>
    class MySimpleDoubleConverter : ConverterBase
    {
        public override object StringToField(string from)
        {
            double result;

            if (double.TryParse(from, out result))
            {
                return result;
            }
            else
            {
                return (double)0;
            }
            
        }
    }
}
