﻿using Meowv.Blog.Caching.News;
using Meowv.Blog.Domain.News;
using Meowv.Blog.Domain.News.Repositories;
using Meowv.Blog.Dto.News.Params;
using Meowv.Blog.Response;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meowv.Blog.News.Impl
{
    public class HotService : ServiceBase, IHotService
    {
        private readonly IHotRepository _hots;
        private readonly IHotCacheService _cache;

        public HotService(IHotRepository hots, IHotCacheService cache)
        {
            _hots = hots;
            _cache = cache;
        }

        /// <summary>
        /// Get the list of sources.
        /// </summary>
        /// <returns></returns>
        [Route("api/meowv/hots/source")]
        public async Task<BlogResponse<Dictionary<string, string>>> GetSourcesAsync()
        {
            return await _cache.GetSourcesAsync(async () =>
            {
                return await Task.FromResult(new BlogResponse<Dictionary<string, string>>
                {
                    Result = Hot.KnownSources.Dictionary
                });
            });
        }

        /// <summary>
        /// Get the list of hot news by source.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [Route("api/meowv/hots/{source}")]
        public async Task<BlogResponse<HotDto>> GetHotsAsync(string source)
        {
            return await _cache.GetHotsAsync(source, async () =>
            {
                var response = new BlogResponse<HotDto>();

                if (!Hot.KnownSources.Dictionary.ContainsKey(source))
                {
                    response.IsFailed($"The hot source not exists.");
                    return response;
                }

                var hot = await _hots.GetAsync(x => x.Source == source);
                var result = ObjectMapper.Map<Hot, HotDto>(hot);

                response.Result = result;
                return response;
            });
        }
    }
}