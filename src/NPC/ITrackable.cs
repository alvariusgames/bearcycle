using Godot;
using System;
using System.Collections.Generic;

public interface ITrackable {
    IEnumerable<Node2D> NodesToTrack {get; }
    Boolean ShouldTrackNodesNow {get; }
}
