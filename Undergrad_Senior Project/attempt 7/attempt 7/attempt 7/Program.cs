//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Anthony">
//     Program - the main entry point for the application
// </copyright>
//-----------------------------------------------------------------------

namespace Attempt_7
{
    using System;

#if WINDOWS || XBOX

    /// <summary>
    /// The Program Class
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Strings passed to the application at startup from Command Prompt</param>
        static void Main(string[] args)
        {
            using (SimulationMain game = new SimulationMain())
            {
                game.Run();
            }
        }
    }
#endif
}

