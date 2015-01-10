using System;
using System.ComponentModel;
using System.Net;
using MediatR;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace RedisMediatorClient
{
    public class CachingHandler<TRequest, TResponse>
        : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private IRequestHandler<TRequest, TResponse> _inner;

        public CachingHandler(IRequestHandler<TRequest, TResponse> inner)
        {
            _inner = inner;
        }

        public TResponse Handle(TRequest message)
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("playtime.redis.cache.windows.net,ssl=true,password=");
            var cache = connection.GetDatabase();
            var cachable = message as IRedisCachable;
            string key = "";
            if (cachable != null)
            {
                key = cachable.GenerateKey();
                RedisValue result = cache.StringGet(key);
                if (!result.IsNull)
                {
                    Console.WriteLine("Cache hit");
                    var handle = default(TResponse);
                    return JsonConvert.DeserializeObject<TResponse>(result.ToString());
                }
            }
            var response = _inner.Handle(message);
            if (cachable != null)
            {
                cache.StringSet(key, JsonConvert.SerializeObject(response));
            }
            return response;
        }
    }
}