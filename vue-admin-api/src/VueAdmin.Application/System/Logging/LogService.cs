﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueAdmin.Application.Contracts.System.Logging;
using VueAdmin.Common.Base;
using VueAdmin.Common.Extensions;
using VueAdmin.Domain.System;
using VueAdmin.Domain.System.Repositories;

namespace VueAdmin.Application.System.Logging
{
    public class LogService : VueAdminApplicationService, ILogService
    {
        private readonly ILogRepository _logRepository;

        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pageIndex">分页下标</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        public async Task<ServiceResult<PagedList<LogOut>>> QueryList(int pageIndex, int pageSize)
        {
            var result = new ServiceResult<PagedList<LogOut>>();

            var total = await _logRepository.GetCountAsync();
            var logs = _logRepository.OrderByDescending(p => p.CreateTime).PageByIndex(pageIndex, pageSize);
            var list = ObjectMapper.Map<IEnumerable<LogEntity>, IEnumerable<LogOut>>(logs);

            result.IsSuccess(new PagedList<LogOut>(total.TryToInt(), list.ToList()));
            return result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public async Task AddAsync(Exception ex)
        {
            var entity = new LogEntity
            {
                Message = ex.Message,
                StackTrace = !string.IsNullOrWhiteSpace(ex.StackTrace) ? ex.StackTrace : null,
                Exception = ex.ToString(),
                CreateTime = DateTime.Now
            };
            await _logRepository.InsertAsync(entity, true);
        }

        /// <summary>
        /// 获取Log信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ServiceResult<LogOut>> GetLog(string id)
        {
            var result = new ServiceResult<LogOut>();

            var log = await _logRepository.GetAsync(p => p.Id.ToString() == id);
            var logOut = ObjectMapper.Map<LogEntity, LogOut>(log);

            result.IsSuccess(logOut);
            return result;
        }
    }
}