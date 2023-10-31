namespace PeopleDesk.Data
{
    
    public enum CasheExperiationTypeEnum
    {
        mileSecond,
        second,
        minute,
        hour,
        day,
        month,
        year
    }
    public enum GetDataFromJwtTokenRequestType
    {
        Authorization,
        TokenData,
        TokenDataIfAuthorize
    }

    public enum PermissionLebelCheck
    {
        Full,
        BusinessUnit,
        WorkplaceGroup,
        Workplace,
        Wing,
        SoleDepo,
        Region,
        Area,
        Territory
    }

}
