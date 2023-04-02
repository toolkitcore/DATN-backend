namespace HUST.Core.Models
{
    /// <summary>
    /// Lớp base cho các lớp model
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// Hàm clone shallow đối tượng
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
