// IExpandableColumn.cs
// Copyright Karel Kroeze, 2017-2020

using System.Collections.Generic;
using RimWorld;

namespace WorkTab {
    public interface IExpandableColumn {
        bool CanExpand { get; }
        List<PawnColumnDef> ChildColumns { get; }
        bool Expanded { get; set; }
    }
}
