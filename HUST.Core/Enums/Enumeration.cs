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
    /// Trạng thái tài khoản người dùng
    /// </summary>
    public enum UserStatus
    {
        Active = 1,
        NotActivated = 2,
        Deleted = 3
    }
}
