using System;

[Serializable]
public class AT_proto_User
{
    public UInt64 orgScopedID;
    public string screenName;    
    public int level;    
    public int experience;

    public AT_proto_User(UInt64 in_orgScopedID, string in_screenName, int in_level, int in_experience)
    {
        orgScopedID = in_orgScopedID;
        screenName = in_screenName;
        level = in_level;
        experience = in_experience;
    }
    public AT_proto_User(UInt64 in_orgScopedID, string in_screenName)
    {
        orgScopedID = in_orgScopedID;
        screenName = in_screenName;
        level = 1;
        experience = 0;
    }
}