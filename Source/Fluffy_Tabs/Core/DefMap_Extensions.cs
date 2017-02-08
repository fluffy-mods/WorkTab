// Karel Kroeze
// DefMap_Extensions.cs
// 2017-02-07

using Verse;

namespace Fluffy_Tabs
{
    public static class DefMap_Extensions
    {
        public static string GetIntString<T>( this DefMap<T, int> defMap ) where T : Def, new()
        {
            string[] msg = new string[defMap.Count];
            for ( var i = 0; i < defMap.Count; i++ )
                msg[i] = defMap[i].ToString();

            return string.Join( "", msg );
        }
    }
}