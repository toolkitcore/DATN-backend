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
    public class UserConfigService : BaseService, IUserConfigService
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
        /// Lấy danh sách tất cả config: 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserConfigData> GetAllConfigData(string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = this.ServiceCollection.AuthUtil.GetCurrentUserId()?.ToString();
            }

            var tables = new string[]
            {
                nameof(concept_link),
                nameof(example_link),
                nameof(tone),
                nameof(mode),
                nameof(register),
                nameof(nuance),
                nameof(dialect)
            };

            // Không dùng từ "params"
            var param = new Dictionary<string, Dictionary<string, object>>()
            {
                {
                    nameof(concept_link),
                    new Dictionary<string, object> { { nameof(concept_link.user_id), userId } }
                },
                {
                    nameof(example_link),
                    new Dictionary<string, object> { { nameof(example_link.user_id), userId } }
                },
                {
                    nameof(tone),
                    new Dictionary<string, object> { { nameof(tone.user_id), userId } }
                },
                {
                    nameof(mode),
                    new Dictionary<string, object> { { nameof(mode.user_id), userId } }
                },
                {
                    nameof(register),
                    new Dictionary<string, object> { { nameof(register.user_id), userId } }
                },
                {
                    nameof(nuance),
                    new Dictionary<string, object> { { nameof(nuance.user_id), userId } }
                },
                {
                    nameof(dialect),
                    new Dictionary<string, object> { { nameof(dialect.user_id), userId } }
                }
            };

            var queryRes = await _userRepository.SelectManyObjects(tables, param) as Dictionary<string, object>;

            if (queryRes == null)
            {
                return null;
            }

            var res = new UserConfigData
            {
                ListConceptLink = queryRes[nameof(concept_link)] as List<concept_link>,
                ListExampleLink = queryRes[nameof(example_link)] as List<example_link>,
                ListTone = queryRes[nameof(tone)] as List<tone>,
                ListMode = queryRes[nameof(mode)] as List<mode>,
                ListRegister = queryRes[nameof(register)] as List<register>,
                ListNuance = queryRes[nameof(nuance)] as List<nuance>,
                ListDialect = queryRes[nameof(dialect)] as List<dialect>
            };
            //var lstExampleLink = (queryRes[nameof(example_link)] as List<object>).Cast<example_link>().ToList()

            return res;
        }

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
