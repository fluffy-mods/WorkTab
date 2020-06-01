// IAlternatingColumn.cs
// Copyright Karel Kroeze, 2017-2020

using UnityEngine;

namespace WorkTab
{
    public interface IAlternatingColumn
    {
        Vector2 LabelSize { get; }
        bool    MoveDown  { get; set; }
    }
}