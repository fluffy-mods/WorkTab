// Karel Kroeze
// IAlternatingColumn.cs
// 2017-05-28

using UnityEngine;

namespace WorkTab
{
    public interface IAlternatingColumn
    {
        bool MoveDown { get; set; }
        Vector2 LabelSize { get; }
    }
}