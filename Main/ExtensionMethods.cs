using System;
using System.Collections.Generic;
using System.Text;

namespace Main
{
   static class ExtensionMethods
   {
      /// <summary>
      /// Returns the entire source string or everything to the left of specified string
      /// </summary>
      /// <param name="sourceString"></param>
      /// <param name="str"></param>
      /// <returns></returns>
      public static string AllToTheLeftOfStringOrFull(this string sourceString, string str)
      {
         var resultString = sourceString;
         int index = sourceString.IndexOf(str, StringComparison.Ordinal);
         if (index != -1)
            resultString = sourceString.Substring(0, index);
         return resultString;
      }
      /// <summary>
      /// Returns everything to the right of the first occurence of specified string or empty string
      /// </summary>
      /// <param name="sourceString"></param>
      /// <param name="str"></param>
      /// <returns></returns>
      public static string AllTotheRightOfString(this string sourceString, string str)
      {
         var resultString = sourceString;
         int index = sourceString.IndexOf(str, StringComparison.Ordinal);
         if (index != -1)
            resultString = sourceString.Substring(index + str.Length);
         return resultString;
      }
   }
}
