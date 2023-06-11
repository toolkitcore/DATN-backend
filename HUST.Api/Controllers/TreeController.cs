using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HUST.Api.Controllers
{
    /// <summary>
    /// Lớp controller cung cấp api liên quan đến view tree
    /// </summary>
    public class TreeController : BaseApiController
    {
        #region Fields
        private readonly IConceptService _conceptService;
        #endregion

        #region Constructors

        public TreeController(IHustServiceCollection serviceCollection, 
            IConceptService conceptService) : base(serviceCollection)
        {
            _conceptService = conceptService;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Lấy dữ liệu tree của concept
        /// </summary>
        /// <param name="conceptId"></param>
        /// <returns></returns>
        [HttpGet("get_tree")]
        public async Task<IServiceResult> GetTree(Guid conceptId)
        {
            var res = new ServiceResult();
            try
            {
                return await _conceptService.GetTree(conceptId);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Lấy dữ liệu tree: các concept cha của 1 concept
        /// </summary>
        /// <param name="conceptId"></param>
        /// <returns></returns>
        [HttpGet("get_concept_parents")]
        public async Task<IServiceResult> GetConceptParents(Guid conceptId)
        {
            var res = new ServiceResult();
            try
            {
                return await _conceptService.GetConceptParents(conceptId);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }


        /// <summary>
        /// Lấy dữ liệu tree: các concept con của 1 concept
        /// </summary>
        /// <param name="conceptId"></param>
        /// <returns></returns>
        [HttpGet("get_concept_children")]
        public async Task<IServiceResult> GetConceptChildren(Guid conceptId)
        {
            var res = new ServiceResult();
            try
            {
                return await _conceptService.GetConceptChildren(conceptId);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Lấy dữ liệu tree: danh sách example liên kết với 1 concept theo loại mối quan hệ
        /// </summary>
        /// <param name="conceptId"></param>
        /// <returns></returns>
        [HttpGet("get_linked_example_by_relationship_type")]
        public async Task<IServiceResult> GetLinkedExampleByRelationshipType(Guid conceptId, Guid exampleLinkId)
        {
            var res = new ServiceResult();
            try
            {
                return await _conceptService.GetLinkedExampleByRelationshipType(conceptId, exampleLinkId);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }
        #endregion
    }
}
