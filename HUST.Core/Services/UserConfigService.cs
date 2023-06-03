using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Services
{
    /// <summary>
    /// Serivce xử lý account
    /// </summary>
    public class UserConfigService : BaseService, IUserConfig
    {
        #region Field

        private readonly IUserRepository _userRepository;

        #endregion

        #region Constructor

        public UserConfigService(IUserRepository userRepository,
            IHustServiceCollection serviceCollection) : base(serviceCollection)
        {
            _userRepository = userRepository;
        }



        #endregion

        #region Method

        /// <summary>
        /// Lấy danh sách concept link
        /// </summary>
        /// <returns></returns>
        public async Task<IServiceResult> GetListConceptLink()
        {
            var res = new ServiceResult();

            var userId = this.ServiceCollection.AuthUtil.GetCurrentUserId();
            var data = await _userRepository.SelectObjects<ConceptLink>(new Dictionary<string, object>
            {
                { nameof(concept_link.user_id), userId }
            }) as List<ConceptLink>;

            data?.Add(new ConceptLink
            {
                ConceptLinkId = Guid.Empty,
                SysConceptLinkId = Guid.Empty,
                UserId = userId,
                ConceptLinkName = "No link",
                ConceptLinkType = (int)ConceptLinkType.NoLink,
                SortOrder = 0
            });

            res.Data = data?.OrderBy(x => x.SortOrder);

            return res;
        }
        #endregion

    }
}
