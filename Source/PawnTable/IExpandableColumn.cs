// Karel Kroeze
// IExpandableColumn.cs
// 2017-05-28

using System.Collections.Generic;
using RimWorld;

namespace WorkTab
{
    public interface IExpandableColumn
    {
        bool Expanded { get; set; }
        bool CanExpand { get; }
        List<PawnColumnDef> ChildColumns { get; }
    }
}