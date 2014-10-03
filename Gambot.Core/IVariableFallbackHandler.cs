using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gambot.Core
{
    public interface IVariableFallbackHandler
    {
        /// <summary>
        /// Called when a variable falls through all other variable handlers.
        /// If this handler cannot find a value to supply for this variable,
        /// it should return <b>null</b>, which will cause the next
        /// registered IVariableFallbackHandler to be triggered.
        /// </summary>
        /// <param name="variable">the name of the variable, sans $</param>
        /// <param name="context">the message the variable appears in</param>
        /// <returns>a value for the variable, or <b>null</b></returns>
        string Fallback(string variable, IMessage context);
    }
}
