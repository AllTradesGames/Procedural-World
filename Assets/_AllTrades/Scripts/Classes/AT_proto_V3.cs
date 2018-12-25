using System;

[Serializable]
public class AT_proto_V3
{
    public UInt16 x;
    public UInt16 y;
    public UInt16 z;

    public AT_proto_V3()
    {
        x = 0;
        y = 0;
        z = 0;
    }
    public AT_proto_V3(UInt16 in_x, UInt16 in_y, UInt16 in_z)
    {
        x = in_x;
        y = in_y;
        z = in_z;
    }
}