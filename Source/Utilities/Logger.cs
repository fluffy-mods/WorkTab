// Logger.cs
// Copyright Karel Kroeze, 2018-2020

using System.Diagnostics;
using Verse;

namespace WorkTab
{
    public static class Logger
    {
        public static string Identifier => "WorkTab";

        [Conditional( "DEBUG" )]
        public static void Assert( object obj, string name )
        {
            Debug( $"{name} :: {obj ?? "NULL"}" );
        }

        [Conditional( "DEBUG" )]
        public static void Debug( string msg )
        {
            Log.Message( FormatMessage( msg ) );
        }

        public static void Error( string msg )
        {
            Log.Error( FormatMessage( msg ) );
        }

        public static string FormatMessage( string msg )
        {
            return Identifier + " :: " + msg;
        }

        public static void Message( string msg )
        {
            Log.Message( FormatMessage( msg ) );
        }

        [Conditional( "TRACE" )]
        public static void Trace( string msg )
        {
            Log.Error( FormatMessage( msg ) );
        }
    }
}