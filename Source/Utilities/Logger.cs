// Karel Kroeze
// Logger.cs
// 2017-05-22

using System.Diagnostics;
using Verse;

namespace WorkTab
{
    public static class Logger
    {
        public static string Identifier => "WorkTab";

        public static string FormatMessage( string msg ) { return Identifier + " :: " + msg; }

        [Conditional( "DEBUG" )]
        public static void Debug( string msg ) { Log.Message( FormatMessage( msg ) ); }

        [Conditional( "TRACE" )]
        public static void Trace( string msg ) { Log.Error( FormatMessage( msg ) ); }

        public static void Message( string msg ) { Log.Message( FormatMessage( msg ) ); }
    }
}