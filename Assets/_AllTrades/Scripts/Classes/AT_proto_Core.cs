using System;
using System.Collections.Generic;

[Serializable]
public class AT_proto_Core
{
    public UInt16 availablePoints;
    public AT_proto_V3[] placements;
    public List<AT_proto_V3> pList;

    public AT_proto_Core()
    {
        availablePoints = 1;
        placements = new AT_proto_V3[1];
        placements[0] = new AT_proto_V3(0,0,0);
        pList = new List<AT_proto_V3>(placements);
    }
    public AT_proto_Core(UInt16 in_availablePoints, AT_proto_V3[] in_placements)
    {
        availablePoints = in_availablePoints;
        placements = in_placements;
        pList = new List<AT_proto_V3>(in_placements);
    }
}