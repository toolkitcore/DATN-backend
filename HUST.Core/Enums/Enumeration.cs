namespace HUST.Core.Enums
{
    /// <summary>
    /// Trạng thái ServiceResult
    /// </summary>
    public enum ServiceResultStatus
    {
        Success = 1,
        Fail = 2,
        Exception = -1
    }

    /// <summary>
    /// Loại cache
    /// </summary>
    public enum CacheSqlType
    {
        ImportDictionary = 1
    }

    /// <summary>
    /// Loại external api
    /// </summary>
    public enum ExternalApiType
    {
        WordsApi = 1,
        FreeDictionaryApi = 2
    }

    /// <summary>
    /// Trạng thái tài khoản người dùng
    /// </summary>
    public enum UserStatus
    {
        Active = 1,
        NotActivated = 2,
        Blocked = 3
    }


    /// <summary>
    /// Loại liên kết concept-concept (mặc định hệ thống)
    /// </summary>
    public enum ConceptLinkType
    {
        NoLink = 1,
        Association = 2,
        TypeOf = 3
    }

    /// <summary>
    /// Loại liên kết example-concept (mặc định hệ thống)
    /// </summary>
    public enum ExampleLinkType
    {
        NoLink = 1,
        Idiom = 2,
        Nominal = 3,
        Agent = 4,
        Patient = 5,
        Action = 6,
        DescribedBy = 7,
        Describing = 8,
        OtherPhrase = 9
    }

    /// <summary>
    /// Loại tone (mặc định hệ thống)
    /// </summary>
    public enum ToneType
    {
        Neutral = 1,
        Informal = 2,
        Formal = 3,
        SlightlyInformal = 4,
        SlightlyFormal = 5
    }

    /// <summary>
    /// Loại mode (mặc định hệ thống)
    /// </summary>
    public enum ModeType
    {
        Neutral = 1,
        Spoken = 2,
        Written = 3
    }

    /// <summary>
    /// Loại register (mặc định hệ thống) 
    /// </summary>
    public enum RegisterType
    {
        Neutral = 1,
        Academic = 2,
        Literature = 3,
        Business = 4,
        Law = 5,
        Journalism = 6,
        Medicine = 7,
        IT = 8,
        Other = 9
    }

    /// <summary>
    /// Loại nuance (mặc định hệ thống)
    /// </summary>
    public enum NuanceType
    {
        Neutral = 1,
        OldFashioned = 2,
        Humorous = 3,
        OftPositive = 4,
        OftNegative = 5
    }

    /// <summary>
    /// Loại dialect (mặc định hệ thống)
    /// </summary>
    public enum DialectType
    {
        Neutral = 1,
        American = 2,
        British = 3,
        Other = 4
    }
}
